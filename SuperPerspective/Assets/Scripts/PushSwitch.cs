using UnityEngine;
using System.Collections;

public class PushSwitch : PlayerInteractable {

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool enabled = false; //whether switch is currently enabled

	float distThresh = 1.5f; //distance threshhold where it will become unpressed
	GameObject player; //stores player gameobject
		//NOTE: used to calculate distance to determine if it's pressed

	void Start(){
		player = GameObject.Find("NewPlayer");//needs to be updated depending on player name
	}

	void Update(){
		//calculate distance between switch and player
		float dist = Vector3.Distance(transform.position, player.transform.position);
		//become unpressed if distance is small enough
		if(dist > distThresh && enabled){
			enabled = false;
			//update activatables
			foreach(Activatable o in triggers)
				o.setActivated(enabled);
		}
		//update color for debugging
		if(enabled)
			gameObject.renderer.material.color = Color.white;
		else
			gameObject.renderer.material.color = Color.red;
	}

	public override void CollisionWithPlayer(){
		enabled = true;//becomes enabled when it collides with player
		//enabled is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(enabled);
	}
}
