using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController : PhysicalObject{	
	public bool testFalling;

	//suppress warnings
	#pragma warning disable 1691,168,219,414

   #region Properties & Variables

	//singleton
	public static PlayerController instance;

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
	private bool grounded;
	private bool falling;
	private bool jumpPressedOnPreviousFrame;
	private bool bounced;
	private float jumpPressedTime;
	private bool walking;
	private bool shimmying;
	private bool running;

	// Raycasting Variables
	public int verticalRays = 8;
	float Margin = 0.05f;
	float verticalOverlapThreshhold = .3f;

	private Rect box;
	private float colliderHeight;
	private float colliderWidth;
	private float colliderDepth;

	private CollisionChecker colCheck;

    // Vector used to store new veolicty
    //private Vector3 velocity; functionality has been moved to PhysicalObject

	// Vars for Z-locking
	private float zlock = int.MinValue;
	private bool zlockFlag;

	private Animator anim;
	private GameObject model;
	private float prePauseAnimSpeed = 0;
	
	private Crate crate = null;
	private bool pushFlag = false;
	private Vector3 grabAxis = Vector3.zero;
	private bool launched;

	//direction of player
	private float orientation = 0;

	//Vars for edge grabbing
	private Vector3[] cuboid;
	private Edge grabbedEdge = null;
	private EdgeState edgeState = EdgeState.FAR_FROM_EDGE;
	private int climbTimer = 0;
	private const int CLIMB_TIME = 10;

	private int kickTimer = 0;
	private const int KICK_TIME = 10;
	
	bool cutsceneMode = false;

	//Emitter
	public ParticleSystem dustEmitter;
	//public ParticleSystem landingEmitter;
	AnimatorStateInfo currentState;
	private bool _paused;
	
	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;
	private float epsilon = .1f;

   #endregion
	
	#region Init
	
	void Awake(){
		generateSingleton();
	}
	
	void Start () {
		base.Init();
	
		initMovementVariables();
	
		initCollisionVariables();

		initAnimationVariables();

		initParticleVariables();

		registerEventHandlers();
	}
	
	public void Reset() {		
		initMovementVariables();
		
		initParticleVariables();
	}
	
	private void generateSingleton(){
		if(instance == null)
			instance = this;
		else if(instance!= this)
			Destroy(this);
	}
	
	private void initMovementVariables(){
		grounded = false;
		falling = true;
		jumpPressedOnPreviousFrame = false;
	}

	private void initCollisionVariables(){
		cuboid = new Vector3[2];
		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;
		colCheck = new CollisionChecker(GetComponent<Collider> ());
	}
	
	private void initAnimationVariables(){
		anim = GetComponentInChildren<Animator>();
		model = anim.gameObject;
	}
	
	private void initParticleVariables(){
		dustEmitter.enableEmission = false;	
		//landingEmitter.enableEmission = false;
	}
	
	private void registerEventHandlers(){
		GameStateManager.instance.GamePausedEvent += OnPauseGame;
	}
	
	#endregion Init
	
	void Update(){
		testFalling = falling;
		if (canMove()){		
			checkForJump();
			updateAnimationVariables();
		}
	}
	
	private void checkForJump(){
		updateJumpPressedTime();
		
		bool jumpInputed = (Time.time - jumpPressedTime) < jumpMargin;
		bool canJump = (grounded || edgeState == EdgeState.HANGING) && !GrabbedCrate();
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
		
		ReleaseEdge();
		
		anim.SetTrigger("Jump");
		anim.SetInteger("EdgeState", 6);
	}
	
	private void updateAnimationVariables(){
		if (falling) anim.SetBool("Kick", false);
		anim.SetBool("Falling", falling);
		anim.SetBool("Grounded", grounded);
	}
	
	// Collision detection and velocity calculations are done in the fixed update step
	void FixedUpdate(){
		updateTimers();
		updateCuboid();
		
		if(canMove()) move();
		
		if(!isDisabled()){
			updateStateVariables();
			
			updateOrientation();
		}
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
			if(edgeOnXAxis){
				shimmyOnAxis(X);
			}else{
				shimmyOnAxis(Z);
			}
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
		float newVelocity = velocity[axis];
		
		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
			InputManager.instance.GetSideMovement();
			
		Vector3 pos = transform.position;
		pos[axis] += newVelocity/50f;
		
		if(axisInput != 0 && grabbedEdge.isPositionValidOnEdge(pos)){
			newVelocity = hangMaxSpeed * Mathf.Sign(axisInput);
			//update edgeState for animation
			if(newVelocity!=0 && anim.GetInteger("EdgeState") < 3){
				anim.SetInteger("EdgeState", 3);
			}
		}else{
			newVelocity = 0;
		}
		
		velocity[axis] = newVelocity;
	}
	
	private void updateStateVariables(){
		updateMovementStates();
		
		updateCrateStates();
		
		updateEdgeStates();
	}
	
	private void updateMovementStates(){
		bool moving = velocity.magnitude > epsilon;
		walking = moving && edgeState != EdgeState.HANGING;
		shimmying = moving && edgeState == EdgeState.HANGING;
		running = velocity.magnitude > maxSpeed / 2;
		anim.SetBool("Walking", walking && !running && !GrabbedCrate());
		anim.SetBool("Running", running && !GrabbedCrate());
		anim.SetBool("Kick", false);
		
		falling = velocity.y < 0f && edgeState != EdgeState.HANGING;
		
		dustEmitter.enableEmission =(running || walking) && grounded;
	}
	
	private void updateCrateStates(){
		if (!GrabbedCrate()){
			if (!pushFlag) anim.SetBool("Pushing", false);
			anim.SetBool("Pulling", false);
			anim.SetBool("CrateIdle", false);
		} else {
			float crateVel = Vector3.Dot(velocity, grabAxis);
			anim.SetBool("Pushing", crateVel > epsilon);
			anim.SetBool("Pulling", crateVel < -epsilon);
			anim.SetBool("CrateIdle", -epsilon <= crateVel && crateVel <= epsilon);
		}
	}
	
	private void updateEdgeStates(){
		int animEdgeState = anim.GetInteger("EdgeState");
		if(animEdgeState < 3 || (animEdgeState == 5 && !isClimbing()))	
			anim.SetInteger("EdgeState", (int)edgeState);
		else if(animEdgeState == 3){
			if(Mathf.Abs(velocity.z) < 0.1 && Mathf.Abs(velocity.x) < 0.1)
				anim.SetInteger("EdgeState", (int)edgeState);
		}else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("HangBegin") && 
				!anim.GetCurrentAnimatorStateInfo(0).IsName("HangShimmying")){
			anim.SetInteger("EdgeState", (int)edgeState);
		}
	}
	
	private void updateOrientation(){
		if (!isClimbing() && walking && !GrabbedCrate() && edgeState != EdgeState.HANGING && anim.GetInteger("EdgeState") == 0){
			orientation = Mathf.Rad2Deg * Mathf.Atan2(-velocity.z, velocity.x) + 90;
		}else if(edgeState == EdgeState.HANGING){
			orientation = (-1 - grabbedEdge.getOrientation()) * 90;
		}
		model.transform.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
	}
	
	#region Collisions
	 
	public void CheckCollisions(){
		CheckCollisionsOnAxis(Y);
		pushFlag = false;
		CheckCollisionsOnAxis(X);
		CheckCollisionsOnAxis(Z);
	}
	
	void CheckCollisionsOnAxis(int axis){
		Vector3 axisVector = getAxisVector(axis);
		
		Vector3 trajectory;

		RaycastHit[] hits = colCheck.CheckCollisionOnAxis(axis,velocity, Margin);

		float close = -1;
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
				} else if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					if(axis == Y){
						if (velocity.y < 0) {
							grounded = true;
							falling = false;
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
		
		if (close != -1) {
			if(axis == Y){
				if (!bounced) {
					transform.Translate(
						axisVector * 
						Mathf.Sign(velocity[axis]) * 
						(close - getDimensionAlongAxis(axis) / 2)
					);
					velocity[axis] = 0f;
				} else {
					bounced = false;
				}
			}else{
				if(!pushFlag) velocity[axis] = 0f;
			}
		} else if(axis == Y){
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
				anim.SetTrigger("Jump");
				bounced = true;
			}
			foreach (LandOnObject c in other.GetComponents<LandOnObject>()) {
				c.LandedOn ();
			}
						
		}
		// Crate
		if (trajectory.normalized != Vector3.down && trajectory.normalized != Vector3.zero && other.GetComponent<Crate>() && !other.GetComponent<Crate>().IsAxisBlocked(trajectory)) {
			other.GetComponent<Crate>().FreePush((trajectory*0.75f).x, (trajectory*0.75f).z);
			pushFlag = true;
			if (crate == null && velocity != Vector3.zero)
				anim.SetBool("Pushing", true);
		} else if (crate == null && !pushFlag) {
			anim.SetBool("Pushing", false);
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
			if (!grounded) {
				if (velocity.y <= 0)
					velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - upGravity * Time.deltaTime, -terminalVelocity), velocity.z);
				else
					velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - downGravity * Time.deltaTime, -terminalVelocity), velocity.z);
			}
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
		
	//This can only be called from self
	void ReleaseEdge(){
		if(grabbedEdge!=null)
			grabbedEdge.resetStatus();
		grabbedEdge = null;
		edgeState = EdgeState.FAR_FROM_EDGE;
	}

	public bool isFalling(){
		return falling;
	}

    public bool isGrounded() {
        return grounded;
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
				if(animState!= -1){
					anim.SetInteger("EdgeState", animState);
				}
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
				Vector3 pos = gameObject.transform.position;
				pos.y = e.gameObject.transform.position.y + (e.gameObject.transform.lossyScale.y * .5f) - (colliderHeight * .5f);
				gameObject.transform.position = pos;
				break;
		}
	}
	
	public void Grab(Crate crate) {
		this.crate = crate;
		if (crate == null)
			return;
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p2D || Mathf.Abs (crate.transform.position.x - transform.position.x) > Mathf.Abs (crate.transform.position.z - transform.position.z)) {
			grabAxis = Vector3.right * Mathf.Sign(crate.transform.position.x - transform.position.x);
		} else {
			grabAxis = Vector3.forward * Mathf.Sign(crate.transform.position.z - transform.position.z);
		}
	}
	
	#endregion EdgeGrabbing

	#region Accessor Methods
	
	public bool GrabbedCrate() {
		return crate != null;
	}
	
	public Vector3[] getCuboid(){
		return cuboid;
	}
	
	public float getOrientation(){
		return orientation;
	}
	
	public void Teleport(Vector3 newPos){
		transform.position = newPos;
		gameObject.GetComponent<BoundObject>().updateBounds();
	}

	public Vector3 GetVelocity() {
		return velocity;
	}

	public bool isPaused(){
		return _paused;
	}

	public void setCutsceneMode(bool c){
		cutsceneMode = c;
	}
	
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
	
	public float getColliderWidth(){
		return colliderWidth;
	}
	
	public float getColliderHeight(){
		return colliderHeight;
	}
	
	public float getColliderDepth(){
		return colliderDepth;
	}
	
	public bool isClimbing(){
		return climbTimer > 0;
	}
	
	public bool isKicking(){
		return kickTimer > 0;
	}
	
	#endregion Accessor Methods
	
	private void OnPauseGame(bool p){
		if(_paused != p){
			if(p){
				prePauseAnimSpeed = anim.speed;
				anim.speed = 0;
			}else{
				anim.speed = prePauseAnimSpeed;
			}
		}		
		_paused = p;
	}

	public void StartKick(){
	  anim.SetBool("Kick", true);
	  kickTimer = KICK_TIME;
	  velocity.x = 0;
	  velocity.z = 0;
	}
}

public enum EdgeState{
	FAR_FROM_EDGE, CLOSE_TO_EDGE, HANGING
}
