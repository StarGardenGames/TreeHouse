using UnityEngine;
using System.Collections;

public class Mover : Activatable {
	public Vector3 movement = Vector3.zero;//path along which object will move
	public float transitionTime = 1f;//time it takes for transition to occur

	Vector3 startPosition;//start position
	float prog = 0f; //progression from start to start+ movement

	void Start(){
		//make startPositon to be current position
		Vector3 pos = transform.position;
		startPosition = new Vector3(pos.x,pos.y,pos.z);
	}

	void Update(){
		if (!PlayerController.instance.isPaused()){
			//update prog
			prog+= (Time.deltaTime/transitionTime) * ((activated)? 1 : -1);//increase or decrease depending on activated
			prog = Mathf.Clamp01(prog); //clamp between 0 and 1
	
			//set position
			transform.position = Vector3.Lerp(startPosition, startPosition + movement, prog);
		}
	}
}
