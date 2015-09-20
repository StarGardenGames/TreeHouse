﻿using UnityEngine;
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
	
	//variables to determine whether player can trigger
	bool inRange = false;
	bool playerFacing = false;
	bool canTrigger = false;
	
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
		if(selected == this)
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

	protected Quadrant GetQuadrant() {
		float colliderWidth = GetComponent<Collider>().bounds.size.x;
		PerspectiveType persp = GameStateManager.instance.currentPerspective;
		if (Mathf.Abs(player.transform.position.x - transform.position.x) > colliderWidth / 2 || persp == PerspectiveType.p2D) {
			if (player.transform.position.x - transform.position.x > 0)
				return Quadrant.xPlus;
			else
				return Quadrant.xMinus;
		} else {
			if (player.transform.position.z - transform.position.z > 0)
				return Quadrant.zPlus;
			else
				return Quadrant.zMinus;
		}
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
		}
		return 0;
	}

	protected void FixedUpdateLogic() {
		//check distance and determine if range methods need to be called
		float dist = 0;
		bool is3D = player.GetComponent<PlayerController>().is3D();
		if(is3D) {
			if (ignoreYDistance) {
				dist = Vector2.Distance(new Vector2(transform.position.x,transform.position.z),
			               new Vector2(player.transform.position.x, player.transform.position.z));
			} else {
				dist = Vector3.Distance(transform.position, player.transform.position);
			}
		} else {
			if (ignoreYDistance) {
				dist = Mathf.Abs(transform.position.x - player.transform.position.x);
			} else {
				dist = Vector2.Distance(new Vector2(transform.position.x,transform.position.y),
				           new Vector2(player.transform.position.x, player.transform.position.y));
			}
		}
		//update inRange
		inRange = GetDistance() < range;
		//update player facing
		//get orientation from player
		float playerOrientation = player.GetComponent<PlayerController>().getOrientation();
		//calculate angle between interactable and player
		float playerAngle = Vector2.Angle(new Vector2(transform.position.x - player.transform.position.x,
		                                              transform.position.z - player.transform.position.z),Vector2.up);
		float playerAngle2 = Vector2.Angle(new Vector2(transform.position.x - player.transform.position.x,
		                                               transform.position.z - player.transform.position.z),Vector2.right);
		//adjust angle to be between 0 and 360
		if(90 < playerAngle2 && playerAngle2 < 270)
			playerAngle = 360 - playerAngle;
		//calculate difference and modify so that it's in the correct range 
		float angleDiff = Mathf.Abs(playerOrientation - playerAngle);
		angleDiff += 360;
		angleDiff %= 360;
		if(angleDiff > 180)
			angleDiff = 360 - angleDiff;
		//determine whether player is facing interactable
		playerFacing = angleDiff < angleBuffer;
		//update canTrigger
		canTrigger = (GetComponentInChildren<Renderer>().enabled || GetComponent<Door>()) && inRange && (playerFacing || !is3D) &&
			((GetComponent<Collider>().bounds.max.y - 0.05f > player.GetComponent<Collider>().bounds.min.y && GetComponent<Collider>().bounds.min.y + 0.05f < player.GetComponent<Collider>().bounds.max.y) || !ignoreYDistance);
		//update notiShown
		if(canTrigger && (!notiShown || dist < notiDist) && ((this.gameObject.GetComponent<LockedDoor>() == null || Key.GetKeysHeld() > 0))){
			selected = this;
			notiShown = true;
			notiMarker.updateVisible(true);
			notiDist = dist;
		}
		
		fixedCalled = true;
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
		zPlus, zMinus, xPlus, xMinus
	}
}