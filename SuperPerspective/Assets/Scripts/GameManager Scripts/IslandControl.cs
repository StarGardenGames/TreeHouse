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
		generateBounds ();
	}

	void generateBounds(){
		grounds = GameObject.FindGameObjectsWithTag("Ground");
		islandBounds = new Rect[grounds.Length];
		float gX, gZ, gW, gD;
		for (int i = 0; i < grounds.Length; i++) {
			gX = grounds[i].transform.position.x;
			gZ = grounds[i].transform.position.z;
			gW = grounds[i].transform.lossyScale.x;
			gD = grounds[i].transform.lossyScale.z;
			islandBounds[i] = new Rect(
				gX - gW/2f, gZ - gD/2f, gW, gD);
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
			if(yy <= grounds[i].transform.position.y)
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
}
