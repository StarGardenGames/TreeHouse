using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	private const float gravity = 1.5f;
	private const float terminalVelocity = 60;
	private const float decelleration = 15;

	private Vector3 velocity;
	private Vector3 trajectory;
	private bool grounded;
	private float colliderHeight, colliderWidth, colliderDepth;
	private float Margin = 0.05f;

	private PlayerController3 player;

	private CollisionChecker colCheck;

	private bool grabbed;

	private PerspectiveType persp = PerspectiveType.p3D;

	void Start() {
		grounded = false;
		velocity = Vector3.zero;
		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;

		player = GameObject.Find("NewPlayer").GetComponent<PlayerController3>();

		// Register CheckGrab to grab input event
		InputManager.instance.InteractPressed += CheckGrab;
		GameStateManager.instance.PerspectiveShiftEvent += Shift;
		colCheck = new CollisionChecker (GetComponent<Collider> ());
	}

	void FixedUpdate() {
		if (!grounded)
			velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - gravity, -terminalVelocity), velocity.z);

		float newVelocityX = velocity.x, newVelocityZ = velocity.z;
		if (velocity.x != 0)
		{
			int modifier = velocity.x > 0 ? -1 : 1;
			newVelocityX += Mathf.Min(decelleration, Mathf.Abs(velocity.x)) * modifier;
		}

		if (velocity.z != 0)
		{
			int modifier = velocity.z > 0 ? -1 : 1;
			newVelocityZ += Mathf.Min(decelleration, Mathf.Abs(velocity.z)) * modifier;
		}

		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;

		CheckCollisions ();
	}

	void LateUpdate () {
		if (grabbed && !PlayerInRange()) {
			player.Grab(null);
			grabbed = false;
		}
		transform.Translate(velocity * Time.deltaTime);
	}

	public void CheckCollisions() {
		#region Checking Below

		// If any rays connected move the player and set grounded to true since we're now on the ground
		RaycastHit hitInfo = colCheck.CheckYCollision (velocity, Margin);

		if (hitInfo.collider != null)
		{
			if (velocity.y < 0) {
				grounded = true;
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

		// If any rays connected move the player and set grounded to true since we're now on the ground
		hitInfo = colCheck.CheckXCollision (velocity, Margin);
		
		if (hitInfo.collider != null)
		{
			trajectory = velocity.x * Vector3.right;
			transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
			velocity = new Vector3(0f, velocity.y, velocity.z);
			CollideWithObject(hitInfo, trajectory);
		}
		
		#endregion Checking X Axis
		
		// Fourth do the same along the Z axis  
		#region Checking Z Axis

		// If any rays connected move the player and set grounded to true since we're now on the ground
		hitInfo = colCheck.CheckZCollision (velocity, Margin);
		
		if (hitInfo.collider != null)
		{
			trajectory = velocity.z * Vector3.forward;
			transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
			velocity = new Vector3(velocity.x, velocity.y, 0f);
			CollideWithObject(hitInfo, trajectory);
		}
		
		#endregion Checking Z Axis
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
		//Collision w/ PlayerInteractable
		foreach(PlayerInteractable c in other.GetComponents<PlayerInteractable>()){
			c.CollisionWithPlayer();
		}
	}

	public void CheckGrab() {
		if (!grabbed && PlayerInRange()) {
			player.Grab(this);
			grabbed = true;
		} else if (grabbed) {
			player.Grab(null);
			grabbed = false;
		}
	}

	public void SetVelocity(Vector3 velocity) {
		this.velocity = velocity;
	}

	private void Shift(PerspectiveType p) {
		persp = p;
	}

	private bool PlayerInRange() {
		if (persp == PerspectiveType.p3D)
			return Mathf.Abs(player.transform.position.y - transform.position.y) <= colliderHeight / 2 && Mathf.Abs (player.transform.position.x - transform.position.x) <= colliderWidth / 2 + player.GetComponent<Collider>().bounds.size.x / 2 + Margin && Mathf.Abs (player.transform.position.z - transform.position.z) <= GetComponent<Collider>().bounds.size.z / 2 + player.GetComponent<Collider>().bounds.size.z / 2 + Margin;
		else
			return Mathf.Abs(player.transform.position.y - transform.position.y) <= colliderHeight / 2 && Mathf.Abs (player.transform.position.x - transform.position.x) <= colliderWidth / 2 + player.GetComponent<Collider>().bounds.size.x / 2 + Margin;
	}
}
