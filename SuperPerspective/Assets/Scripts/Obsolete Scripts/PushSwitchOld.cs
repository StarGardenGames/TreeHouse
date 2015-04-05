using UnityEngine;
using System.Collections;

public class PushSwitchOld : Interactable {

	public Activatable[] triggers;//Activatable objects which this switch triggers
	
	bool pushed = false; //whether switch is currently pushed
	
	Collider pusher = null;
	
	void Update(){
		//update color for debugging
		if (pushed) {
			gameObject.GetComponent<Renderer> ().material.color = Color.white;
		} else {
			gameObject.GetComponent<Renderer> ().material.color = Color.red;
		}
		Bounds check;
		if (pusher != null) {
			check = pusher.bounds;
			check.Expand (0.1f);
			if (!check.Intersects (GetComponent<Collider> ().bounds))
				ExitCollisionWithGeneral (pusher.gameObject);
		}
	}

	public override void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
		pusher = other.GetComponent<Collider> ();
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
	
	public override void ExitCollisionWithGeneral(GameObject other){
		pushed = false;//becomes pushed when it collides with player
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
}
