using UnityEngine;
using System.Collections;

public class PushSwitch : PassiveInteractable {

	#pragma warning disable 114

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pushed = false; //whether switch is currently pushed

	private ArrayList pushers = new ArrayList();

	Renderer rune;

	Vector3 baseScale;

	void Start() {
		base.Start();
		rune = GetComponentInChildren<Renderer>();
		baseScale = rune.transform.localScale;
	}

	void Update(){
		//update color for debugging
		if (pushed) {
			rune.transform.localScale = baseScale * 0.8f;
		} else {
			rune.transform.localScale = baseScale;
		}
		Bounds check;
		if (pushers.Count > 0) {
			foreach (Collider pusher in pushers) {
				check = pusher.bounds;
				if (!check.Intersects (GetComponent<Collider> ().bounds)) {
					pushers.RemoveAt(pushers.Count - 1);
					ExitCollisionWithGeneral(pusher.gameObject);
				}
			}
		}
	}

	void FixedUpdate() {
		if (!pushed)
			rune.transform.RotateAround (transform.position, Vector3.up, 1);
		else
			rune.transform.RotateAround (transform.position, Vector3.up, 2);
	}

	public override void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
		pushers.Add(other.GetComponent<Collider>());
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
	
	public override void ExitCollisionWithGeneral(GameObject other){
		if (pushers.Count == 0) {
			pushed = false;//becomes pushed when it collides with player
			//pushed is also updated for all activatable objects
			foreach(Activatable o in triggers)
				o.setActivated(pushed);
		}
	}
}
