using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour {
	//suppress warnings
	#pragma warning disable 162

	
	int or = 0; //orientation of game object (0-3: right,back,left,front)
	
	int overlapIndex; //how many of the overlaps that we've checked

	Vector3[] cuboid;

	byte status = 0; //0: no overlap, 1: lined up, 2: latched, 3: rested latch

	bool validIn2D = true;

	PlayerController player;
	
	bool init = false;//indicates whether Edge has been initiated
	
	bool playerAboveEdge = false;
	
	public string edgeIndex = "n/a";

	float hangThresh = 35;//time to wait till player can climb
	float hangCounter = 0.0f;
	
	float edgeFactor = .5f;//factor by which top check is smaller

	public void FixedUpdate(){
		if(!init)
			return;
		bool playerCanGrab = player.isFalling() && (player.is3D() || validIn2D);
		//if locked on
		if(status >= 2){
			//check if rested latch can be entered
			hangCounter++;
			if(status == 2 && hangCounter > hangThresh/*&& !GrabButtonDown()*/)
				status = 3;
			//if player is trying to let go
			if(ReleaseButtonDown()){
				status = 0;
				player.UpdateEdgeState(this, status, 4);
			}
			//if player is trying to get up
			if(status == 3 && GrabButtonDown() && SpaceAboveFree()){
				Vector3 playerPos = player.gameObject.transform.position;
				Vector3 playerScale = player.gameObject.transform.localScale;
				playerPos.y += playerScale.y;
				float offsetScale = .5f;
				switch(or){
				case 0: playerPos.x -= playerScale.x * offsetScale; break;
				case 1: playerPos.z -= playerScale.z * offsetScale; break;
				case 2: playerPos.x += playerScale.x * offsetScale; break;
				case 3: playerPos.z += playerScale.z * offsetScale; break;
				}
				player.gameObject.transform.position = playerPos;
				status = 0;
				player.UpdateEdgeState(this, status, 5);
			}
		//if player is overlapping
		}else if(playerCanGrab && isOverlaping(cuboid, player.getCuboid())/* && GrabButtonDown()*/){
			Vector3 playerPos = player.gameObject.transform.position;
			float diff = 0;
			if(or%2 == 0)
				diff = Mathf.Abs(playerPos.x - transform.position.x);
			else
				diff = Mathf.Abs(playerPos.z - transform.position.z);
			if(diff < .0001f){
				if(status == 0){
					playerAboveEdge = player.getCuboid()[1].y > cuboid[1].y;
					status = 1;
					player.UpdateEdgeState(this, status);
				}else if (status == 1 && playerAboveEdge != (player.getCuboid()[1].y > cuboid[1].y)){
					status = 2;
					hangCounter = 0;
					player.UpdateEdgeState(this,status);
				}
			}
		//if nothing is overlapping
		}else{
			status = 0;
			player.UpdateEdgeState(this, status);
		}

		//check for making the player let go
		if(status != 0 && !player.is3D() && !validIn2D){
			status = 0;
			player.UpdateEdgeState(this, status);
		}	
	}

	public bool GrabButtonDown(){
		switch(or){
		case 0: return (InputManager.instance.GetForwardMovement() < 0); break;
		case 1: return (InputManager.instance.GetSideMovement() > 0); break;
		case 2: return (InputManager.instance.GetForwardMovement() > 0); break;
		case 3: return (InputManager.instance.GetSideMovement() < 0); break;
		default: return false;
		}
	}

	public bool ReleaseButtonDown(){
		switch(or){
		case 0: return (InputManager.instance.GetForwardMovement() > 0); break;
		case 1: return (InputManager.instance.GetSideMovement() < 0); break;
		case 2: return (InputManager.instance.GetForwardMovement() < 0); break;
		case 3: return (InputManager.instance.GetSideMovement() > 0); break;
		default: return false;
		}
	}

	public bool isOverlaping(Vector3[] c1, Vector3[] c2){
		bool ans = true;
		for(int i = 0; i < 3; i++){
			if(i == 2 && !player.is3D())
				continue;
			ans = ans && c1[0][i] <= c2[1][i] && c2[0][i] <= c1[1][i];
		}
		return ans;
	}

	//inits edge
	//args0: orientation of game object (0-3: right,back,left,front)
	//args1: width of terrain
	//args2: depth of terrain
	//args3: bool[] showing overlapping terrains
	//args4: how many of the terrains have been checked
	public void Init(int or, float width, float depth, int overlapIndex, string edgeIndex){
		//set index
		this.edgeIndex = edgeIndex;
		//set parent
		transform.parent = EdgeManager.instance.transform;
		//init player reference
		player = PlayerController.instance;
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

		//init cubiod
		Vector3 halfScale = gameObject.transform.localScale * .5f;
		cuboid = new Vector3[2];
		cuboid[0] = gameObject.transform.position - halfScale;
		cuboid[1] = gameObject.transform.position + halfScale;
		
		//initialization has been completed
		init = true;
	}

	public void Init(int or, float width, float depth, string edgeIndex){
		Init(or, width, depth, 0, edgeIndex);
	}

	bool SpaceAboveFree(){
		//reference to terrain in edgemanager
		GameObject[] t = EdgeManager.instance.getTerrain();
		//generate hitbox
		Vector3[] checkBox = new Vector3[2];
		Vector3 playerPos = PlayerController.instance.gameObject.transform.position;
		Vector3 playerScale = PlayerController.instance.gameObject.transform.localScale;
		checkBox[0] = playerPos + new Vector3(0,playerScale.y *  .5f,0);
		checkBox[1] = playerPos + new Vector3(0,playerScale.y * 1.5f,0);
		float width = playerScale.x;
		switch(or){
		case 0://right
			checkBox[0] += new Vector3(-width,0,0);
			checkBox[1] += new Vector3(0,0,0);
			break;
		case 1://back
			checkBox[0] += new Vector3(0,0,-width);
			checkBox[1] += new Vector3(0,0,0);
			break;
		case 2://left
			checkBox[0] += new Vector3(0,0,0);
			checkBox[1] += new Vector3(width,0,0);
			break;
		case 3://up
			checkBox[0] += new Vector3(0,0,0);
			checkBox[1] += new Vector3(0,0,width);
			break;
		}
		
		//find overlaps
		for(int i = overlapIndex; i < t.Length; i++){
			if(PlayerController.instance.is3D()){
				Vector3[] boxOverlap = EdgeManager.instance.GetOverlap(i,checkBox);
				if(boxOverlap != null)
					return false;
			}else{
				if(EdgeManager.instance.CheckOverlap2D(i,checkBox))
					return false;
			}
		}
		
		//if no overlaps are found
		return true;
	}
	
	
	
	public void checkOverlaps(){
		//reference to terrain in edgemanager
		GameObject[] t = EdgeManager.instance.getTerrain();
		//create corners
		Vector3[] cubBot = new Vector3[2];
		Vector3 halfScale = gameObject.transform.localScale * .5f;
		cubBot[0] = gameObject.transform.position - halfScale;
		cubBot[1] = gameObject.transform.position + halfScale;
		Vector3[] cubTop = new Vector3[2];
		float edgeSize = gameObject.transform.localScale.y;
		float smallEdge = edgeSize * edgeFactor;
		switch(or){
		case 0:
			cubTop[0] = cubBot[0] + new Vector3(-smallEdge,edgeSize,0);
			cubTop[1] = cubBot[1] + new Vector3(0,smallEdge,0);
			break;
		case 1:
			cubTop[0] = cubBot[0] + new Vector3(0,edgeSize,-smallEdge);
			cubTop[1] = cubBot[1] + new Vector3(0,smallEdge,0);
			break;
		case 2:
			cubTop[0] = cubBot[0] + new Vector3(0,edgeSize,0);
			cubTop[1] = cubBot[1] + new Vector3(smallEdge,smallEdge,0);
			break;
		case 3:
			cubTop[0] = cubBot[0] + new Vector3(0,edgeSize,0);
			cubTop[1] = cubBot[1] + new Vector3(0,smallEdge,smallEdge);
			break;
		}

		//find overlaps
		for(int i = overlapIndex; i < t.Length; i++){
			Vector3[] overBot = EdgeManager.instance.GetOverlap(i,cubBot);
			Vector3[] overTop = EdgeManager.instance.GetOverlap(i,cubTop);
			if(overBot == null && overTop == null){
				continue;
			}
			//find range of overlap
			float min = 0;
			float max = 0;
			if(overBot != null){
				if(or%2==0){
					min = overBot[0][2];
					max = overBot[1][2];
				}else{
					min = overBot[0][0];
					max = overBot[1][0];
				}
			}
			if(overTop != null){
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
					newEdge.GetComponent<Edge>().Init(or, 0, newDepth, overlapIndex, 
						"split_"+EdgeManager.instance.getGlobalIndex());
				//shorten current edge
				float myZ = (min + posZ - halfScaleZ) * .5f;
				Vector3 myPos = gameObject.transform.position;
				myPos.z = myZ;
				gameObject.transform.position = myPos;
				float myDepth = min - (posZ - halfScaleZ);
				if(myDepth <= 0){
					Destroy(this.gameObject);
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
					newEdge.GetComponent<Edge>().Init(or, newWidth, 0, overlapIndex,
						"split_"+EdgeManager.instance.getGlobalIndex());
				//shorten current edge
				float myX = (min + posX - halfScaleX) * .5f;
				Vector3 myPos = gameObject.transform.position;
				myPos.x = myX;
				gameObject.transform.position = myPos;
				float myWidth = min - (posX - halfScaleX);
				if(myWidth <= 0){
					Destroy(this.gameObject);
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

		//determine valid in 2D
		if(or%2 == 1)
			validIn2D = false;
		else{
			for(int i = overlapIndex; i < t.Length; i++){
				bool overBot = EdgeManager.instance.CheckOverlap2D(i,cubBot);
				bool overTop = EdgeManager.instance.CheckOverlap2D(i,cubTop);
				validIn2D = validIn2D && !overBot && !overTop;
			}
		}
	}

	public void resetStatus(){
		status = 0;
	}

	public int getOrientation(){
		return or;
	}
}
