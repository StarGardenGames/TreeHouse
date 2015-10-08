using UnityEngine;
using System.Collections;

//can be activated by player
public class ActiveInteractable : PhysicalObject {
	
	//suppress warnings
	#pragma warning disable 414

	public bool ignoreYDistance;

	//player
	protected GameObject player;
	
	//main ActiveInteractable
	static ActiveInteractable main;
	
	//keeps track of notification marker
	static NotificationController notiMarker;
	
	//whether notification is shown
	static bool notiShown = false;
	static float notiDist = -1;
	static ActiveInteractable selected = null;
	static int frameCount = 0;
	
	//used to create a fixedlateupdate effect
	bool fixedCalled = false;
	
	//distance for inRange
	protected float range = 3f;
	
	//how much error there can be in the angle for it to be valid
	float angleBuffer = 80;
	
	void Start(){
		StartSetup ();
	}
		
	void FixedUpdate(){
		FixedUpdateLogic ();
	}
	
	void LateUpdate(){
		LateUpdateLogic ();
	}
	
	void InteractPressed(){
		if(!GameStateManager.IsGamePaused() && selected == this)
			Triggered();
	}
	
	public virtual void Triggered(){}

	protected void StartSetup() {
		base.Init();
		//find player
		player = PlayerController.instance.gameObject;
		//become main if no one else has become it yet
		if(main == null)
			main = this;
		//perform static actions
		if(main == this){
			//find notification marker
			notiMarker = player.transform.Find("Notification").GetComponent<NotificationController>();
			//disable it so it will be invisible
			notiMarker.updateVisible(notiShown);
		}
		//register interactpressed to the InputManager
		InputManager.instance.InteractPressedEvent += InteractPressed;
	}

	protected void FixedUpdateLogic() {
		float dist = GetDistance();
		
		bool inRange = dist < range;

		bool playerFacing = isPlayerFacingObject();
			
		bool inYRange = yRangeOverlapsWithPlayer();
	
		bool aa = (GetComponentInChildren<Renderer>().enabled || GetComponent<Door>());
		
		bool unlockable = ((this.gameObject.GetComponent<LockedDoor>() == null || Key.GetKeysHeld() > 0));
		
		bool canTrigger = 
			aa && inRange && (playerFacing || !GameStateManager.is3D()) && inYRange && unlockable;
		
		bool notificationCanBeShown = !notiShown || dist < notiDist;
		
		//update notiShown
		if(canTrigger && notificationCanBeShown){
			selected = this;
			notiShown = true;
			notiMarker.updateVisible(true);
			notiDist = dist;
		}
		
		fixedCalled = true;
	}
	
	public virtual float GetDistance() {	
		switch (GetQuadrant ()) {
			case Quadrant.xPlus:
					return player.transform.position.x - transform.position.x;
			case Quadrant.xMinus:
					return transform.position.x - player.transform.position.x;
			case Quadrant.zPlus:
					return player.transform.position.z - transform.position.z;
			case Quadrant.zMinus:
					return transform.position.z - player.transform.position.z;
            default:
                    return float.MaxValue;
		}
	}
	
	protected Quadrant GetQuadrant() {
		float colliderWidth = GetComponent<Collider>().bounds.size.x;
		float colliderDepth = GetComponent<Collider>().bounds.size.z;
		if (Mathf.Abs(player.transform.position.x - transform.position.x) > colliderWidth / 2 || GameStateManager.is2D()) {
			if (Mathf.Abs(player.transform.position.z - transform.position.z) <= colliderDepth)
			{
				 if (player.transform.position.x - transform.position.x > 0)
					  return Quadrant.xPlus;
				 else
					  return Quadrant.xMinus;
			}
		} else if (Mathf.Abs(player.transform.position.z - transform.position.z) > colliderDepth / 2) {
			if (Mathf.Abs(player.transform.position.x - transform.position.x) <= colliderWidth)
			{
				 if (player.transform.position.z - transform.position.z > 0)
					  return Quadrant.zPlus;
				 else
					  return Quadrant.zMinus;
			}
		}
		return Quadrant.none;
	}
	
	private bool isPlayerFacingObject(){
		float playerOrientation = player.GetComponent<PlayerController>().getOrientation();
		playerOrientation = (playerOrientation + 360) % 360;
		
		//calculate angle between interactable and player
		float playerAngle = Vector2.Angle(new Vector2(transform.position.x - player.transform.position.x,
		                                              transform.position.z - player.transform.position.z),Vector2.up);
																	 
		bool inFrontOfPlayer = transform.position.x < player.transform.position.x;
		if(inFrontOfPlayer){
			playerAngle = 360 - playerAngle;
		}

		//calculate difference and modify so that it's in the correct range 
		float angleDiff = Mathf.Abs(playerOrientation - playerAngle);
		angleDiff += 360;
		angleDiff %= 360;
		if(angleDiff > 180)
			angleDiff = 360 - angleDiff;
		//determine whether player is facing interactable
		return angleDiff < angleBuffer;
	}

	private bool yRangeOverlapsWithPlayer(){
		float colliderBot = GetComponent<Collider>().bounds.min.y + 0.05f;
		float colliderTop = GetComponent<Collider>().bounds.max.y - 0.05f;
		float playerBot = player.GetComponent<Collider>().bounds.min.y;
		float playerTop = player.GetComponent<Collider>().bounds.max.y;
		
		bool rangesOverlap = (colliderTop > playerBot && colliderBot < playerTop);
		
		return rangesOverlap || !ignoreYDistance;
	}
	
	protected void LateUpdateLogic() {
		//perform static actions
		if(main == this && fixedCalled){
			frameCount++;
			//make notification invisible if no interactables could trigger it
			if(!notiShown){
				notiMarker.updateVisible(false);
				selected = null;
			}
			//prepare for next frame
			notiShown = false;
			
			fixedCalled = false;
		}
	}

	public enum Quadrant {
		zPlus, zMinus, xPlus, xMinus, none
	}
}