using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	#pragma warning disable 219

	private const float gravity = 1.5f;
	private const float terminalVelocity = 60;
	private const float decelleration = 15;

	private Vector3 velocity;
	private Vector3 trajectory;
	private bool grounded;
	private float colliderHeight, colliderWidth, colliderDepth;
	private float Margin = 0.05f;

	private Vector3 startPos;

	private PlayerController player;

	private CollisionChecker colCheck;

	private bool grabbed, respawnFlag = false;

	private PerspectiveType persp = PerspectiveType.p3D;

	void Start() {
		grounded = false;
		velocity = Vector3.zero;
		colliderHeight = GetComponent<Collider>().bounds.size.y;
		colliderWidth = GetComponent<Collider>().bounds.size.x;
		colliderDepth = GetComponent<Collider>().bounds.size.z;

		player = GameObject.Find("Player").GetComponent<PlayerController>();

		// Register CheckGrab to grab input event
		InputManager.instance.InteractPressed += CheckGrab;
		GameStateManager.instance.PerspectiveShiftEvent += Shift;
		CameraController2.instance.ShiftStartEvent += checkBreak;
		colCheck = new CollisionChecker (GetComponent<Collider> ());
		startPos = transform.position;
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
		velocity.x = newVelocityX;
	
		if (velocity.z != 0)
		{
			int modifier = velocity.z > 0 ? -1 : 1;
			newVelocityZ += Mathf.Min(decelleration, Mathf.Abs(velocity.z)) * modifier;
		}
		velocity.z = newVelocityZ;

		if (GetComponent<Collider> ().enabled) {
			colliderHeight = GetComponent<Collider>().bounds.size.y;
			colliderWidth = GetComponent<Collider>().bounds.size.x;
			colliderDepth = GetComponent<Collider>().bounds.size.z;
		}

		CheckCollisions ();
	}

	void LateUpdate () {
		if (grabbed && !PlayerInRange()) {
			player.Grab(null);
			grabbed = false;
		}
		transform.Translate(velocity * Time.deltaTime);
		if (respawnFlag && Vector2.Distance(new Vector2(startPos.x, startPos.y), new Vector2(player.transform.position.x, player.transform.position.y)) > colliderWidth) {
			Vector3 pos = transform.position;
			pos = startPos + Vector3.up;
			transform.position = pos;
			GetComponent<Collider>().enabled = true;
			GetComponentInChildren<Renderer>().enabled = true;
			respawnFlag = false;
		}
	}

	public void CheckCollisions() {
		Vector3 trajectory;
		
		RaycastHit[] hits = colCheck.CheckYCollision (velocity, Margin);
		
		float close = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null)
			{
				if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					if (velocity.y < 0) {
						grounded = true;
					}
					trajectory = velocity.y * Vector3.up;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}
		if (close == -1) {
			grounded = false;
		} else {
			transform.Translate(Vector3.up * Mathf.Sign(velocity.y) * (close - colliderHeight / 2));
			velocity = new Vector3(velocity.x, 0f, velocity.z);
		}
		
		// Third check the player's velocity along the X axis and check for collisions in that direction is non-zero
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		
		hits = colCheck.CheckXCollision (velocity, Margin);
		
		close = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null)
			{
				if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
					trajectory = velocity.x * Vector3.right;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}
		if (close != -1) {
			//transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (close - colliderWidth / 2));
			velocity = new Vector3(0f, velocity.y, velocity.z);
		}
		
		
		// Fourth do the same along the Z axis  
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		hits = colCheck.CheckZCollision (velocity, Margin);
		
		close = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null)
			{
				if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
					trajectory = velocity.z * Vector3.forward;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}
		if (close != -1) {
			//transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (close - colliderDepth / 2));
			velocity = new Vector3(velocity.x, velocity.y, 0f);
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
		float centerZ   = GetComponent<Collider>().bounds.center.z;
		
		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, maxY, centerZ),
			new Vector3(maxX, maxY, centerZ),
			new Vector3(minX, minY, centerZ),
			new Vector3(maxX, minY, centerZ),
			new Vector3(centerX, centerY, centerZ)
		};
		
		//check all startpoints
		for (int i = 0; i < startPoints.Length; i++) {
			connected = connected || Physics.Raycast (startPoints [i], Vector3.forward) || Physics.Raycast (startPoints [i], -Vector3.forward);
		}

		return connected;
	}

	void checkBreak() {
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p2D && Check2DIntersect()) {
			respawnFlag = true;
		}
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
		if (other.GetComponent<PushSwitchOld>() && colliderDim == colliderWidth) {
			transform.Translate(0, 0.1f, 0);
		}
		//Collision w/ PlayerInteractable
		foreach(Interactable c in other.GetComponents<Interactable>()){
			c.EnterCollisionWithGeneral(gameObject);
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

	public void SetVelocity(float x, float z) {
		velocity.x = x;
		velocity.z = z;
	}

	private void Shift(PerspectiveType p) {
		persp = p;
		if (respawnFlag) {
			GetComponent<Collider>().enabled = false;
			GetComponentInChildren<Renderer>().enabled = false;
		}
	}

	private bool PlayerInRange() {
		if (persp == PerspectiveType.p3D)
			return Mathf.Abs(player.transform.position.y - transform.position.y) <= colliderHeight / 2 && Mathf.Abs (player.transform.position.x - transform.position.x) <= colliderWidth / 2 + player.GetComponent<Collider>().bounds.size.x / 2 + Margin * 4 && Mathf.Abs (player.transform.position.z - transform.position.z) <= GetComponent<Collider>().bounds.size.z / 2 + player.GetComponent<Collider>().bounds.size.z / 2 + Margin * 4;
		else
			return Mathf.Abs(player.transform.position.y - transform.position.y) <= colliderHeight / 2 && Mathf.Abs (player.transform.position.x - transform.position.x) <= colliderWidth / 2 + player.GetComponent<Collider>().bounds.size.x / 2 + Margin * 4;
	}
}
