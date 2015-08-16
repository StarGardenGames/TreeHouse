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
		Vector3 edgeScale = edgePrefab.transform.lossyScale;
		
		Vector3 posBack = new Vector3 (0, 0, .5f);
		Vector3 posLeft = new Vector3 (-.5f, 0, 0);
		Vector3 posRight = new Vector3 (.5f,0,0);
		Vector3 posFront = new Vector3 (0, 0, -.5f);

		Vector3 posTop = new Vector3(0,.5f,0);

		for(int i = 0; i < terrain.Length; i++){
			Vector3 boxSize = terrain[i].GetComponent<LevelGeometry>().getTrueBoxColliderSize();
			Vector3 boxCenter = terrain[i].GetComponent<LevelGeometry>().getTrueBoxColliderCenter();
			
			Vector3 transformScale = terrain[i].transform.lossyScale;			
			
			Vector3 trueEdgeScale = new Vector3(
				edgeScale.x, edgeScale.y, edgeScale.z
			);
			
			Vector3 trueTerrainScale = new Vector3(
				boxSize.x * transformScale.x,
				boxSize.y * transformScale.y,
				boxSize.z * transformScale.z
			);
			
			Vector3 trueTerrainCenter = 
				(Vector3.right * boxCenter.x * transformScale.x) +
				(Vector3.up * boxCenter.y * transformScale.y) +
				(Vector3.forward * boxCenter.z * transformScale.z);
			
			int orOffset = (int)Mathf.Round((float)(terrain[i].transform.rotation.eulerAngles.y / 90.0));
			if(orOffset % 2 == 1){
				trueEdgeScale = flipXZ(trueEdgeScale);
				trueTerrainScale = flipXZ(trueTerrainScale);
				trueTerrainCenter = flipXZ(trueTerrainCenter);
			}
			
			if(index == 0 || index==4)Debug.Log(trueTerrainScale);
			
			Vector3 top = terrain[i].transform.position + trueTerrainCenter + (trueTerrainScale.y-trueEdgeScale.y) * posTop;
			
			GameObject rightEdge = Instantiate(edgePrefab, top + (trueTerrainScale.x+trueEdgeScale.x) * posRight, Quaternion.identity) as GameObject;
			GameObject backEdge = Instantiate(edgePrefab, top + (trueTerrainScale.z+trueEdgeScale.z) * posBack, Quaternion.identity) as GameObject;
			GameObject leftEdge = Instantiate(edgePrefab, top + (trueTerrainScale.x+trueEdgeScale.x) * posLeft, Quaternion.identity) as GameObject;
			GameObject frontEdge = Instantiate(edgePrefab, top + (trueTerrainScale.z+trueEdgeScale.z) * posFront, Quaternion.identity) as GameObject;
			
			rightEdge.GetComponent<Edge>().Init(0, trueTerrainScale.x, trueTerrainScale.z, 
				"orig_"+index,terrain[i].transform);
			backEdge.GetComponent<Edge>().Init(1, trueTerrainScale.x, trueTerrainScale.z, 
				"orig_"+(index+1),terrain[i].transform);
			leftEdge.GetComponent<Edge>().Init(2, trueTerrainScale.x, trueTerrainScale.z,
				"orig_"+(index+2),terrain[i].transform);
			frontEdge.GetComponent<Edge>().Init(3, trueTerrainScale.x, trueTerrainScale.z, 
				"orig_"+(index+3),terrain[i].transform);
						
			index += 4;
		}
		
		generated = true;
	}

	//convenience method for flipping the x and z values in a Vector3
	private Vector3 flipXZ(Vector3 input){
		return new Vector3(
			input.z, input.y, input.x
		);
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
