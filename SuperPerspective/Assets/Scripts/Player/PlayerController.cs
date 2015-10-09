using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController : PhysicalObject
{	
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
	public float jump;
	public float jumpMargin;
	private bool _paused;

	// Layer mask used for collision checks
	private int layerMask;  // Not needed?

	// Verticle movement flags
	private bool grounded;
	public bool falling;
	private bool canJump;
	private bool lastInput;
	private bool bounced;
	private float jumpPressedTime;

	// Raycasting Variables
	public int verticalRays = 8;
	float Margin = 0.05f;
	float verticalOverlapThreshhold = .3f;

	private Rect box;
	public float colliderHeight;
	public float colliderWidth;
	public float colliderDepth;

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
	private int launched;

	//direction of player
	private float orientation = 0;

	//Vars for edge grabbing
	private Vector3[] cuboid;
	public Edge grabbedEdge = null;
	public byte edgeState = 0;//0: not near an edge, 1: close to an edge, 2:hanging
	bool climbing = false;

    private int kicking;
	 private const int KICK_TIME = 10;
	private float lastUpdate;
	
	bool cutsceneMode = false;

	//Emitter
	public ParticleSystem dustEmitter;
	//public ParticleSystem landingEmitter;
	AnimatorStateInfo currentState;


   #endregion
	
	//setup singleton
	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance!= this)
			Destroy(this);
	}
	
    // Initialization
	void Start () {
		base.Init();
		// Set player as falling 
		// TODO: Since the player often falls through the ground we should cast a ray down and immediately place then on the ground.
		grounded = false;
		falling = true;
		canJump = false;
		lastInput = false;
		dustEmitter.enableEmission = false;
		//landingEmitter.enableEmission = false;
		// Initialize the layer mask
		layerMask = LayerMask.NameToLayer("normalCollisions");

		// Get the collider dimensions to use for raycasting
		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;

		anim = GetComponentInChildren<Animator>();
		model = anim.gameObject;

		//cuboid
		cuboid = new Vector3[2];

		colCheck = new CollisionChecker(GetComponent<Collider> ());

		// Register event handlers
		GameStateManager.instance.GamePausedEvent += OnPauseGame;

		lastUpdate = Time.fixedTime;
	}

	void Update(){
		if (canMove()){
			// See if the player is pressing the jump button this frame
			bool input = InputManager.instance.JumpStatus();

			// If this is the first frame the button was pressed then store the current time
			if (input && !lastInput)
			{
			jumpPressedTime = Time.time;
			}
			// If the player didn't press the jump key this frame store time as 0
			else if (!input)
			{
			jumpPressedTime = 0;
			}

			// This only runs while the player is grounded, it allows the player to jump as soon as they land 
			//      even if they pressed the jump button while in midair if they were close enough to the ground
			if ((grounded || edgeState == 2) && Time.time - jumpPressedTime < jumpMargin && crate == null)
			{
			anim.SetTrigger("Jump");
			grounded = false;
			velocity = new Vector3(velocity.x, jump, velocity.z);
			jumpPressedTime = 0;
			ReleaseEdge();
			//update edgeState for animation
			anim.SetInteger("EdgeState", 6);
			}

			lastInput = input;

			anim.SetBool("Falling", falling);
			anim.SetBool("Grounded", grounded);


		}else{
			if(anim.speed != 0){
				
			}
		}
	}

    // Collision detection and velocity calculations are done in the fixed update step
    void FixedUpdate()
    {
        if (kicking > 0)
            kicking--;
        if (canMove() && kicking == 0)
        {
				//update climbing variable
				climbing = anim.GetCurrentAnimatorStateInfo(0).IsName("HangUp");
			  
            //update cuboid for edges
				Vector3 halfScale = gameObject.transform.lossyScale * .5f;
				cuboid[0] = gameObject.transform.position - halfScale;
				cuboid[1] = gameObject.transform.position + halfScale;

            // ------------------------------------------------------------------------------------------------------
            // X-AXIS MOVEMENT VELOCITY CALCULATIONS
            // ------------------------------------------------------------------------------------------------------
            float newVelocityX = velocity.x;
            //if we're either not on an edge
            if (edgeState != 2 && !climbing)
            {
                float xAxis = InputManager.instance.GetForwardMovement();
                if (xAxis != 0 && launched == 0)
                {
                    newVelocityX += acceleration * Mathf.Sign(xAxis);
                    newVelocityX = Mathf.Clamp(newVelocityX, -maxSpeed * Mathf.Abs(xAxis), maxSpeed * Mathf.Abs(xAxis));
                }
                else if (velocity.x != 0 && launched == 0)
                {
                    int modifier = velocity.x > 0 ? -1 : 1;
                    newVelocityX += Mathf.Min(decelleration, Mathf.Abs(velocity.x)) * modifier;
                }
            }
				//shimmy
				else if(grabbedEdge!=null && !climbing && grabbedEdge.getOrientation() % 2 == 1){
					//adjust velocity
					float xAxis = InputManager.instance.GetForwardMovement();
					if(xAxis == 0)
						newVelocityX = 0;
					else
						newVelocityX = hangMaxSpeed * Mathf.Sign(xAxis);
					
					//check if edge is still valid
					Vector3 pos = transform.position;
					pos.x += newVelocityX/50f;
					if(grabbedEdge.isPositionValidOnEdge(pos)){					
						//clamp motion
						float newX = gameObject.transform.position.x;
						float edgeX = grabbedEdge.gameObject.transform.position.x;
						float edgeScale = grabbedEdge.gameObject.transform.lossyScale.x;
						float minBound = edgeX - edgeScale * .5f + colliderWidth * .5f;
						float maxBound = edgeX + edgeScale * .5f - colliderWidth * .5f;
						if (!(minBound <= newX && newX <= maxBound))
						{
							newX = Mathf.Clamp(newX, minBound, maxBound);
							newVelocityX = 0;
							pos = gameObject.transform.position;
							pos.x = newX;
							gameObject.transform.position = pos;
						}
						//update edgeState for animation
						if(newVelocityX!=0 && anim.GetInteger("EdgeState") < 3){
							anim.SetInteger("EdgeState", 3);
						}
					}else{
						newVelocityX = 0;
					}
				}
            else
            {
                //if we're latched to a wall which doesn't allow x axis movement don't move along x axis
                newVelocityX = 0;
            }

            velocity.x = newVelocityX;

			//if (launched > 0)
				//launched--;

            // ------------------------------------------------------------------------------------------------------
            // Z-AXIS MOVEMENT VELOCITY CALCULATIONS
            // ------------------------------------------------------------------------------------------------------
            float newVelocityZ = velocity.z;
            //if we're either not on an edge or on an edge which allows z movement
            if (edgeState != 2 && !climbing)
            {
                float zAxis = -InputManager.instance.GetSideMovement();

                if (zAxis != 0 && launched == 0)
                {
                    newVelocityZ += acceleration * Mathf.Sign(zAxis);
                    newVelocityZ = Mathf.Clamp(newVelocityZ, -maxSpeed * Mathf.Abs(zAxis), maxSpeed * Mathf.Abs(zAxis));
                }
				else if (velocity.z != 0 && launched == 0)
                {
                    int modifier = velocity.z > 0 ? -1 : 1;
                    newVelocityZ += Mathf.Min(decelleration, Mathf.Abs(velocity.z)) * modifier;
                }
            }else if(!climbing && grabbedEdge.getOrientation() % 2 == 0){
					float zAxis = -InputManager.instance.GetSideMovement();
					if(zAxis == 0)
						newVelocityZ = 0f;
					else
						newVelocityZ = hangMaxSpeed * Mathf.Sign(zAxis);
					
					//check if new position is valid
					Vector3 pos = transform.position;
					pos.z += newVelocityZ/50f;
					if(grabbedEdge.isPositionValidOnEdge(pos)){
						//clamp position
						float newZ = gameObject.transform.position.z + (newVelocityZ/50f);
						float edgeZ = grabbedEdge.gameObject.transform.position.z;
						float edgeScale = grabbedEdge.gameObject.transform.lossyScale.z;
						float minBound = edgeZ - edgeScale * .5f + colliderDepth * .5f;
						float maxBound = edgeZ + edgeScale * .5f - colliderDepth * .5f;
						if (!(minBound <= newZ && newZ <= maxBound))
						{
							newZ = Mathf.Clamp(newZ, minBound, maxBound);
							newVelocityZ = 0;
							pos = gameObject.transform.position;
							pos.z = newZ;
							gameObject.transform.position = pos;
						}
						
						//update edgeState for animation
						if(newVelocityZ!=0 && anim.GetInteger("EdgeState") < 3){
							anim.SetInteger("EdgeState", 3);
						}
					}else{
						newVelocityZ = 0;
					}
				}
            else{
                //if we're latched to a wall which doesn't allow z movement
                newVelocityZ = 0;
            }

            velocity.z = newVelocityZ;

            bool walking = edgeState != 2 && (Mathf.Abs(velocity.z) > 0.1 || Mathf.Abs(velocity.x) >= 0.1);
			bool shimmying = edgeState == 2 && ( Mathf.Abs(velocity.z) > 0.1 || Mathf.Abs(velocity.x) >= 0.1 );
            bool running = (Mathf.Abs(velocity.z) >= maxSpeed / 2 || Mathf.Abs(velocity.x) >= maxSpeed / 2);
            anim.SetBool("Walking", walking && !running && crate == null);
            anim.SetBool("Running", running && crate == null);
				
			if((running || walking) && grounded){dustEmitter.enableEmission =true;}
			else{dustEmitter.enableEmission =false;}
				if (crate == null) {
					if (!pushFlag)
						anim.SetBool("Pushing", false);
					anim.SetBool("Pulling", false);
					anim.SetBool("CrateIdle", false);
				} else {
					float epsilon = .1f;
					float crateVel = Vector3.Dot(velocity, grabAxis);
					anim.SetBool("Pushing", crateVel > epsilon);
					anim.SetBool("Pulling", crateVel < -epsilon);
					anim.SetBool("CrateIdle", -epsilon <= crateVel && crateVel <= epsilon);
				}
				// ------------------------------------------------------------------------------------------------------
            // Update Orientation
            // ------------------------------------------------------------------------------------------------------
				if (walking && crate == null && edgeState < 2 && anim.GetInteger("EdgeState") == 0)
				{
					orientation = Mathf.Rad2Deg * Mathf.Atan2(-velocity.z, velocity.x) + 90;
				}else if(edgeState >= 2){
					orientation = (-1 - grabbedEdge.getOrientation()) * 90;
				}
				model.transform.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
			
				// ------------------------------------------------------------------------------------------------------
            // MANAGE EDGE STATE
            // ------------------------------------------------------------------------------------------------------
				int animEdgeState = anim.GetInteger("EdgeState");
				if(animEdgeState < 3 || (animEdgeState == 5 && !climbing))	
					anim.SetInteger("EdgeState", edgeState);
				else if(animEdgeState == 3){
						if(Mathf.Abs(velocity.z) < 0.1 && Mathf.Abs(velocity.x) < 0.1)
							anim.SetInteger("EdgeState", edgeState);
				}else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("HangBegin") && !anim.GetCurrentAnimatorStateInfo(0).IsName("HangShimmying")){
					anim.SetInteger("EdgeState", edgeState);
				}
			}
    }

	#region Collisions
	 
	public void CheckCollisions(){
		CheckCollisionsOnAxis('Y');
		pushFlag = false;
		CheckCollisionsOnAxis('X');
		CheckCollisionsOnAxis('Z');
	}
	
	void CheckCollisionsOnAxis(char axis){
		int axisIndex = getAxisIndex(axis);
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
				if(axis != 'Y' && !significantVerticalOverlap){
					transform.Translate(new Vector3(0f,verticalOverlap,0f));
					continue;
				}
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity[axisIndex] * axisVector;
					CollideWithObject(hitInfo, trajectory);
				} else if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					if(axis == 'Y'){
						if (velocity.y < 0) {
							grounded = true;
							falling = false;
							canJump = true;
							launched = 0;
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
							Mathf.Sign(velocity[axisIndex]) * 
							(hitInfo.distance - getDimensionAlongAxis(axis) / 2)
						);
					}
					trajectory = velocity[axisIndex] * axisVector;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}
		
		if (close != -1) {
			if(axis == 'Y'){
				if (!bounced) {
					transform.Translate(
						axisVector * 
						Mathf.Sign(velocity[axisIndex]) * 
						(close - getDimensionAlongAxis(axis) / 2)
					);
					velocity[axisIndex] = 0f;
				} else {
					bounced = false;
				}
			}else{
				if(!pushFlag) velocity[axisIndex] = 0f;
			}
		} else if(axis == 'Y'){
			grounded = false;
		}
	}
	
	//Convenience Methods for Axis calculations
	private int getAxisIndex(char axis){
		switch(axis){
			case 'X': return 0;
			case 'Y': return 1;
			case 'Z': return 2;
			default:
				throw new System.ArgumentException("Invalid Axis Character");
		}
	}

	public void Reset() {
		base.Init();
		// Set player as falling 
		// TODO: Since the player often falls through the ground we should cast a ray down and immediately place then on the ground.
		grounded = false;
		falling = true;
		canJump = false;
		lastInput = false;
		
		// Initialize the layer mask
		layerMask = LayerMask.NameToLayer("normalCollisions");
		
		// Get the collider dimensions to use for raycasting
		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;
		
		anim = GetComponentInChildren<Animator>();
		model = anim.gameObject;
		
		//cuboid
		cuboid = new Vector3[2];
		
		colCheck = new CollisionChecker(GetComponent<Collider> ());

		verticalRays = 8;
		Margin = 0.05f;
		zlock = int.MinValue;

		// Register event handlers
		//GameStateManager.instance.GamePausedEvent += OnPauseGame;

		_paused = false;
	}

	
	Vector3 getAxisVector(char axis){
		switch(axis){
			case 'X': return Vector3.right;
			case 'Y': return Vector3.up;
			case 'Z': return Vector3.forward;
			default:
				throw new System.ArgumentException("Invalid Axis Character");
		}
	}
	
	float getDimensionAlongAxis(char axis){
		switch(axis){
			case 'X': return colliderWidth;
			case 'Y': return colliderHeight;
			case 'Z': return colliderDepth;
			default:
				throw new System.ArgumentException("Invalid Axis Character");
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
					launched = 50;
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
		float timeDiff = Time.fixedTime - lastUpdate;
			if (canMove()) {
				// ------------------------------------------------------------------------------------------------------
				// VERTICAL MOVEMENT VELOCITY CALCULATIONS
				// ------------------------------------------------------------------------------------------------------
				if (edgeState != 2 && !climbing)
				{
					// Apply Gravity
					if (!grounded) {
						if (velocity.y <= 0)
							velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - upGravity * Time.deltaTime, -terminalVelocity), velocity.z);
						else
							velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - downGravity * Time.deltaTime, -terminalVelocity), velocity.z);
					}
					
					// Determine if the player is falling
					if (velocity.y < 0f && edgeState == 0)
						falling = true;
				}
				else
				{
					//if holding on to edge set velocity to 0 and set falling to false
					velocity.y = 0;
					falling = false;
				}
			}
			timeDiff -= 1 / 50f;

		CheckCollisions();

        if (canMove())
        {
            if (crate != null)
            {
				Vector3 drag = Vector3.Dot(velocity, grabAxis) * grabAxis * 0.75f;
				crate.SetVelocity(drag.x, drag.z);
                transform.Translate(drag * Time.deltaTime);
            }
            else
            {
                transform.Translate(velocity * Time.deltaTime);
            }
        }
		lastUpdate = Time.fixedTime;
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
		edgeState = 0;
	}

	public bool isFalling(){
		return falling;
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
						this.edgeState = 0;
						grabbedEdge =null;
					}
				}
				//adjust animation state
				if(animState!= -1){
					anim.SetInteger("EdgeState", animState);
				}
				break;
			case 1:
				this.edgeState = 1;
				grabbedEdge = e;
				break;
			case 2:
				this.edgeState	 = 2;
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

    public void StartKick()
    {
        anim.SetTrigger("Kick");
        kicking = KICK_TIME;
        velocity.x = 0;
        velocity.z = 0;
    }

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
		return !_paused && !cutsceneMode;
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
}
