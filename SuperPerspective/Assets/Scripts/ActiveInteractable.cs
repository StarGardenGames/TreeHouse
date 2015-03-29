using UnityEngine;
using System.Collections;

//can be activated by player
public class ActiveInteractable : Interactable {
	
	//suppress warnings
	#pragma warning disable 414
	
	//keeps track of notification marker
	static NotificationController notiMarker;
	
	//whether notification is shown
	static bool notiShown = false;
	static float notiDist = -1;
	static ActiveInteractable selected = null;
	
	//variables to determine whether player can trigger
	bool inRange = false;
	bool playerFacing = false;
	bool canTrigger = false;
	
	//distance for inRange
	float range = 3.0f;
	
	public override void Start(){
		base.Start();
		//find notification marker
		notiMarker = player.transform.Find("Notification").GetComponent<NotificationController>();
		//disable it so it will be invisible
		notiMarker.updateVisible(notiShown);
		//register interactpressed to the InputManager
		InputManager.instance.InteractPressed += InteractPressed;
	}
		
	public override void FixedUpdate(){
		base.FixedUpdate();
		//check distance and determine if range methods need to be called
		float dist = Vector3.Distance(transform.position, player.transform.position);
		//update inRange
		inRange = dist < range;
		//update player facing
		playerFacing = true;
		//update canTrigger
		canTrigger = inRange && playerFacing;
		//update notiShown
		if(canTrigger && (!notiShown || dist < notiDist)){
			selected = this;
			notiShown = true;
			notiMarker.updateVisible(true);
			notiDist = dist;
		}
	}
	
	void LateUpdate(){
		//make notification invisible if no interactables could trigger it
		if(notiShown == false){
			notiMarker.updateVisible(false);
			selected = null;
		}
		//prepare for next frame
		notiShown = false;
	}
	
	void InteractPressed(){
		if(selected == this)
			Triggered();
	}
	
	public virtual void Triggered(){}
}