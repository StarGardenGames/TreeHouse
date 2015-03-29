using UnityEngine;
using System.Collections;

//can be activated by player
public class ActiveInteractable : Interactable {
	
	//suppress warnings
	#pragma warning disable 414
	
	//main ActiveInteractable
	static ActiveInteractable main;
	
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
		InputManager.instance.InteractPressed += InteractPressed;
	}
		
	public override void FixedUpdate(){
		base.FixedUpdate();
		//check distance and determine if range methods need to be called
		float dist = 0;
		if(player.GetComponent<PlayerController3>().is3D())
			dist = Vector3.Distance(transform.position, player.transform.position);
		else
			dist = Vector2.Distance(new Vector2(transform.position.x,transform.position.y),
				new Vector2(player.transform.position.x, player.transform.position.y));
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
		//perform static actions
		if(main == this){
			//make notification invisible if no interactables could trigger it
			if(!notiShown){
				notiMarker.updateVisible(false);
				selected = null;
			}
			//prepare for next frame
			notiShown = false;
		}
	}
	
	void InteractPressed(){
		if(selected == this)
			Triggered();
	}
	
	public virtual void Triggered(){}
}