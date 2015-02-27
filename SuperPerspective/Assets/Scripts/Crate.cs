using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	private const float gravity = 1.5f;
	private const float terminalVelocity = 60;
	private const float decelleration = 15;

	private Vector3 velocity;
	private bool grounded;
	private float colliderHeight, colliderWidth, colliderDepth;
	float Margin = 0.05f;

	void Start() {
		grounded = false;
		velocity = Vector3.zero;
		colliderHeight = collider.bounds.max.y - collider.bounds.min.y;
		colliderWidth = collider.bounds.max.x - collider.bounds.min.x;
		colliderDepth = collider.bounds.max.z - collider.bounds.min.z;
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
		
		// Check for collisions below the player if he/she is not moving up
		//if (grounded || falling)
		//{
		// Check each of the four corners and the center of the collider
		// TODO: Store coordinates in an array to do this as a loop
		
		// True if any ray hits a collider
		//bool connected = false;
		
		// Set the raycast distance to check as far as the player will fall this frame
		distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);
		
		// Top Left (Min X, Max Z)
		startPoint = new Vector3(collider.bounds.min.x + Margin, collider.bounds.center.y, collider.bounds.max.z - Margin);
		ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
		connected = Physics.Raycast(ray, out hitInfo, distance);
		
		// Top Right (Max X, Max Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.max.x - Margin, collider.bounds.center.y, collider.bounds.max.z - Margin);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Left (Min X, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.min.x + Margin, collider.bounds.center.y, collider.bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Right (Max X, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.max.x - Margin, collider.bounds.center.y, collider.bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Center (Center Y, Center Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.center.z);
			ray = new Ray(startPoint, Vector3.up * Mathf.Sign(velocity.y));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		if (connected)
		{
			if (velocity.y > 0)
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
		startPoint = new Vector3(collider.bounds.center.x, collider.bounds.min.y + Margin, collider.bounds.max.z - Margin);
		ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
		connected = Physics.Raycast(ray, out hitInfo, distance);
		
		// Top Front (Max Y, Max Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.center.x, collider.bounds.max.y - Margin, collider.bounds.max.z - Margin);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Back (Min Y, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.center.x, collider.bounds.min.y + Margin, collider.bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Top Back (Max Y, Min Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.center.x, collider.bounds.max.y - Margin, collider.bounds.min.z + Margin);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Center (Center Y, Center Z)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.center.z);
			ray = new Ray(startPoint, Vector3.right * Mathf.Sign(velocity.x));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
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
		startPoint = new Vector3(collider.bounds.min.x + Margin, collider.bounds.max.y - Margin, collider.bounds.center.z);
		ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
		connected = Physics.Raycast(ray, out hitInfo, distance);
		
		// Top Right (Max X, Max Y)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.max.x - Margin, collider.bounds.max.y - Margin, collider.bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Left (Min X, Min Y)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.min.x + Margin, collider.bounds.min.y + Margin, collider.bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Bottom Right (Max X, Min Y)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.max.x - Margin, collider.bounds.min.y + Margin, collider.bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// Center (Center X, Center Y)
		if (!connected)
		{
			startPoint = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.center.z);
			ray = new Ray(startPoint, Vector3.forward * Mathf.Sign(velocity.z));
			connected = Physics.Raycast(ray, out hitInfo, distance);
		}
		
		// If any rays connected move the player and set grounded to true since we're now on the ground
		if (connected)
		{
			transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
			velocity = new Vector3(velocity.x, velocity.y, 0f);
		}

		#endregion Checking Z Axis

	}

	void LateUpdate () {
		transform.Translate(velocity * Time.deltaTime);
	}

	public void AddVelocity(float x, float y, float z) {
		velocity.x += x;
		velocity.y += y;
		velocity.z += z;
	}
}
