using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController3 : MonoBehaviour
{

    #region Properties & Variables

    // Movement parameters
    public float acceleration;
    public float decelleration;
    public float maxSpeed;
    public float gravity;
    public float terminalVelocity;
    public float jump;
    public float jumpMargin;

    // Layer mask used for collision checks
    private int layerMask;  // Not needed?

    // Verticle movement flags
    private bool grounded;
    public bool falling;
    private bool canJump;
    private bool lastInput;
    private float jumpPressedTime;

    // Raycasting Variables
    public int verticalRays = 8;
    float Margin = 0.05f;
    
    private Rect box;
    private float colliderHeight;
    private float colliderWidth;
    private float colliderDepth;

    // Vector used to store new veolicty
    private Vector3 velocity;

	// Vars for Z-locking
	private float zlock = int.MinValue;
	private bool zlockFlag;

	private PerspectiveType persp = PerspectiveType.p3D;

	private Animator anim;
	private GameObject model;

	private Crate crate = null;
	private Vector3 grabAxis = Vector3.zero;
    #endregion

    #region MonoBehavior Implementation

    // Initialization
	void Start () {

        // Set player as falling (they should immediately register a hit)
        grounded = false;
        falling = true;
        canJump = false;
        lastInput = false;

        // Initialize the layer mask
        layerMask = LayerMask.NameToLayer("normalCollisions");
        
        // Set the starting velocity to the zero vector
        velocity = Vector3.zero;

        // Get the collider dimensions to use for raycasting
        colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
        colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
        colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;

		//Register Flip method to the shift event
		GameStateManager.instance.PerspectiveShiftEvent += Flip;

		anim = GetComponentInChildren<Animator>();
		model = anim.gameObject;
	}

    void Update()
    {

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
        if (grounded && Time.time - jumpPressedTime < jumpMargin && crate == null)
        {
			anim.SetTrigger("Jump");
            grounded = false;
            velocity = new Vector3(velocity.x, jump, velocity.z);
            jumpPressedTime = 0;
        }

        lastInput = input;

		anim.SetBool("Falling", falling);
		anim.SetBool("Grounded", velocity.y == 0);
    }

    // Collision detection and velocity calculations are done in the fixed update step
    void FixedUpdate()
    {

        

		if (zlockFlag) {
			DoZLock();
			zlockFlag = false;
		}

        // ------------------------------------------------------------------------------------------------------
        // VERTICAL MOVEMENT VELOCITY CALCULATIONS
        // ------------------------------------------------------------------------------------------------------

        // Apply Gravity
		if (!grounded)
			velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - gravity, -terminalVelocity), velocity.z);

        // Determine if the player is falling
        if (velocity.y < 0f)
            falling = true;

        // ------------------------------------------------------------------------------------------------------
        // X-AXIS MOVEMENT VELOCITY CALCULATIONS
        // ------------------------------------------------------------------------------------------------------
        float xAxis = InputManager.instance.GetForwardMovement();
        float newVelocityX = velocity.x;
        if (xAxis != 0)
        {
            newVelocityX += acceleration * Mathf.Sign(xAxis);
            newVelocityX = Mathf.Clamp(newVelocityX, -maxSpeed * Mathf.Abs(xAxis), maxSpeed * Mathf.Abs(xAxis));
        }
        else if (velocity.x != 0)
        {
            int modifier = velocity.x > 0 ? -1 : 1;
            newVelocityX += Mathf.Min(decelleration, Mathf.Abs(velocity.x)) * modifier;
        }

        velocity.x = newVelocityX;

        // ------------------------------------------------------------------------------------------------------
        // Z-AXIS MOVEMENT VELOCITY CALCULATIONS
        // ------------------------------------------------------------------------------------------------------
        float zAxis = -InputManager.instance.GetSideMovement();
        float newVelocityZ = velocity.z;
        if (zAxis != 0)
        {
            newVelocityZ += acceleration * Mathf.Sign(zAxis);
            newVelocityZ = Mathf.Clamp(newVelocityZ, -maxSpeed * Mathf.Abs(zAxis), maxSpeed * Mathf.Abs(zAxis));
        }
        else if (velocity.z != 0)
        {
            int modifier = velocity.z > 0 ? -1 : 1;
            newVelocityZ += Mathf.Min(decelleration, Mathf.Abs(velocity.z)) * modifier;
        }

        velocity.z = newVelocityZ;

		bool walking = (Mathf.Abs(velocity.z) > 0.1 || Mathf.Abs(velocity.x) >= 0.1);
		bool running = (Mathf.Abs(velocity.z) >= maxSpeed / 2 || Mathf.Abs(velocity.x) >= maxSpeed / 2);
		anim.SetBool("Walking", walking && !running);
		anim.SetBool("Running", running);
		anim.SetBool("Pushing", false);
		anim.SetBool("Pulling", walking && crate != null);
		if (walking) {
			if (crate == null)
				model.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(-velocity.z, velocity.x) + 90, Vector3.up);
		}

		// ------------------------------------------------------------------------------------------------------
		// COLLISION CHECKING
		// ------------------------------------------------------------------------------------------------------
		CheckCollisions();
	
    }

	public void CheckCollisions(){


		// Used for raycasting
		float distance;         // The distance of a ray
		RaycastHit hitInfo;     // Stored the information if a ray registers a hit
		
		//reference variables
		float minX 		= GetComponent<Collider>().bounds.min.x + Margin;
		float centerX 	= GetComponent<Collider>().bounds.center.x;
		float maxX 		= GetComponent<Collider>().bounds.max.x - Margin;
		float minY 		= GetComponent<Collider>().bounds.min.y + Margin;
		float centerY 	= GetComponent<Collider>().bounds.center.x;
		float maxY 		= GetComponent<Collider>().bounds.max.y - Margin;
		float minZ 		= GetComponent<Collider>().bounds.min.z + Margin;
		float centerZ	= GetComponent<Collider>().bounds.center.z;
		float maxZ 		= GetComponent<Collider>().bounds.max.z - Margin;
		
		// First determine if the player is not moving up, if not the check for collisions below
		#region Checking Below
		bool connected = false;
		Vector3 trajectory;
		
		// Check for collisions below the player if he/she is not moving up
		//if (grounded || falling)
		//{
		// Check each of the four corners and the center of the collider
		// TODO: Store coordinates in an array to do this as a loop
		
		// True if any ray hits a collider
		//bool connected = false;
		
		// Set the raycast distance to check as far as the player will fall this frame
		distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);
		
		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, centerY, maxZ),
			new Vector3(maxX, centerY, maxZ),
			new Vector3(minX, centerY, minZ),
			new Vector3(maxX, centerY, minZ),
			new Vector3(centerX, centerY, centerZ)
		};
		
		//test all startpoints
		for(int i = 0; i < startPoints.Length; i++)
			connected = connected || isConnected(startPoints[i], 1, hitInfo, distance);
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		if (connected)
		{
			if (velocity.y < 0) {
				grounded = true;
				falling = false;
				canJump = true;
				// Z-lock
				if (hitInfo.collider.gameObject.GetComponent<LevelGeometry>())
					zlock = hitInfo.transform.position.z;
				else
					zlock = int.MinValue;
			}
			
			trajectory = velocity.y * Vector3.up;
			transform.Translate(Vector3.up * Mathf.Sign(velocity.y) * (hitInfo.distance - colliderHeight / 2));
			velocity = new Vector3(velocity.x, 0f, velocity.z);
			CollideWithObject(hitInfo, trajectory);
		}
		
		// Otherwise we're not grounded (temporary?)
		else
			grounded = false;
		//}
		
		#endregion Checking Below
		
		// Third check the player's velocity along the X axis and check for collisions in that direction is non-zero
		#region Checking X Axis
		
		// TODO: Write final movement and collision code
		// NOTE: This is temporary movement code with no collision detection
		
		// True if any ray hits a collider
		connected = false;
		
		// Set the raycast distance to check as far as the player will move this frame
		distance = (colliderWidth / 2) + Mathf.Abs(velocity.x * Time.deltaTime);
		
		//setup startPoints, note /**/ means margin wasn't applied previously
		startPoints[0] = new Vector3(centerX, minY, maxZ/**/);
		startPoints[1] = new Vector3(centerX, maxY/**/, maxZ/**/);
		startPoints[2] = new Vector3(centerX, minY, minZ/**/);
		startPoints[3] = new Vector3(centerX, maxY, minZ);
		startPoints[4] = new Vector3(centerX, centerY, centerZ);
		
		//test all startpoints
		for(int i = 0; i < startPoints.Length; i++)
			connected = connected || isConnected(startPoints[i], 0, hitInfo, distance);
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		if (connected)
		{
			trajectory = velocity.x * Vector3.right;
			transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
			velocity = new Vector3(0f, velocity.y, velocity.z);
			CollideWithObject(hitInfo, trajectory);
		}
		
		#endregion Checking X Axis
		
		// Fourth do the same along the Z axis  
		#region Checking Z Axis
		
		// TODO: Write final movement and collision code
		// NOTE: This is temporary movement code with no collision detection
		
		// True if any ray hits a collider
		connected = false;
		
		// Set the raycast distance to check as far as the player will move this frame
		distance = (colliderDepth / 2 + Mathf.Abs(velocity.z * Time.deltaTime));
		
		//setup startPoints arary
		startPoints[0] = new Vector3(minX, maxY, centerZ);
		startPoints[1] = new Vector3(maxX, maxY, centerZ);
		startPoints[2] = new Vector3(minX, minY, centerZ);
		startPoints[3] = new Vector3(maxX, minY, centerZ);
		startPoints[4] = new Vector3(centerX, centerY, centerZ);
		
		//loop through and check all startpoints
		for(int i = 0; i < startPoints.Length; i++)
			connected = connected || isConnected(startPoints[i], 2, hitInfo, distance);
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		if (connected)
		{
			trajectory = velocity.z * Vector3.forward;
			transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
			velocity = new Vector3(velocity.x, velocity.y, 0f);
			CollideWithObject(hitInfo, trajectory);
		}
		
		#endregion Checking Z Axis
	}
	
	public bool isConnected(Vector3 startPoint, int axis, RaycastHit hitInfo, float distance){
		//determine direction
		Vector3 dir = Vector3.zero;
		dir[axis] = Mathf.Sign(velocity[axis]);
		if(hitInfo == null)
			dir[axis] = Mathf.Abs(dir[axis]);
		//compute ray
		Ray ray = new Ray(startPoint, dir);
		//return output
		if(hitInfo == null)
			return Physics.Raycast(ray);
		else
			return Physics.Raycast(ray, out hitInfo, distance);
	}

	// LateUpdate is used to actually move the position of the player
	void LateUpdate () {
		if (crate != null) {
			crate.transform.Translate(Vector3.Dot (velocity, grabAxis) * grabAxis * 0.5f * Time.deltaTime);
			transform.Translate(velocity * 0.5f * Time.deltaTime);
		} else {
			transform.Translate(velocity * Time.deltaTime);
		}
    }

	#endregion

	private void DoZLock() {
		if (zlock > int.MinValue) {
			Vector3 pos = transform.position;
			pos.z = zlock;
			transform.position = pos;
		}
	}

	private bool Check2DIntersect() {
		// True if any ray hits a collider
		bool connected = false;

		GameObject grnd = GameObject.Find("Ground");
		float gz = grnd.transform.lossyScale.z;

		//reference variables
		float minX 		= GetComponent<Collider>().bounds.min.x + Margin;
		float centerX 	= GetComponent<Collider>().bounds.center.x;
		float maxX 		= GetComponent<Collider>().bounds.max.x - Margin;
		float minY 		= GetComponent<Collider>().bounds.min.y + Margin;
		float centerY 	= GetComponent<Collider>().bounds.center.x;
		float maxY 		= GetComponent<Collider>().bounds.max.y - Margin;
		float centerZ	= GetComponent<Collider>().bounds.center.z - gz/2;

		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, maxY, centerZ),
			new Vector3(maxX, maxY, centerZ),
			new Vector3(minX, minY, centerZ),
			new Vector3(maxX, minY, centerZ),
			new Vector3(centerX, centerY, centerZ)
		};

		//check all startpoints
		for(int i = 0; i < startPoints.Length; i++)
			connected = connected || isConnected(startPoints[i], 2, null, 0);
		
		return connected;
	}

	// Used to check collisions with special objects
	// Make this more object oriented? Collidable interface?
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
		if (trajectory.normalized == Vector3.down && other.GetComponent<BouncePad>()) {
			velocity.y += other.GetComponent<BouncePad>().GetBouncePower();
			anim.SetTrigger("Jump");
		}
		// Crate
		if (trajectory.normalized != Vector3.down && other.GetComponent<Crate>()) {
			other.transform.Translate(trajectory * 0.5f * Time.deltaTime);
			//transform.Translate(trajectory * Time.deltaTime);
			velocity += trajectory * 0.5f;
			anim.SetBool("Pushing", true);
		} else {
			anim.SetBool("Pushing", false);
		}
  		//Collision w/ PlayerInteractable
		foreach(PlayerInteractable c in other.GetComponents<PlayerInteractable>()){
			c.CollisionWithPlayer();
		}
	}

	public void Flip(PerspectiveType persp) {
		if (persp == PerspectiveType.p3D)
			zlockFlag = true;
		else if (Check2DIntersect())
			InputManager.instance.SetFailFlag();
		this.persp = persp;
	}

	public void Grab(Crate crate) {
		this.crate = crate;
		if (crate == null)
			return;
		if (persp == PerspectiveType.p2D || Mathf.Abs (crate.transform.position.x - transform.position.x) > Mathf.Abs (crate.transform.position.z - transform.position.z)) {
			grabAxis = Vector3.right;
		} else {
			grabAxis = Vector3.forward;
		}
	}
}
