using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	private const float gravity = 1.5f;
	private const float terminalVelocity = 60;
	private const float decelleration = 15;

	private Vector3 velocity;
	private bool grounded;
	private float colliderHeight, colliderWidth, colliderDepth;
	private float Margin = 0.05f;

	private PlayerController3 player;

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
		InputManager.instance.GrabPressed += CheckGrab;
		GameStateManager.instance.PerspectiveShiftEvent += Shift;
	}

	void FixedUpdate() {
		Vector3 startPoint;     // The starting point of a ray  
		float distance;         // The distance of a ray
		RaycastHit hitInfo;     // Stored the information if a ray registers a hit

		if (!grounded)
			velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - gravity, -terminalVelocity), velocity.z);

		float newVelocityX = velocity.x;
		if (velocity.x != 0) {
			int modifier = velocity.x > 0 ? -1 : 1;
			newVelocityX += Mathf.Min (decelleration, Mathf.Abs (velocity.x)) * modifier;
		}
		velocity.x = newVelocityX;
		
		float newVelocityZ = velocity.z;
		if (velocity.z != 0) {
			int modifier = velocity.z > 0 ? -1 : 1;
			newVelocityZ += Mathf.Min (decelleration, Mathf.Abs (velocity.z)) * modifier;
		}
		velocity.z = newVelocityZ;

		bool connected = false;
		Ray ray;
		
		// Set the raycast distance to check as far as the player will fall this frame
		distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);
		
		// Top Left (Min X, Max Z)
		startPoint = new Vector3(GetComponent<Collider>().bounds.min.x + Margin, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.max.z - Margin);
		ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
		connected = Physics.Raycast(ray, out hitInfo, distance);
		
		// Top Right (Max X, Max Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.max.x - Margin, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.max.z - Margin);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Left (Min X, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.min.x + Margin, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Right (Max X, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.max.x - Margin, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Center (Center Y, Center Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.center.z);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// If any rays connected move the crate and set grounded to true since we're now on the ground
		if (connected)
		{
			if (velocity.y < 0)
				grounded = true;
			transform.Translate(Vector3.up * Mathf.Sign(velocity.y) * (hitInfo.distance - colliderHeight / 2));
			velocity = new Vector3(velocity.x, 0f, velocity.z);
		}
		
		// Otherwise we're not grounded (temporary?)
		else
			grounded = false;

		#region Checking X Axis
		// True if any ray hits a collider
		connected = false;
		
		// Set the raycast distance to check as far as the player will move this frame
		distance = (colliderWidth / 2) + Mathf.Abs(velocity.x * Time.deltaTime);
		
		// Bottom Front (Min Y, Max Z)
		startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.min.y + Margin, GetComponent<Collider>().bounds.max.z - Margin);
		ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
		connected = Physics.Raycast(ray, out hitInfo, distance);
		
		// Top Front (Max Y, Max Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.max.y - Margin, GetComponent<Collider>().bounds.max.z - Margin);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Back (Min Y, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.min.y + Margin, GetComponent<Collider>().bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Top Back (Max Y, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.max.y - Margin, GetComponent<Collider>().bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Center (Center Y, Center Z)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.center.z);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// If any rays connected move the crate and set grounded to true since we're now on the ground
		if (connected)
		{
			transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
			velocity = new Vector3(0f, velocity.y, velocity.z);
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
		
		// Top Left (Min X, Max Y)
		startPoint = new Vector3(GetComponent<Collider>().bounds.min.x + Margin, GetComponent<Collider>().bounds.max.y - Margin, GetComponent<Collider>().bounds.center.z);
		ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
		connected = Physics.Raycast(ray, out hitInfo, distance);
		
		// Top Right (Max X, Max Y)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.max.x - Margin, GetComponent<Collider>().bounds.max.y - Margin, GetComponent<Collider>().bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Left (Min X, Min Y)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.min.x + Margin, GetComponent<Collider>().bounds.min.y + Margin, GetComponent<Collider>().bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Right (Max X, Min Y)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.max.x - Margin, GetComponent<Collider>().bounds.min.y + Margin, GetComponent<Collider>().bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Center (Center X, Center Y)
		if (!connected)
		{
			startPoint = new Vector3(GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.center.y, GetComponent<Collider>().bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// If any rays connected move the crate and set grounded to true since we're now on the ground
		if (connected)
		{
			transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
			velocity = new Vector3(velocity.x, velocity.y, 0f);
		}

		#endregion Checking Z Axis

	}

	void LateUpdate () {
		if (grabbed && !PlayerInRange()) {
			player.Grab(null);
			grabbed = false;
		}
		transform.Translate(velocity * Time.deltaTime);
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
