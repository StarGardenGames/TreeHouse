/*Implementation Requirements
 * Islands must not overlap when viewed from above
 * Player must update bounds when switching view
 * Player must update bounds when it lands on a surface
 * Camera must be updated when switching views
 * 
 * Involved scripts/objects
 * Guard Prefab must exist
 * BoundObject script must be applied to necessary objects
 */

using UnityEngine;
using System.Collections;

public class IslandControl : MonoBehaviour {

	public static IslandControl instance;
	
	GameObject[] grounds;
	Transform[] pauseMounts;
	public Rect[] islandBounds;
	//public float[,] altBounds;

	// Use this for initialization
	void Awake () {
		//singleton
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		//init islandBounds
		generateBounds();
		findPauseMounts();
	}

	void findPauseMounts(){
		pauseMounts = new Transform[grounds.Length];
		for(int i = 0; i < pauseMounts.Length; i++){
			pauseMounts[i] = grounds[i].transform.parent.Find("PauseMount");
		}
	}

	void generateBounds(){
		grounds = GameObject.FindGameObjectsWithTag("Ground");
		islandBounds = new Rect[grounds.Length];
		for (int i = 0; i < grounds.Length; i++) {
			Bounds bounds = grounds[i].GetComponent<Collider>().bounds;
			islandBounds[i] = new Rect(
				bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
		}
	}

	//note islands cannot be overlapping (along x axis?)
	public int getBound(float xx, float yy, float zz, bool onlyX){
		//find valid bounds
		int numValid = 0;
		int curValid = -1;
		//note: valid initailized to all false
		bool[] valid = new bool[islandBounds.Length];
		for (int i = 0; i < islandBounds.Length; i++) {
			if (xx < islandBounds [i].xMin)
					continue;
			if (!onlyX && zz < islandBounds [i].yMin)
					continue;
			if (xx  > islandBounds [i].xMax)
					continue;
			if (!onlyX && zz > islandBounds [i].yMax)
					continue;
			valid [i] = true;
			curValid = i;
			numValid++;
		}

		//check for duplicates
		if (numValid > 1) {
			curValid = -1;
			float bestY = 0;
			for(int i = 0; i< grounds.Length; i++){
				if(!valid[i])
					continue;
				float groundY = grounds[i].transform.position.y;
				if(curValid == -1 || (bestY < groundY && groundY < yy)){
					curValid = i;
					bestY = groundY;
				}
			}
		}
		return curValid;
	}
	
	public GameObject findGround(GameObject obj){
		Vector3 pos = obj.transform.position;
		int boundIndex = getBound (pos.x, pos.y, pos.z, !PlayerController.instance.is3D());
		if(boundIndex == -1){
			throw new System.ArgumentException(
				"(IslandControl) There is no valid bound for "+obj +
				"\nMake sure all objects are placed within the bounds of a ground");
		}
		return grounds[boundIndex];
	}
	
	public Transform findCurrentPauseMount(){
		Vector3 playerPos = PlayerController.instance.transform.position;
		int boundIndex = getBound(playerPos.x, playerPos.y, playerPos.z, false);
		return pauseMounts[boundIndex];
	}
}
