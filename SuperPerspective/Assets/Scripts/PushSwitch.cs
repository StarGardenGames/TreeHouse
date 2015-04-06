using UnityEngine;
using System.Collections;

public class PushSwitch : Interactable {

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pushed = false; //whether switch is currently pushed

	Collider pusher = null;

	Renderer rune;

	void Update(){
		//update color for debugging
		if (pushed) {
			transform.localScale = new Vector3(0.8f, 1, 0.8f);
		} else {
			transform.localScale = new Vector3(1, 1, 1);
		}
		Bounds check;
		if (pusher != null) {
			check = pusher.bounds;
			check.Expand (0.1f);
			if (!check.Intersects (GetComponent<Collider> ().bounds))
				ExitCollisionWithGeneral (pusher.gameObject);
		}
	}

	public override void FixedUpdate() {
		rune = GetComponentInChildren<Renderer>();
		if (!pushed)
			rune.transform.RotateAround (transform.position, Vector3.up, 1);
		else
			rune.transform.RotateAround (transform.position, Vector3.up, 2);
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
