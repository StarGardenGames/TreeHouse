using UnityEngine;
using System.Collections;

public class EdgeManager : MonoBehaviour {

	#pragma warning disable 219

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
		for(int i = 0; i < terrain.Length; i++){	
			CreateEdgesAroundTerrain(terrain[i]);
		}
		generated = true;
	}
	
	private void CreateEdgesAroundTerrain(GameObject terrain){
		Vector3 top = findTerrainTopCenterForEdge(terrain);
		Vector3 edgeScale = findTrueEdgeScale(terrain);
		Vector3 terrainScale = findTrueTerrainScale(terrain);
		
		CreateEdge(terrain, top, Vector3.right, (terrainScale+edgeScale).x * .5f, terrainScale);
		CreateEdge(terrain, top, Vector3.forward, (terrainScale+edgeScale).z * .5f, terrainScale);
		CreateEdge(terrain, top, Vector3.left, (terrainScale+edgeScale).x * .5f, terrainScale);
		CreateEdge(terrain, top, Vector3.back, (terrainScale+edgeScale).z * .5f, terrainScale);
	}
	
	private Vector3 findTerrainTopCenterForEdge(GameObject terrain){
		Vector3 terrainScale = findTrueTerrainScale(terrain);
		Vector3 centerOffset = findTrueTerrainCenter(terrain);
		Vector3 edgeScale = findTrueEdgeScale(terrain);
		return
			terrain.transform.position + centerOffset + (terrainScale.y-edgeScale.y) * Vector3.up * .5f;
	}
	
	private Vector3 findTrueEdgeScale(GameObject terrain){
		Vector3 scale = edgePrefab.transform.lossyScale;
		if(isTerrainOnRotatedQuadrant(terrain))
			return flipXZ(scale);
		else
			return new Vector3(scale.x,scale.y,scale.z);
	}
	
	private Vector3 findTrueTerrainScale(GameObject terrain){
		Vector3 boxSize = terrain.GetComponent<LevelGeometry>().getTrueBoxColliderSize();
		Vector3 transformScale = terrain.transform.lossyScale;	
		Vector3 scale = new Vector3(
				boxSize.x * transformScale.x,
				boxSize.y * transformScale.y,
				boxSize.z * transformScale.z
			);
		if(isTerrainOnRotatedQuadrant(terrain))
			return flipXZ(scale);
		else
			return scale;
	}
	
	private Vector3 findTrueTerrainCenter(GameObject terrain){
		Vector3 boxCenter = terrain.GetComponent<LevelGeometry>().getTrueBoxColliderCenter();
		Vector3 transformScale = terrain.transform.lossyScale;	
		
		Vector3 center = 	(Vector3.right * boxCenter.x * transformScale.x) +
							(Vector3.up * boxCenter.y * transformScale.y)+
							(Vector3.forward * boxCenter.z * transformScale.z);
		
		switch(getTerrainRotationQuadrant(terrain)){
			case 0:	return new Vector3( center.x, center.y,  center.z);
			case 1: return new Vector3( center.z, center.y, -center.x);
			case 2:	return new Vector3(-center.x, center.y, -center.z);
			case 3: return new Vector3(-center.z, center.y,  center.x);
			default:
				throw new System.ArgumentException("Invalid rotation value, must be (0-3)");
		}
	}
	
	private bool isTerrainOnRotatedQuadrant(GameObject terrain){
		return getTerrainRotationQuadrant(terrain) % 2 == 1;
	}
	
	private int getTerrainRotationQuadrant(GameObject terrain){
		return (int)Mathf.Round((float)(terrain.transform.rotation.eulerAngles.y / 90.0));
	}
	
	private void CreateEdge(GameObject terrain, Vector3 top, Vector3 offsetDir, float offsetMag, Vector3 terrainScale){
		GameObject edge = Instantiate(edgePrefab, top + offsetMag * offsetDir, Quaternion.identity) as GameObject;
		
		int or = 0;
		if(offsetDir == Vector3.right) 	or = 0;
		if(offsetDir == Vector3.forward) or = 1;
		if(offsetDir == Vector3.left) 	or = 2;
		if(offsetDir == Vector3.back) 	or = 3;
		
		edge.GetComponent<Edge>().Init(or, terrainScale.x, terrainScale.z, 
			index,terrain.transform,isTerrainOnRotatedQuadrant(terrain));
			
		index++;
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
		if(!overlap){
			return null;
		}else{
			return region;
		}
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
