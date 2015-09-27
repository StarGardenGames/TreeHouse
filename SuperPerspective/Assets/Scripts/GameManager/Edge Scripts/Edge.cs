using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour {
	//suppress warnings
	#pragma warning disable 162, 649, 219, 414
	
	int or = 0; //orientation of game object (0-3: right,back,left,front)
	
	int overlapIndex; //how many of the overlaps that we've checked

	Vector3[] cuboid;

	byte status = 0; //0: no overlap, 1: lined up, 2: latched, 3: rested latch

	bool playerAboveEdge = false;
	
	PlayerController player;
	
	bool init = false;//indicates whether Edge has been initiated
	
	bool rotatedTerrain = false;
	
	public int edgeIndex = 0;
	
	float edgeFactor = .5f;
	
	public bool validIn2D = true;

	float hangThresh = 35;//time to wait till player can climb
	float hangCounter = 0.0f;
	
	public void FixedUpdate(){
		if(!init)
			return;
		updateCuboid();
		
		bool is3D = GameStateManager.instance.currentPerspective == PerspectiveType.p3D;
		bool playerCanGrab = player.isFalling() && (is3D || validIn2D);
		//if locked on
		if(status >= 2){
			//check if rested latch can be entered
			hangCounter++;
			if(status == 2 && hangCounter > hangThresh)
				status = 3;
			//if player is trying to let go
			if(ReleaseButtonDown()){
				status = 0;
				player.UpdateEdgeState(this, status, 4);
			}
			//if player is trying to get up
			if(status == 3 && GrabButtonDown() && SpaceAboveFree()){
				Vector3 playerPos = player.gameObject.transform.position;
				playerPos.y += player.colliderHeight;
				float offsetScale = .5f;
				switch(or){
				case 0: playerPos.x -= player.colliderWidth * offsetScale; break;
				case 1: playerPos.z -= player.colliderDepth * offsetScale; break;
				case 2: playerPos.x += player.colliderWidth * offsetScale; break;
				case 3: playerPos.z += player.colliderDepth * offsetScale; break;
				}
				player.gameObject.transform.position = playerPos;
				status = 0;
				player.UpdateEdgeState(this, status, 5);
			}
		//if player is overlapping
		}else if(playerCanGrab && isOverlaping(cuboid, player.getCuboid())){
			Vector3 playerPos = player.gameObject.transform.position;
			float diff = 0;
			if(or%2 == 0)
				diff = Mathf.Abs(playerPos.x - transform.position.x);
			else
				diff = Mathf.Abs(playerPos.z - transform.position.z);
			if(diff < .1f){
				if(status == 0){
					playerAboveEdge = player.getCuboid()[1].y > cuboid[1].y;
					status = 1;
					player.UpdateEdgeState(this, status);
				}else if(status == 1 && playerAboveEdge != (player.getCuboid()[1].y > cuboid[1].y)){
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
		if(status != 0 && !is3D && !validIn2D){
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
	public void Init(int or, float width, float depth, int overlapIndex, int edgeIndex, Transform parent, 
		bool rotatedTerrain){
		this.edgeIndex = edgeIndex;
		player = PlayerController.instance;
		this.or = or; //orientation
		this.overlapIndex = overlapIndex;
		this.rotatedTerrain = rotatedTerrain;
	
		checkOverlaps();
		
		transform.parent = parent;
		
		shortenSides();
		
		updateDimensions(or,width,depth);
		
		cuboid = new Vector3[2];
		updateCuboid();
		
		//initialization has been completed
		init = true;
	}

	public void Init(int or, float width, float depth, int edgeIndex, Transform transform, bool rotatedTerrain){
		Init(or, width, depth, 0, edgeIndex, transform, rotatedTerrain);
	}

	bool SpaceAboveFree(){
		//reference to terrain in edgemanager
		GameObject[] t = EdgeManager.instance.getTerrain();
		//generate hitbox
		Vector3[] checkBox = new Vector3[2];
		Vector3 playerPos = PlayerController.instance.gameObject.transform.position;
		PlayerController player = PlayerController.instance;
		checkBox[0] = playerPos + new Vector3(0,player.colliderHeight *  .5f,0);
		checkBox[1] = playerPos + new Vector3(0,player.colliderHeight * 1.5f,0);
		float width = player.colliderWidth;
		float halfDepth = player.colliderDepth * .5f;
		switch(or){
		case 0://right
			checkBox[0] += new Vector3(-width,0,-halfDepth);
			checkBox[1] += new Vector3(0,0,halfDepth);
			break;
		case 1://back
			checkBox[0] += new Vector3(-halfDepth,0,-width);
			checkBox[1] += new Vector3(halfDepth,0,0);
			break;
		case 2://left
			checkBox[0] += new Vector3(0,0,-halfDepth);
			checkBox[1] += new Vector3(width,0,halfDepth);
			break;
		case 3://up
			checkBox[0] += new Vector3(-halfDepth,0,0);
			checkBox[1] += new Vector3(halfDepth,0,width);
			break;
		}
		
		//find overlaps
		for(int i = overlapIndex; i < t.Length; i++){
			if(GameStateManager.instance.currentPerspective == PerspectiveType.p3D){
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
	
	void updateCuboid(){
		Vector3 parScale = gameObject.transform.parent.transform.lossyScale;
		Vector3 halfScale = gameObject.transform.localScale * .5f;
		halfScale.x *= parScale.x;
		halfScale.y *= parScale.y;
		halfScale.z *= parScale.z;
		cuboid[0] = gameObject.transform.position - halfScale;
		cuboid[1] = gameObject.transform.position + halfScale;
	}
	
	private void updateDimensions(int or, float width, float depth){
		Vector3 scale = gameObject.transform.localScale;
		Vector3 parScale = findAdjustedParentScale();
		
		if(or%2 == 0)//if left or right
			scale.z = depth / parScale.z;
		else//if front or back
			scale.x = width / parScale.x;
		gameObject.transform.localScale = scale;
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
		/*for(int i = overlapIndex; i < t.Length; i++){
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
						EdgeManager.instance.getGlobalIndex(),transform.parent,rotatedTerrain);
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
						EdgeManager.instance.getGlobalIndex(),transform.parent,rotatedTerrain);
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
		}*/

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
	
	private void shortenSides(){
		Vector3 lossyScale = gameObject.transform.lossyScale;
		Vector3 localScale = gameObject.transform.localScale;
		Vector3 parentScale = findAdjustedParentScale();
		if(or%2==0){//left/right
			if(lossyScale.z >= 1)
				localScale.z -= 1f / parentScale.z;
			else
				Destroy(this);
		}else{//back/front
			if(lossyScale.x >= 1)
				localScale.x -= 1f / parentScale.x;
			else
				Destroy(this);
		}
		gameObject.transform.localScale = localScale;
	}
	
	private Vector3 findAdjustedParentScale(){
		Vector3 parScale = gameObject.transform.parent.lossyScale;
		if(rotatedTerrain){
			float t = parScale.x;
			parScale.x = parScale.z;
			parScale.z = t;
		}
		return parScale;
	}
	
	public void resetStatus(){
		status = 0;
	}

	public int getOrientation(){
		return or;
	}
}
