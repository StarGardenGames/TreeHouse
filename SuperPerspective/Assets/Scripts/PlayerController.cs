using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // Movement properties (units/second)
    public float acceleration;
    public float maxSpeed;
    public float gravity;
    public float maxFall;
    public float jump;

    // Layer mask for collidable objects
    private int layerMask;

    // Vector to store the player's velocity
    private Vector3 velocity;  
 
    // Vertical movement flags
    private bool grounded;
    private bool falling;

    // Raycasting variables
    private int horizontalRays = 8;
    private int verticalRays = 4;
    private int margin = 2;

	// Use this for initialization
	void Start () {
        layerMask = LayerMask.NameToLayer("normalCollisions");
	}

    // Used to detect collisions and physics properties
    void FixedUpdate()
    {

    }

    // Used to update player's position after all physics calculations have finished
    void LateUpdate()
    {

    }
}
