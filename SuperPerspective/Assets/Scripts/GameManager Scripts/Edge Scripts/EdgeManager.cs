using UnityEngine;
using System.Collections;

public class EdgeManager : MonoBehaviour {

	public static EdgeManager instance;

	public GameObject edgePrefab;

	private GameObject[] terrain;

	int index = 0;
	
	bool generated = false;
	
	void Start () {
		//sigleton
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);

		//init terrain
		terrain = GameObject.FindGameObjectsWithTag("Terrain");

	}

	void FixedUpdate(){
		//generate edges
		if(terrain != null && generated == false)
			GenerateEdges();
	}
	
	public void GenerateEdges(){
		float xx = edgePrefab.transform.lossyScale.x;
		float yy = edgePrefab.transform.lossyScale.y;
		float zz = edgePrefab.transform.lossyScale.z;
		
		Vector3 posBack = new Vector3 (0, 0, .5f);
		Vector3 posLeft = new Vector3 (-.5f, 0, 0);
		Vector3 posRight = new Vector3 (.5f,0,0);
		Vector3 posFront = new Vector3 (0, 0, -.5f);

		Vector3 posTop = new Vector3(0,.5f,0);

		for(int i = 0; i < terrain.Length; i++){
			Vector3 boxSize = terrain[i].GetComponent<LevelGeometry>().getTrueBoxColliderSize();
			Vector3 boxCenter = terrain[i].GetComponent<LevelGeometry>().getTrueBoxColliderCenter();
			
			float w = terrain[i].transform.lossyScale.x * boxSize.x;
			float d = terrain[i].transform.lossyScale.z * boxSize.z;
			float h = terrain[i].transform.lossyScale.y * boxSize.y;
			
			int orOffset = (int)Mathf.Round((float)(terrain[i].transform.rotation.eulerAngles.y / 90.0));
			if(orOffset % 2 == 1){
				float t = w;
				w = d;
				d = t;
			}
			
			w+=xx;
			d+=zz;
			h-=yy;
	
			Vector3 transformScale = terrain[i].transform.lossyScale;
			
			Vector3 center = 
				(Vector3.right * boxCenter.x * transformScale.x) +
				(Vector3.up * boxCenter.y * transformScale.y) +
				(Vector3.forward * boxCenter.z * transformScale.z);
			
			Vector3 top = terrain[i].transform.position + center + h * posTop;
			
			GameObject rightEdge = Instantiate(edgePrefab, top + w * posRight, Quaternion.identity) as GameObject;
			GameObject backEdge = Instantiate(edgePrefab, top + d * posBack, Quaternion.identity) as GameObject;
			GameObject leftEdge = Instantiate(edgePrefab, top + w * posLeft, Quaternion.identity) as GameObject;
			GameObject frontEdge = Instantiate(edgePrefab, top + d * posFront, Quaternion.identity) as GameObject;
			
			rightEdge.GetComponent<Edge>().Init(0, w - xx, d - zz, 
				"orig_"+index,terrain[i].transform);
			backEdge.GetComponent<Edge>().Init(1, w - xx, d - zz, 
				"orig_"+(index+1),terrain[i].transform);
			leftEdge.GetComponent<Edge>().Init(2, w - xx, d - zz, 
				"orig_"+(index+2),terrain[i].transform);
			frontEdge.GetComponent<Edge>().Init(3, w - xx, d - zz, 
				"orig_"+(index+3),terrain[i].transform);
						
			index += 4;
		}
		
		generated = true;
	}

	//check if terrain i overlaps with arbitrary cuboid
	public Vector3[] GetOverlap(int i, Vector3[] c){
		//compute corners
		Vector3 halfScale = terrain[i].transform.localScale * .5f;
		Vector3 p0 = terrain[i].transform.position - halfScale;
		Vector3 p1 = terrain[i].transform.position + halfScale;
		//result
		Vector3[] region = new Vector3[2];
		bool overlap = true;
		//check overlaps in all 3 dimenions
		for(int k = 0; k < 3; k++){
			overlap = overlap && 
				p0[k] + .00001f < c[1][k] &&
				p1[k] - .00001f > c[0][k];
			region[0][k] = Mathf.Max(p0[k],c[0][k]);
			region[1][k] = Mathf.Min(p1[k],c[1][k]);
		}
		if(!overlap)
			return null;
		else
			return region;
	}

	public int getGlobalIndex(){
		index ++;
		return index-1;
	}
	
	public bool CheckOverlap2D(int i, Vector3[] c){
		//compute corners
		Vector3 halfScale = terrain[i].transform.localScale * .5f;
		Vector3 p0 = terrain[i].transform.position - halfScale;
		Vector3 p1 = terrain[i].transform.position + halfScale;
		//result
		bool overlap = true;
		//check overlaps in 2 dimensions dimenions
		for(int k = 0; k < 2; k++){
			overlap = overlap && 
				p0[k] < c[1][k] &&
					p1[k] > c[0][k];
		}
		return overlap;
	}
	
	public GameObject[] getTerrain(){
		return terrain;
	}
}
