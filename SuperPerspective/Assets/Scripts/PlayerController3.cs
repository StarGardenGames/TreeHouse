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
    private bool falling;
    private bool canJump;
    private bool lastInput;
    private float jumpPressedTime;

    // Raycasting Variables
    public int verticalRays = 8;
    public float Margin;   // Not needed?
    
    private Rect box;
    private float colliderHeight;
    private float colliderWidth;
    private float colliderDepth;

    // Vector used to store new veolicty
    private Vector3 velocity;

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
        colliderHeight = collider.bounds.max.y - collider.bounds.min.y;
        colliderWidth = collider.bounds.max.x - collider.bounds.min.x;
        colliderDepth = collider.bounds.max.z - collider.bounds.min.z;
	}

    void Update()
    {
        // See if the player is pressing the jump button this frame
        bool input = Input.GetButton("Jump");

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
        if (grounded && Time.time - jumpPressedTime < jumpMargin)
        {
            grounded = false;
            velocity = new Vector3(velocity.x, jump, velocity.z);
            jumpPressedTime = 0;
        }

        lastInput = input;
    }

    // Collision detection and velocity calculations are done in the fixed update step
    void FixedUpdate()
    {
        // Used for raycasting
        Vector3 startPoint;     // The starting point of a ray  
        float distance;         // The distance of a ray
        RaycastHit hitInfo;     // Stored the information if a ray registers a hit

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
        float xAxis = Input.GetAxis("Horizontal");
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
        float zAxis = Input.GetAxis("Vertical");
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

        // ------------------------------------------------------------------------------------------------------
        // COLLISION CHECKING
        // ------------------------------------------------------------------------------------------------------

        // First determine if the player is not moving up, if not the check for collisions below
        #region Checking Below
        
        // Check for collisions below the player if he/she is not moving up
        if (grounded || falling)
        {
            // Check each of the four corners and the center of the collider
            // TODO: Store coordinates in an array to do this as a loop

            // True if any ray hits a collider
            bool connected = false;

            // Set the raycast distance to check as far as the player will fall this frame
            distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);

            // Top Left (Min X, Max Z)
            startPoint = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.max.z);
            Ray ray = new Ray(startPoint, Vector3.down);
            connected = Physics.Raycast(ray, out hitInfo, distance);

            // Top Right (Max X, Max Z)
            if (!connected)
            {
                startPoint = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.max.z);
                ray = new Ray(startPoint, Vector3.down);
                connected = Physics.Raycast(ray, out hitInfo, distance);
            }

            // Bottom Left (Min X, Min Z)
            if (!connected)
            {
                startPoint = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.min.z);
                ray = new Ray(startPoint, Vector3.down);
                connected = Physics.Raycast(ray, out hitInfo, distance);
            }

            // Bottom Right (Max X, Min Z)
            if (!connected)
            {
                startPoint = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.min.z);
                ray = new Ray(startPoint, Vector3.down);
                connected = Physics.Raycast(ray, out hitInfo, distance);
            }

            // If any rays connected move the player and set grounded to true since we're now on the ground
            if (connected)
            {
                grounded = true;
                falling = false;
                canJump = true;
                transform.Translate(Vector3.down * (hitInfo.distance - colliderHeight / 2));
                velocity = new Vector3(velocity.x, 0f, velocity.z);
            }

            // Otherwise we're not grounded (temporary?)
            else
                grounded = false;
        }

        #endregion Checking Below

        // Second check above if the player is moving upwards
        #region Checking Above

        // This means we're moving up in the air
        else if (!grounded && !falling)
        {
            // True if any ray hits a collider
            bool connected = false;

            // Set the raycast distance to check as far as the player will move up this frame
            distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);

            // Top Left (Min X, Max Z)
            startPoint = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.max.z);
            Ray ray = new Ray(startPoint, Vector3.up);
            connected = Physics.Raycast(ray, out hitInfo, distance);

            // Top Right (Max X, Max Z)
            if (!connected)
            {
                startPoint = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.max.z);
                ray = new Ray(startPoint, Vector3.up);
                connected = Physics.Raycast(ray, out hitInfo, distance);
            }

            // Bottom Left (Min X, Min Z)
            if (!connected)
            {
                startPoint = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.min.z);
                ray = new Ray(startPoint, Vector3.up);
                connected = Physics.Raycast(ray, out hitInfo, distance);
            }

            // Bottom Right (Max X, Min Z)
            if (!connected)
            {
                startPoint = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.max.z);
                ray = new Ray(startPoint, Vector3.up);
                connected = Physics.Raycast(ray, out hitInfo, distance);
            }

            // If any rays connected move the player and set falling to true since we hit something
            if (connected)
            {
                grounded = false;
                falling = true;
                transform.Translate(Vector3.up * (hitInfo.distance - colliderHeight / 2));
                velocity = new Vector3(velocity.x, 0f, velocity.z);
            }
        }

        #endregion Checking Above

        // Third check the player's velocity along the X axis and check for collisions in that direction is non-zero
        #region Checking X Axis

        // TODO: Write final movement and collision code
        // NOTE: This is temporary movement code with no collision detection


        #endregion Checking X Axis

        // Fourth do the same along the Z axis  
        #region Checking Z Axis

        // TODO: Write final movement and collision code
        // NOTE: This is temporary movement code with no collision detection
        

        #endregion Checking Z Axis

    }

	// LateUpdate is used to actually move the position of the player
	void LateUpdate () {
        transform.Translate(velocity * Time.deltaTime);
    }

    #endregion

}
