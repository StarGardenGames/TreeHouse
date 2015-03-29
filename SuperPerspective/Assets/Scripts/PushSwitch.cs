using UnityEngine;
using System.Collections;

public class PushSwitch : Interactable {

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pushed = false; //whether switch is currently pushed

	float distThresh = 1.5f; //distance threshhold where it will become unpressed

	void Update(){
		//calculate distance between switch and player
		float dist = Vector3.Distance(transform.position, player.transform.position);
		//become unpressed if distance is small enough
		if(dist > distThresh && pushed){
			pushed = false;
			//update activatables
			foreach(Activatable o in triggers)
				o.setActivated(pushed);
		}
		//update color for debugging
		if(pushed)
			gameObject.GetComponent<Renderer>().material.color = Color.white;
		else
			gameObject.GetComponent<Renderer>().material.color = Color.red;
	}

	public override void EnterCollisionWithGeneral(){
		pushed = true;//becomes pushed when it collides with player
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
	
	public override void ExitCollisionWithGeneral(){
		
	}
}
