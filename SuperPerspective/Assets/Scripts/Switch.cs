using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pressed = false; //whether switch is currently pressed
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
		if(dist > distThresh)
			pressed = false;
		//update color for debugging
		if(enabled)
			gameObject.renderer.material.color = Color.white;
		else
			gameObject.renderer.material.color = Color.red;
	}

	public void CollisionWithPlayer(){
		if(!pressed)//only update switch if it's not already pressed;
			Toggle();//toggle switch
	}

	void Toggle(){
		pressed = true;//switch becomes pressed
		enabled = !enabled;//enable toggles
		//enabled is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(enabled);
	}
}
