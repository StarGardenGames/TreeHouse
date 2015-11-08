using UnityEngine;
using System.Collections;

public class MobilePlatform : ActiveInteractable {

	public float acceleration = 1.5f;
	public float decelleration = 15f;
	public float maxSpeed = 8f;
	bool controlled = false;

	private float colliderHeight, colliderWidth, colliderDepth;

	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;

	private CollisionChecker colCheck;
	private float Margin = 0.05f;

	void Start() {
		StartSetup();
		colCheck = new CollisionChecker (GetComponent<Collider> ());
		colliderHeight = GetComponent<Collider>().bounds.size.y;
		colliderWidth = GetComponent<Collider>().bounds.size.x;
		colliderDepth = GetComponent<Collider>().bounds.size.z;
	}

	void FixedUpdate () {
		base.FixedUpdateLogic();
		moveOnAxis(X);
		moveOnAxis(Z);
		CheckCollisions();
	}

	void Update() {
		if (player.transform.position.y - 0.5f < transform.position.y) {
			range = float.MinValue;
		} else {
			range = GetComponent<Collider>().bounds.size.y + 1f;
		}
	}

	void LateUpdate() {
		LateUpdateLogic();
		transform.Translate(velocity * Time.deltaTime);
	}

	private void moveOnAxis(int axis){
		float newVelocity = velocity[axis];
		
		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
				-InputManager.instance.GetSideMovement();

		if (!controlled)
			axisInput = 0;

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

	public void CheckCollisions() {
		Vector3 trajectory;
		
		RaycastHit[] hits = colCheck.CheckYCollision (velocity, Margin);

		// If any rays connected move the player and set grounded to true since we're now on the ground
		
		hits = colCheck.CheckXCollision (velocity, Margin);
		
		float close = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null)
			{
				if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
					trajectory = velocity.x * Vector3.right;
				}
			}
		}
		if (close != -1) {
			//transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (close - colliderWidth / 2));
			velocity = new Vector3(0f, velocity.y, velocity.z);
		}

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
				}
			}
		}
		if (close != -1) {
			//transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (close - colliderDepth / 2));
			velocity = new Vector3(velocity.x, velocity.y, 0f);
		}
	}

	public override void Triggered() {
		if (!controlled && player.transform.position.y > transform.position.y) {
			PlayerController.instance.setRiding(this.gameObject);
			controlled = true;
			player.transform.parent = transform;
		} else {
			PlayerController.instance.setRiding(null);
			controlled = false;
			player.transform.parent = null;
		}
	}

	public override float GetDistance() {
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if (pPos.x >= bounds.min.x && pPos.x <= bounds.max.x && pPos.z >= bounds.min.z && pPos.z <= bounds.max.z) {
			return pPos.y - transform.position.y;
		}
		return float.MaxValue;
	}

	protected override bool isPlayerFacingObject() {
		return true;
	}
}
