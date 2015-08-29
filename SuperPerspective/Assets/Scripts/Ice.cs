using UnityEngine;
using System.Collections;

public class Ice : ActiveInteractable {

	#pragma warning disable 219

	private const float gravity = 1.5f;
	private const float terminalVelocity = 60;
	private const float decelleration = 15;
	
	private Vector3 trajectory, newVelocity;
	private bool grounded, svFlag;
	private float colliderHeight, colliderWidth, colliderDepth;
	private float Margin = 0.05f;
	private float slideSpeed = 20;
	
	private Vector3 startPos;
	private Vector3 nextVelocity;//previously called setVelocity
	
	private CollisionChecker colCheck;
	
	private bool respawnFlag, startPush;
	
	private PerspectiveType persp = PerspectiveType.p3D;
	
	private bool[] axisBlocked = new bool[4];

	public GameObject brokenIceSpawnPoint;
	public GameObject brokenIce;
	public GameObject spawnCircle;

	void Awake() {
		colliderHeight = GetComponent<Collider>().bounds.size.y;
		colliderWidth = GetComponent<Collider>().bounds.size.x;
		colliderDepth = GetComponent<Collider>().bounds.size.z;
	}

	void Start() {
		base.StartSetup ();
		grounded = false;
		velocity = Vector3.zero;
		nextVelocity = Vector3.zero;

		// Register CheckGrab to grab input event
		//InputManager.instance.InteractPressed += CheckGrab;
		GameStateManager.instance.PerspectiveShiftEvent += Shift;
		CameraController.instance.TransitionStartEvent += checkBreak;
		range = colliderWidth >= colliderDepth ? colliderWidth * 0.85f : colliderDepth * 0.85f;
		colCheck = new CollisionChecker (GetComponent<Collider> ());
		startPos = transform.position;
		
		for (int i = 0; i < 4; i++)
			axisBlocked[i] = false;
	}

	void Update() {
		if (!nextVelocity.Equals(Vector3.zero)) {
			velocity = nextVelocity;
			nextVelocity = Vector3.zero;
			startPush = true;
			BoundObject binder = gameObject.GetComponent<BoundObject>();
			if(binder!=null)
				binder.bind();
		}
		CheckCollisions();
	}

	void FixedUpdate() {
		base.FixedUpdateLogic ();
		if (!grounded)
			velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - gravity, -terminalVelocity), velocity.z);
		
		/*if (grabbed) {
			float vy = velocity.y;
			velocity = player.GetComponent<PlayerController>().GetVelocity();
			velocity.y = vy;
		}*/
		
		//CheckCollisions();
		
		/*float newVelocityX = velocity.x, newVelocityZ = velocity.z;
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
		velocity.z = newVelocityZ;*/
		
		if (GetComponent<Collider> ().enabled) {
			colliderHeight = GetComponent<Collider>().bounds.size.y;
			colliderWidth = GetComponent<Collider>().bounds.size.x;
			colliderDepth = GetComponent<Collider>().bounds.size.z;
		}
		
		if (svFlag) {
			velocity.x = newVelocity.x;
			velocity.z = newVelocity.z;
			svFlag = false;
		}
		
		//CheckCollisions();
	}
	
	void LateUpdate () {
		base.LateUpdateLogic ();
		transform.Translate(velocity * Time.deltaTime);
		if (respawnFlag && Vector2.Distance(new Vector2(startPos.x, startPos.y), new Vector2(player.transform.position.x, player.transform.position.y)) > colliderWidth) {
			Vector3 pos = transform.position;
			pos = startPos + Vector3.up;
			transform.position = pos;
			GetComponent<Collider>().enabled = true;
			GetComponentInChildren<Renderer>().enabled = true;
			GameObject.Instantiate(spawnCircle, transform.position, Quaternion.identity);
			velocity = Vector3.zero;
			respawnFlag = false;
		}
		
		BoundObject binder = gameObject.GetComponent<BoundObject>();
		if(binder != null)
			binder.bind();
		
		//call custom bind

		if (startPush) {
			if (velocity.Equals(Vector3.zero)){
				respawnFlag = true;
				GetComponent<Collider>().enabled = false;
				GetComponentInChildren<Renderer>().enabled = false;
				if(brokenIce != null){
					GameObject.Instantiate(brokenIce, brokenIceSpawnPoint.transform.position, Quaternion.identity);
				}
			}
			startPush = false;
		}

		//CheckCollisions();
	}
	
	public void CheckCollisions() {
		Vector3 trajectory;
		
		RaycastHit[] hits = colCheck.CheckYCollision (velocity, Margin);
		
		for (int i = 0; i < 4; i++)
			axisBlocked[i] = false;
		
		float close = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null)
			{
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity.y * Vector3.up;
					CollideWithObject(hitInfo, trajectory);
				} else if (close == -1 || close > hitInfo.distance) {
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
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity.x * Vector3.right;
					CollideWithObject(hitInfo, trajectory);
				} else if (close == -1 || close > hitInfo.distance) {
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
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity.z * Vector3.forward;
					CollideWithObject(hitInfo, trajectory);
				} else if (close == -1 || close > hitInfo.distance) {
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
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p2D && Check2DIntersect ()) {
			respawnFlag = true;
			//TODO
			if(brokenIce != null){
				GameObject.Instantiate(brokenIce, brokenIceSpawnPoint.transform.position, Quaternion.identity);
			}
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
		// Rune Switch
		if (other.GetComponent<PushSwitch>()) {
			other.GetComponent<PushSwitch>().EnterCollisionWithGeneral(gameObject);
		}
		//Collision w/ PlayerInteractable
		foreach(Interactable c in other.GetComponents<Interactable>()){
			c.EnterCollisionWithGeneral(gameObject);
		}
	}
	//Mathf.Abs(player.transform.position.x - transform.position.x) > colliderWidth / 2
	public override void Triggered() {
		if (velocity.Equals(Vector3.zero)) {
			if (Mathf.Abs(player.transform.position.x - transform.position.x) > colliderWidth / 2 || persp == PerspectiveType.p2D) {
				if (player.transform.position.x - transform.position.x > 0)
					nextVelocity = Vector3.left * slideSpeed;
				else
					nextVelocity = Vector3.right * slideSpeed;
			} else {
				if (player.transform.position.z - transform.position.z > 0)
					nextVelocity = Vector3.back * slideSpeed;
				else
					nextVelocity = Vector3.forward * slideSpeed;
			}
		}
	}

	private void Shift(PerspectiveType p) {
		persp = p;
	}
}
