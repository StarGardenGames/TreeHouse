using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour {

	int or = 0; //orientation of game object (0-3: right,back,left,front)
	
	int overlapIndex; //how many of the overlaps that we've checked


	public void Init(int or, float width, float depth){
		Init(or, width, depth, 0);

	}
	//inits edge
	//args0: orientation of game object (0-3: right,back,left,front)
	//args1: width of terrain
	//args2: depth of terrain
	//args3: bool[] showing overlapping terrains
	//args4: how many of the terrains have been checked
	public void Init(int or, float width, float depth, int overlapIndex){
		//scale
		Vector3 scale = gameObject.transform.localScale;
		if(or%2 == 0)//if left or right
			scale.z = depth;
		else//if front or back
			scale.x = width;
		gameObject.transform.localScale = scale;

		//orientation
		this.or = or;

		//overlapIndex
		this.overlapIndex = overlapIndex;

		//overlaps
		checkOverlaps();
	}

	public void checkOverlaps(){
		//reference to terrain in edgemanager
		GameObject[] t = EdgeManager.instance.terrain;
		//create corners
		Vector3[] cubBot = new Vector3[2];
		Vector3 halfScale = gameObject.transform.localScale * .5f;
		cubBot[0] = gameObject.transform.position - halfScale;
		cubBot[1] = gameObject.transform.position + halfScale;
		Vector3[] cubTop = new Vector3[2];
		float edgeSize = gameObject.transform.localScale.y;
		switch(or){
		case 0:
			cubTop[0] = cubBot[0] + new Vector3(-edgeSize,edgeSize,0);
			cubTop[1] = cubBot[1] + new Vector3(0,edgeSize,0);
			break;
		case 1:
			cubTop[0] = cubBot[0] + new Vector3(0,edgeSize,-edgeSize);
			cubTop[1] = cubBot[1] + new Vector3(0,edgeSize,0);
			break;
		case 2:
			cubTop[0] = cubBot[0] + new Vector3(0,edgeSize,0);
			cubTop[1] = cubBot[1] + new Vector3(edgeSize,edgeSize,0);
			break;
		case 3:
			cubTop[0] = cubBot[0] + new Vector3(0,edgeSize,0);
			cubTop[1] = cubBot[1] + new Vector3(0,edgeSize,edgeSize);
			break;
		}
		/*if(or == 2){
		Debug.Log("["+cubBot[0].x+","+cubBot[0].y+","+cubBot[0].z+"] __ ["+cubBot[1].x+","+cubBot[1].y+","+cubBot[1].z+"]");
		Debug.Log("["+cubTop[0].x+","+cubTop[0].y+","+cubTop[0].z+"] __ ["+cubTop[1].x+","+cubTop[1].y+","+cubTop[1].z+"]");
		}
		string name = "";
		switch(or){
		case 0: name = "right"; break;
		case 1: name = "back"; break;
		case 2: name = "left"; break;
		case 3: name = "front"; break;
		}*/
		//find overlaps
		for(int i = overlapIndex; i < t.Length; i++){
			Vector3[] overBot = EdgeManager.instance.GetOverlap(i,cubBot);
			Vector3[] overTop = EdgeManager.instance.GetOverlap(i,cubTop);
			if(overBot == null && overTop == null){
				//Debug.Log("--["+name+"] No Overlaps with "+i);
				continue;
			}
			//find range of overlap
			float min = 0;
			float max = 0;
			if(overBot != null){
				//Debug.Log("--["+name+"]Overlap Bot with "+i);
				if(or%2==0){
					min = overBot[0][2];
					max = overBot[1][2];
				}else{
					min = overBot[0][1];
					max = overBot[1][1];
				}
			}
			if(overTop != null){
				//Debug.Log("--["+name+"]Overlap Top with "+i);
				//Debug.Log("["+overBot[0].x+","+overBot[0].y+","+cubBot[0].z+"] __ ["+cubBot[1].x+","+cubBot[1].y+","+cubBot[1].z+"]");
				//Debug.Log("["+overTop[0].x+","+overTop[0].y+","+overTop[0].z+"] __ ["+overTop[1].x+","+overTop[1].y+","+overTop[1].z+"]");
				if(or%2==0){
					min = ((overBot == null)? overTop[0][2] : Mathf.Min(overTop[0][2],overBot[0][2]));
					max = ((overBot == null)? overTop[1][2] : Mathf.Max(overTop[1][2],overBot[1][2]));
				}else{
					min = ((overBot == null)? overTop[0][0] : Mathf.Min(overTop[0][0],overBot[0][0]));
					max = ((overBot == null)? overTop[1][0] : Mathf.Max(overTop[1][0],overBot[1][0]));
				}
			}
			//shorter current edge and create a new edge
			if(or%2 == 0){//left/right
				//reference variables
				float posZ = gameObject.transform.position.z;
				float halfScaleZ = gameObject.transform.localScale.z *.5f;
				//create new edge
				float newZ = (max + posZ + halfScaleZ) * .5f;
				Vector3 newPos = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,newZ);
				GameObject newEdge = Instantiate(EdgeManager.instance.edgePrefab, newPos, Quaternion.identity) as GameObject;
				float newDepth = posZ + halfScaleZ - max;
				if(newDepth <=0)
					Destroy(newEdge);
				else
					newEdge.GetComponent<Edge>().Init(or, 0, newDepth, overlapIndex);
				//shorten current edge
				float myZ = (min + posZ - halfScaleZ) * .5f;
				Vector3 myPos = gameObject.transform.position;
				myPos.z = myZ;
				gameObject.transform.position = myPos;
				float myDepth = min - (posZ - halfScaleZ);
				if(myDepth <= 0){
					Destroy(this);
					break;
				}else{
					Vector3 myScale = gameObject.transform.localScale;
					myScale.z = myDepth;
					gameObject.transform.localScale = myScale;
				}
			}else{//front/back
				//reference variables
				float posX = gameObject.transform.position.x;
				float halfScaleX = gameObject.transform.localScale.x *.5f;
				//create new edge
				float newX = (max + posX + halfScaleX) * .5f;
				Vector3 newPos = new Vector3(newX,gameObject.transform.position.y,gameObject.transform.position.z);
				GameObject newEdge = Instantiate(EdgeManager.instance.edgePrefab, newPos, Quaternion.identity) as GameObject;
				float newWidth = posX + halfScaleX - max;
				if(newWidth <= 0)
					Destroy(newEdge);
				else
					newEdge.GetComponent<Edge>().Init(or, newWidth, 0, overlapIndex);
				//shorten current edge
				float myX = (min + posX - halfScaleX) * .5f;
				Vector3 myPos = gameObject.transform.position;
				myPos.x = myX;
				gameObject.transform.position = myPos;
				float myWidth = min - (posX - halfScaleX);
				if(myWidth <= 0){
					Destroy(this);
					break;
				}else{
					Vector3 myScale = gameObject.transform.localScale;
					myScale.x = myWidth;
					gameObject.transform.localScale = myScale;
				}
			}
		}
		//shorten both sides so that player only grabs when fully on the edge
		if(or%2==0){//left/right
			if(gameObject.transform.localScale.z >= 1){
				Vector3 scale = gameObject.transform.localScale;
				scale.z -= 1;
				gameObject.transform.localScale = scale;
			}else
				Destroy(this);
		}else{//back/front
			if(gameObject.transform.localScale.x >= 1){
				Vector3 scale = gameObject.transform.localScale;
				scale.x -= 1;
				gameObject.transform.localScale = scale;
			}else
				Destroy(this);
		}
	}
}
