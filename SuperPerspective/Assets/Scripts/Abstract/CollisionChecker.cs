using UnityEngine;
using System.Collections;

public class CollisionChecker {

	Collider col;
	float colliderWidth, colliderHeight, colliderDepth;
	public float precision = 2;
	
	#pragma warning disable 219

	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;
	
	public CollisionChecker(Collider col) {
		this.col = col;
		colliderWidth = col.bounds.size.x;
		colliderHeight = col.bounds.size.y;
		colliderDepth = col.bounds.size.z;
	}
	
	public RaycastHit[] CheckCollisionOnAxis(int axis, Vector3 velocity, float Margin){
		switch(axis){
			case X: return CheckXCollision(velocity, Margin);
			case Y: return CheckYCollision(velocity, Margin);
			case Z: return CheckZCollision(velocity, Margin);
			default:
				throw new System.ArgumentException("Invalid Axis Character");
		}
	}
	
	public RaycastHit[] CheckXCollision(Vector3 velocity, float Margin) {
		colliderWidth = col.bounds.max.x - col.bounds.min.x;
		RaycastHit[] hits = new RaycastHit[(int)Mathf.Pow(precision + 1, 2)];
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
		
		//test all startpoints
		Vector3 dir = Vector3.right * Mathf.Sign(velocity.x);
		for (int i = 0; i <= precision; i++) {
			for (int j = 0; j <= precision; j++) {
				connected = Physics.Raycast(new Vector3(centerX, minY + (maxY - minY) * (i / precision), minZ + (maxZ - minZ) * (j / precision)), dir, out hitInfo, distance);
				hits[(int)(i * precision + j)] = hitInfo;
				i++;
			}
		}
		
		return hits;
	}

	public RaycastHit[] CheckYCollision(Vector3 velocity, float Margin) {
		colliderHeight = col.bounds.max.y - col.bounds.min.y;
		RaycastHit[] hits = new RaycastHit[(int)Mathf.Pow(precision + 1, 2)];
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
		for (int i = 0; i <= precision; i++) {
			for (int j = 0; j <= precision; j++) {
				connected = Physics.Raycast(new Vector3(minX + (maxX - minX) * (i / precision), centerY, minZ + (maxZ - minZ) * (j / precision)), dir, out hitInfo, distance);
				hits[(int)(i * precision + j)] = hitInfo;
				i++;
			}
		}
		
		return hits;
	}

	public RaycastHit[] CheckZCollision(Vector3 velocity, float Margin) {
		colliderDepth = col.bounds.max.z - col.bounds.min.z;
		RaycastHit[] hits = new RaycastHit[(int)Mathf.Pow(precision + 1, 2)];
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
		
		//loop through and check all startpoints
		Vector3 dir = Vector3.forward * Mathf.Sign(velocity.z);
		for (int i = 0; i <= precision; i++) {
			for (int j = 0; j <= precision; j++) {
				connected = Physics.Raycast(new Vector3(minX + (maxX - minX) * (i / precision), minY + (maxY - minY) * (j / precision), centerZ), dir, out hitInfo, distance);
				hits[(int)(i * precision + j)] = hitInfo;
				i++;
			}
		}

		return hits;
	}
}
