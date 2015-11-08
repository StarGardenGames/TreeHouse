using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController : PhysicalObject{	
	//suppress warnings
	#pragma warning disable 1691,168,219,414

   #region Properties & Variables

	//singleton
	public static PlayerController instance;

	//animator
	private PlayerAnimController animator;
	
	// Movement Parameters
	public float acceleration;
	public float decelleration;
	public float maxSpeed;
	public float hangMaxSpeed;
	public float upGravity;
	public float downGravity;
	public float terminalVelocity;
	public float jumpVelocity;
	public float jumpMargin;
	

	// Verticle movement flags
	private bool jumpPressedOnPreviousFrame;
	private bool bounced;
	private float jumpPressedTime;
	private bool jumping;
	private bool grounded;

	// Raycasting Variables
	public int verticalRays = 8;
	float Margin = 0.05f;
	float verticalOverlapThreshhold = .3f;

	private Rect box;
	private float colliderHeight;
	private float colliderWidth;
	private float colliderDepth;

	private CollisionChecker colCheck;

	// Vars for Z-locking
	private float zlock = int.MinValue;
	private bool zlockFlag;
	
	private Crate crate = null;
	private bool passivePush = false;
	private Vector3 grabAxis = Vector3.zero;
	private bool launched;

	//Vars for edge grabbing
	private Vector3[] cuboid;
	private Edge grabbedEdge = null;
	private EdgeState edgeState = EdgeState.FAR_FROM_EDGE;
	
	private int climbTimer = 0;
	private const int CLIMB_TIME = 10;

	private int kickTimer = 0;
	private const int KICK_TIME = 10;
	
	bool cutsceneMode = false;
	
	private bool _paused;
	
	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;
	private const float epsilon = .1f;
	
   #endregion
	
	#region Init
	
	void Awake(){
		generateSingleton();
	}
	
	void Start () {
		base.Init();
		
		linkAnimator();
		
		initMovementVariables();
	
		initCollisionVariables();

		registerEventHandlers();
	}
	
	public void Reset() {		
		initMovementVariables();
	}
	
	private void generateSingleton(){
		if(instance == null)
			instance = this;
		else if(instance!= this)
			Destroy(this);
	}
	
	private void linkAnimator(){
		animator = PlayerAnimController.instance;
	}
	
	private void initMovementVariables(){
		grounded = false;
		jumpPressedOnPreviousFrame = false;
	}

	private void initCollisionVariables(){
		cuboid = new Vector3[2];
		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;
		colCheck = new CollisionChecker(GetComponent<Collider> ());
	}
	
	private void registerEventHandlers(){
		GameStateManager.instance.GamePausedEvent += OnPauseGame;
	}
	
	#endregion Init
	
	void Update(){
		if (canMove())	checkForJump();
	}

	#region Jump
	private void checkForJump(){
		updateJumpPressedTime();
		
		bool jumpInputed = (Time.time - jumpPressedTime) < jumpMargin;
		bool canJump = (isGrounded() || edgeState == EdgeState.HANGING) && !GrabbedCrate();
		if(canJump && jumpInputed)
			jump();
	}
	
	private void updateJumpPressedTime(){
		bool jumpPressedThisFrame = InputManager.instance.JumpStatus();

		if (jumpPressedThisFrame && !jumpPressedOnPreviousFrame){
			jumpPressedTime = Time.time;
		}else if (!jumpPressedThisFrame){
			jumpPressedTime = 0;
		}

		jumpPressedOnPreviousFrame = jumpPressedThisFrame;
	}

	private void jump(){
		grounded = false;
		velocity = new Vector3(velocity.x, jumpVelocity, velocity.z);
		jumpPressedTime = 0;
		jumping = true;
		
		ReleaseEdge();
	}
	
	#endregion Jump
	
	// Collision detection and velocity calculations are done in the fixed update step
	void FixedUpdate(){
		updateTimers();
		updateCuboid();
		
		if(canMove()) move();
		
		if(!isDisabled()) updateStateVariables();
	}
	
	private void updateTimers(){
		if(kickTimer > 0) kickTimer--;
		if(climbTimer > 0) climbTimer--;
	}
	
	private void updateCuboid(){
		Vector3 halfScale = gameObject.transform.lossyScale * .5f;
		cuboid[0] = gameObject.transform.position - halfScale;
		cuboid[1] = gameObject.transform.position + halfScale;
	}

	private void move(){
		if(edgeState == EdgeState.HANGING){
			bool edgeOnXAxis = grabbedEdge.getOrientation() % 2 == 1;
			if(edgeOnXAxis)
				shimmyOnAxis(X);
			else
				shimmyOnAxis(Z);
		}else{
			moveOnAxis(X);
			moveOnAxis(Z);
		}
	}
	
	private void moveOnAxis(int axis){
		float newVelocity = velocity[axis];
		
		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
			-InputManager.instance.GetSideMovement();
		
		if (axisInput != 0){
			newVelocity += acceleration * Mathf.Sign(axisInput);
			newVelocity = Mathf.Clamp(newVelocity, 
				-maxSpeed * Mathf.Abs(axisInput), 
				maxSpeed * Mathf.Abs(axisInput)
			);
		}else if (velocity[axis] != 0){
			newVelocity -= decelleration * Mathf.Sign(newVelocity);
			if(Mathf.Sign(newVelocity) != Mathf.Sign(velocity[axis])) newVelocity = 0;
		}
		
		velocity[axis] = newVelocity;
	}
	
	private void shimmyOnAxis(int axis){
		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
			-InputManager.instance.GetSideMovement();
		
		float newVelocity = hangMaxSpeed * Mathf.Sign(axisInput);
		
		Vector3 pos = transform.position;
		pos[axis] += newVelocity/50f;
		
		if(axisInput == 0 || !grabbedEdge.isPositionValidOnEdge(pos))
			newVelocity = 0;
		
		velocity[axis] = newVelocity;
	}
	
	private void updateStateVariables(){
		if(isFalling()) jumping = false;
	}
	
	#region Collisions
	 
	public void CheckCollisions(){
		CheckCollisionsOnAxis(Y);
		passivePush = false;
		CheckCollisionsOnAxis(X);
		CheckCollisionsOnAxis(Z);
	}
	
	void CheckCollisionsOnAxis(int axis){
		Vector3 axisVector = getAxisVector(axis);
		
		Vector3 trajectory;

		RaycastHit[] hits = colCheck.CheckCollisionOnAxis(axis,velocity, Margin);

		float distToCollision = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null && !hitInfo.collider.isTrigger)
			{
				float verticalOverlap = getVerticalOverlap(hitInfo);
				bool significantVerticalOverlap = 
					verticalOverlap > verticalOverlapThreshhold;
				if(axis != Y && !significantVerticalOverlap){
					transform.Translate(new Vector3(0f,verticalOverlap,0f));
					continue;
				}
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity[axis] * axisVector;
					CollideWithObject(hitInfo, trajectory);
				} else if (distToCollision == -1 || distToCollision > hitInfo.distance) {
					distToCollision = hitInfo.distance;
					if(axis == Y){
						if (velocity.y < 0) {
							grounded = true;
							launched = false;
							// New Z-lock
							if (hitInfo.collider.gameObject.tag != "Ground" && GameStateManager.instance.currentPerspective == PerspectiveType.p2D) {
								Vector3 pos = transform.position;
								pos.z = hitInfo.collider.gameObject.transform.position.z;
								transform.position = pos;
							}
						}
                        // Z-lock
                        if (hitInfo.collider.gameObject.GetComponent<LevelGeometry>())
                            zlock = hitInfo.transform.position.z;
                        else
                            zlock = int.MinValue;
					}else{
						transform.Translate(
							axisVector * 
							Mathf.Sign(velocity[axis]) * 
							(hitInfo.distance - getDimensionAlongAxis(axis) / 2)
						);
					}
					trajectory = velocity[axis] * axisVector;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}
		
		
		bool collisionWithTangibleOccurred = distToCollision!=-1;
		if (collisionWithTangibleOccurred) {
			if(axis == Y){
				if (!bounced) {
					transform.Translate(
						axisVector * 
						Mathf.Sign(velocity[axis]) * 
						(distToCollision - getDimensionAlongAxis(axis) / 2)
					);
					velocity[axis] = 0f;
				} else {
					bounced = false;
				}
			}else{
				if(!passivePush) velocity[axis] = 0f;
			}
		}else if(axis == Y){
			grounded = false;
		}
	}
	
	Vector3 getAxisVector(int axis){
		switch(axis){
			case X: return Vector3.right;
			case Y: return Vector3.up;
			case Z: return Vector3.forward;
			default:
				throw new System.ArgumentException("Invalid Axis Index");
		}
	}
	
	float getDimensionAlongAxis(Vector3 axis){
		Vector3 norm = axis.normalized;
		if(Mathf.Abs(norm.x) == 1) return colliderWidth;
		if(Mathf.Abs(norm.y) == 1) return colliderHeight;
		if(Mathf.Abs(norm.z) == 1) return colliderDepth;
		throw new System.ArgumentException("Input Vector must only have values along one axis");
	}
	
	//Methods related to Insignificant Ovelap Adjustments
	private float getVerticalOverlap(RaycastHit hitInfo){
		Collider hitCollider = hitInfo.collider;
		float hitColliderHeight = hitCollider.bounds.max.y - hitCollider.bounds.min.y;
		float myBottomY = GetComponent<Collider>().bounds.min.y;
		float hitTopY = hitCollider.bounds.max.y;
		float overlap = hitTopY - myBottomY;
		return overlap;
	}
	
	//Checks Type of Object and collides accordingly
	private void CollideWithObject(RaycastHit hitInfo, Vector3 trajectory) {
		GameObject other = hitInfo.collider.gameObject;
		float colliderDim = 0;
		if (trajectory.normalized == Vector3.up || trajectory.normalized == Vector3.down)
			colliderDim = colliderHeight;
		if (trajectory.normalized == Vector3.right || trajectory.normalized == Vector3.left)
			colliderDim = colliderWidth;
		if (trajectory.normalized == Vector3.forward || trajectory.normalized == Vector3.back)
			colliderDim = colliderDepth;
		// Bounce Pad
		if (trajectory.normalized == Vector3.down) {
			if (other.GetComponent<BouncePad>()) {
				velocity = other.transform.up * other.GetComponent<BouncePad>().GetBouncePower();
				if (!other.transform.up.Equals(Vector3.up))
					launched = true;
				other.GetComponent<BouncePad>().Animate();
				bounced = true;
			}
			foreach (LandOnObject c in other.GetComponents<LandOnObject>()) {
				c.LandedOn ();
			}
						
		}
		// Crate
		if (trajectory.normalized != Vector3.down && trajectory.normalized != Vector3.zero && 
				other.GetComponent<Crate>() && !other.GetComponent<Crate>().IsAxisBlocked(trajectory)) {
			other.GetComponent<Crate>().FreePush((trajectory*0.75f).x, (trajectory*0.75f).z);
			passivePush = true;
		}
		// PushSwitchOld
		if (other.GetComponent<PushSwitchOld>() && colliderDim == colliderWidth) {
			transform.Translate(0, 0.1f, 0);
		}
  		//Collision w/ PlayerInteractable
		foreach (Interactable c in other.GetComponents<Interactable>()) {
			c.EnterCollisionWithPlayer ();
		}
	}

	#endregion Collisions
	
	// LateUpdate is used to actually move the position of the player
	void LateUpdate () {
		if (canMove()) applyGravity();
		
		CheckCollisions();

		if(canMove()) applyMovement();
    }

	private void applyGravity(){
		if (edgeState != EdgeState.HANGING){
			if (velocity.y <= 0)
				velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - upGravity * Time.deltaTime, -terminalVelocity), velocity.z);
			else
				velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - downGravity * Time.deltaTime, -terminalVelocity), velocity.z);
		}else{
			velocity.y = 0;
		}
	}
	 
	private void applyMovement(){
		if (GrabbedCrate()){
			Vector3 drag = Vector3.Dot(velocity, grabAxis) * grabAxis * 0.75f;
			crate.SetVelocity(drag.x, drag.z);
			transform.Translate(drag * Time.deltaTime);
		}else{
			transform.Translate(velocity * Time.deltaTime);
		}
	}
	 
	public bool Check2DIntersect() {
		// True if any ray hits a collider
		bool connected = false;

		//reference variables
		float minX 		= GetComponent<Collider>().bounds.min.x + Margin;
		float centerX 	= GetComponent<Collider>().bounds.center.x;
		float maxX 		= GetComponent<Collider>().bounds.max.x - Margin;
		float minY 		= GetComponent<Collider>().bounds.min.y + Margin;
		float centerY 	= GetComponent<Collider>().bounds.center.y;
		float maxY 		= GetComponent<Collider>().bounds.max.y - Margin;
		float centerZ	= GetComponent<Collider>().bounds.center.z;

		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, maxY, centerZ),
			new Vector3(maxX, maxY, centerZ),
			new Vector3(minX, minY, centerZ),
			new Vector3(maxX, minY, centerZ),
			new Vector3(centerX, centerY, centerZ)
		};

		//ignore intagible objects
		GameObject[] intangibles = GameObject.FindGameObjectsWithTag("Intangible");
		foreach (GameObject obj in intangibles) {
			obj.layer = 2;
		}

		//check all startpoints
		for(int i = 0; i < startPoints.Length; i++)
			connected = connected || Physics.Raycast(startPoints[i], Vector3.forward) || Physics.Raycast(startPoints[i], -1 * Vector3.forward);

		foreach (GameObject obj in intangibles) {
			obj.layer = 0;
		}

		return connected;
	}

	#region EdgeGrabbing
	
	private void ReleaseEdge(){
		if(grabbedEdge!=null)
			grabbedEdge.resetStatus();
		grabbedEdge = null;
		edgeState = EdgeState.FAR_FROM_EDGE;
	}

   //note: this is only called from the Edge.cs
   public void UpdateEdgeState(Edge e, byte edgeState){
		UpdateEdgeState(e,edgeState,-1);
	}
	
	public void UpdateEdgeState(Edge e, byte edgeState, int animState){
		switch(edgeState){
			case 0:
				if(grabbedEdge != null && e!= null){
					if(grabbedEdge == e){
						this.edgeState = EdgeState.FAR_FROM_EDGE;
						grabbedEdge =null;
					}
				}
				
				//adjust animation state
				if(animState!= -1)
					animator.updateEdgeState(animState);
				
				if(animState == 5)
					climbTimer = CLIMB_TIME;
				break;
			case 1:
				this.edgeState = EdgeState.CLOSE_TO_EDGE;
				grabbedEdge = e;
				break;
			case 2:
				this.edgeState	 = EdgeState.HANGING;
				//stop moving
				velocity = Vector3.zero;
				//lock y
				Vector3 pos = transform.position;
				pos.y = e.gameObject.transform.position.y + (e.gameObject.transform.lossyScale.y * .5f) - (colliderHeight * .5f);
				//z-lock to edge
				if(!GameStateManager.is3D())
					pos.z = grabbedEdge.transform.position.z;
				//update transform
				transform.position = pos;
				break;
		}
	}
	
	public void Grab(Crate crate) {
		this.crate = crate;
		if (crate == null)
			return;
		float xDiff = Mathf.Abs(crate.transform.position.x - transform.position.x);
		float zDiff = Mathf.Abs (crate.transform.position.z - transform.position.z);
		bool closerAlongXAxis = xDiff > zDiff;
		if (!GameStateManager.is3D() || closerAlongXAxis) {
			grabAxis = Vector3.right * xDiff;
		} else {
			grabAxis = Vector3.forward * zDiff;
		}
	}
	
	#endregion EdgeGrabbing

	#region Accessor Methods
	
	public bool GrabbedCrate() { return crate != null; }
	
	public bool isPassivelyPushing(){ return passivePush; }
	
	public Vector3[] getCuboid(){ return cuboid; }
	
	public void Teleport(Vector3 newPos){
		transform.position = newPos;
		gameObject.GetComponent<BoundObject>().updateBounds();
	}

	public Vector3 GetVelocity(){ return velocity; }

	public bool isPaused(){ return _paused; }

	public void setCutsceneMode(bool c){ cutsceneMode = c; }
	
	public bool canMove(){
		return !isDisabled() && !isKicking() && !isClimbing() && !launched;
	}
	
	public bool isDisabled(){
		return isPaused() || cutsceneMode;
	}
	
	public float getDimensionAlongAxis(int axis){
		switch(axis){
			case X: return colliderWidth;
			case Y: return colliderHeight;
			case Z: return colliderDepth;
			default:
				throw new System.ArgumentException("Invalid Axis Index");
		}
	}
	
	public float getColliderWidth(){ return colliderWidth; }
	
	public float getColliderHeight(){ return colliderHeight; }
	
	public float getColliderDepth(){ return colliderDepth; }
	
	public EdgeState getEdgeState(){ return edgeState; }
	
	//TODO store axis info in grabbed edge
	public int getEdgeOrientation(){ return grabbedEdge.getOrientation(); }
	
	public Vector3 getGrabAxis(){ return grabAxis; }
	
	public bool isClimbing(){ return climbTimer > 0; }
	
	public bool isKicking(){ return kickTimer > 0; }
	
	public bool isRunning(){ 
		bool movingFast = velocity.magnitude > maxSpeed/2;
		return isGrounded() && movingFast; 
	}
	
	public bool isWalking(){
		bool movingSlow = 0 < velocity.magnitude && velocity.magnitude <= maxSpeed/2;
		return isGrounded() && movingSlow; 
	}
	
	public bool isFalling(){ 
		bool onEdge = edgeState == EdgeState.HANGING;
		return velocity.y < -epsilon && !onEdge;
	}

   public bool isGrounded(){ 
		return grounded; 
	}
	
	public bool isJumping(){ return jumping; }
	
	public bool isShimmying(){ 
		bool moving = velocity.magnitude > epsilon;
		bool onEdge = edgeState == EdgeState.HANGING;
		return moving && onEdge;
	}
	
	public bool isLaunched(){ return launched; }
	
	#endregion Accessor Methods
	
	private void OnPauseGame(bool p){
		if(p)
			animator.pauseAnimation();
		else
			animator.resumeAnimation();
		
		_paused = p;
	}

	public void StartKick(){
	  kickTimer = KICK_TIME;
	  velocity.x = 0;
	  velocity.z = 0;
	}
}

public enum EdgeState{
	FAR_FROM_EDGE, CLOSE_TO_EDGE, HANGING
}
