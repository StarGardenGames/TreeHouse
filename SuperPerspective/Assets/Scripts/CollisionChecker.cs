using UnityEngine;
using System.Collections;

public class CollisionChecker {

	Collider col;
	float colliderWidth, colliderHeight, colliderDepth;
	
	#pragma warning disable 219

	public CollisionChecker(Collider col) {
		this.col = col;
		colliderWidth = col.bounds.size.x;
		colliderHeight = col.bounds.size.y;
		colliderDepth = col.bounds.size.z;
	}
	
	public RaycastHit[] CheckCollisionOnAxis(char axis, Vector3 velocity, float Margin){
		switch(axis){
			case 'X': return CheckXCollision(velocity, Margin);
			case 'Y': return CheckYCollision(velocity, Margin);
			case 'Z': return CheckZCollision(velocity, Margin);
			default:
				throw new System.ArgumentException("Invalid Axis Character");
		}
	}
	
	public RaycastHit[] CheckXCollision(Vector3 velocity, float Margin) {
		colliderWidth = col.bounds.max.x - col.bounds.min.x;
		Vector3[] startPoints = new Vector3[5];
		RaycastHit[] hits = new RaycastHit[5];
		RaycastHit hitInfo = new RaycastHit();

		float minX 		= col.bounds.min.x + Margin;
		float centerX 	= col.bounds.center.x;
		float maxX 		= col.bounds.max.x - Margin;
		float minY 		= col.bounds.min.y + Margin;
		float centerY 	= col.bounds.center.y;
		float maxY 		= col.bounds.max.y - Margin;
		float minZ 		= col.bounds.min.z + Margin;
		float centerZ	= col.bounds.center.z;
		float maxZ 		= col.bounds.max.z - Margin;

		// True if any ray hits a collider
		bool connected = false;
		
		// Set the raycast distance to check as far as the player will move this frame
		float distance = (colliderWidth / 2) + Mathf.Abs(velocity.x * Time.deltaTime);
		
		//setup startPoints
		startPoints[0] = new Vector3(centerX, minY, maxZ);
		startPoints[1] = new Vector3(centerX, maxY, maxZ);
		startPoints[2] = new Vector3(centerX, minY, minZ);
		startPoints[3] = new Vector3(centerX, maxY, minZ);
		startPoints[4] = new Vector3(centerX, centerY, centerZ);
		
		//test all startpoints
		Vector3 dir = Vector3.right * Mathf.Sign(velocity.x);
		connected = Physics.Raycast(startPoints[0], dir, out hitInfo, distance);
		hits[0] = hitInfo;
		int i = 1;
		while (i < startPoints.Length) {
			connected = Physics.Raycast(startPoints[i], dir, out hitInfo, distance);
			hits[i] = hitInfo;
			i++;
		}
		
		return hits;
	}

	public RaycastHit[] CheckYCollision(Vector3 velocity, float Margin) {
		colliderHeight = col.bounds.max.y - col.bounds.min.y;
		RaycastHit[] hits = new RaycastHit[5];
		RaycastHit hitInfo = new RaycastHit();
		
		float minX 		= col.bounds.min.x + Margin;
		float centerX 	= col.bounds.center.x;
		float maxX 		= col.bounds.max.x - Margin;
		float minY 		= col.bounds.min.y + Margin;
		float centerY 	= col.bounds.center.y;
		float maxY 		= col.bounds.max.y - Margin;
		float minZ 		= col.bounds.min.z + Margin;
		float centerZ	= col.bounds.center.z;
		float maxZ 		= col.bounds.max.z - Margin;
		
		// True if any ray hits a collider
		bool connected = false;
		
		// Set the raycast distance to check as far as the player will fall this frame
		float distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);
		
		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, centerY, maxZ),
			new Vector3(maxX, centerY, maxZ),
			new Vector3(minX, centerY, minZ),
			new Vector3(maxX, centerY, minZ),
			new Vector3(centerX, centerY, centerZ)
		};
		
		//test all startpoints
		Vector3 dir = Vector3.up * Mathf.Sign(velocity.y);
		// must run outside loop once to ensure hitInfo is initialized
		connected = Physics.Raycast(startPoints[0], dir, out hitInfo, distance);
		hits[0] = hitInfo;
		int i = 1;
		while (i < startPoints.Length) {
			connected = Physics.Raycast(startPoints[i], dir, out hitInfo, distance);
			hits[i] = hitInfo;
			i++;
		}
		
		return hits;
	}

	public RaycastHit[] CheckZCollision(Vector3 velocity, float Margin) {
		colliderDepth = col.bounds.max.z - col.bounds.min.z;
		Vector3[] startPoints = new Vector3[5];
		RaycastHit[] hits = new RaycastHit[5];
		RaycastHit hitInfo = new RaycastHit();
		
		float minX 		= col.bounds.min.x + Margin;
		float centerX 	= col.bounds.center.x;
		float maxX 		= col.bounds.max.x - Margin;
		float minY 		= col.bounds.min.y + Margin;
		float centerY 	= col.bounds.center.y;
		float maxY 		= col.bounds.max.y - Margin;
		float minZ 		= col.bounds.min.z + Margin;
		float centerZ	= col.bounds.center.z;
		float maxZ 		= col.bounds.max.z - Margin;

		bool connected = false;

		// Set the raycast distance to check as far as the player will move this frame
		float distance = (colliderDepth / 2 + Mathf.Abs(velocity.z * Time.deltaTime));
		
		//setup startPoints arary
		startPoints[0] = new Vector3(minX, maxY, centerZ);
		startPoints[1] = new Vector3(maxX, maxY, centerZ);
		startPoints[2] = new Vector3(minX, minY, centerZ);
		startPoints[3] = new Vector3(maxX, minY, centerZ);
		startPoints[4] = new Vector3(centerX, centerY, centerZ);
		
		//loop through and check all startpoints
		Vector3 dir = Vector3.forward * Mathf.Sign(velocity.z);
		connected = Physics.Raycast(startPoints[0], dir, out hitInfo, distance);
		hits[0] = hitInfo;
		int i = 1;
		while (i < startPoints.Length) {
			connected = Physics.Raycast(startPoints[i], dir, out hitInfo, distance);
			hits[i] = hitInfo;
			i++;
		}

		return hits;
	}
}
