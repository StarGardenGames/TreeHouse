
using UnityEngine;
using System.Collections;

public class PlayerController2 : MonoBehaviour {

    // Movement properties (units/second)
    public float acceleration;
    public float decelleration;
    public float maxSpeed;
    public float gravity;
    public float maxFall;
    public float jump;

    // Vertical movement flags
    private bool grounded;
    private bool falling;

    // Horizontal movement variables
    private float xVelocity;
    private float zVelocity;

    void Awake()
    {
        Physics.gravity = new Vector3(0f, -gravity, 0f);
    }

	// Use this for initialization
	void Start () {

	}

    // Used to detect collisions and physics properties
    void FixedUpdate()
    {
        
    }

    // Used to update player's position after all physics calculations have finished
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.velocity += new Vector3(0f, jump, 0f);
            grounded = false;
        }

        // Variables for X and Z axis movement
        float xMove = 0f;
        float zMove = 0f;
        
        // Get X and Z axis movement
        if (Input.GetKey(KeyCode.D))
        {
            xMove += acceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xMove -= acceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            zMove -= acceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            zMove += acceleration * Time.deltaTime;
        }

        // Apply X and Z movement to velocity (not capped at maxSpeed yet)
        xVelocity = rigidbody.velocity.x + xMove;
        zVelocity = rigidbody.velocity.z + zMove;

        // Reduce x or z velocity if keys are not being pressed (or cancel each other out)
        if (xMove == 0) 
        { 
            if(Mathf.Abs(rigidbody.velocity.x) > decelleration * Time.deltaTime)
                xVelocity += Mathf.Sign(rigidbody.velocity.x) * -decelleration * Time.deltaTime;
            else
                xVelocity = 0f;
        }
        if (zMove == 0)
        {
            if (Mathf.Abs(rigidbody.velocity.z) > decelleration * Time.deltaTime)
                zVelocity += Mathf.Sign(rigidbody.velocity.z) * -decelleration * Time.deltaTime;
            else
                zVelocity = 0f;
        }

        if (!grounded)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, Mathf.Max(rigidbody.velocity.y - gravity, -maxFall), rigidbody.velocity.z);
        }
    }

    void LateUpdate()
    {
        // Apply X and Z velocity values
        rigidbody.velocity = new Vector3(xVelocity, rigidbody.velocity.y, zVelocity);

        if (rigidbody.velocity.y < -0.01 && !grounded)
            falling = true;
        if (rigidbody.velocity.y < 0.01 && !falling)
        {
            grounded = true;
        }
    }
}
