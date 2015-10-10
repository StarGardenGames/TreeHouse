using UnityEngine;
using System.Collections;

public class PushSwitchOld : MonoBehaviour {

	public Activatable[] triggers;//Activatable objects which this switch triggers
	
	bool pushed = false; //whether switch is currently pushed

	Color baseColor = Color.white;
	
	public Rect parentPlatform;
	Collider pusher = null;
	
	void Update(){
		//update color for debugging
		if (pushed) {
			if (baseColor == Color.white)
				baseColor = gameObject.GetComponent<Renderer>().material.color;
			gameObject.GetComponent<Renderer>().material.color = Color.white;
			//gameObject.tag = "Intangible";
		} else {
			if (baseColor != Color.white)
				gameObject.GetComponent<Renderer>().material.color = baseColor;
			//gameObject.tag = "Untagged";
		}
		Bounds check;
		if (pusher != null) {
			check = pusher.bounds;
			check.Expand (0.1f);
			if (!check.Intersects (GetComponent<Collider> ().bounds))
				ExitCollisionWithGeneral (pusher.gameObject);
		}
		/*RaycastHit hit;
		parentPlatform = PlayerController.instance.GetComponent<BoundObject>().GetBounds();
		if (GameStateManager.is3D()) {
			if (Physics.Raycast(transform.position + Vector3.forward * 2f, -Vector3.forward, out hit, 4f, LayerMask.NameToLayer("RaycastIgnore"))) {
				if (!pushed)
					EnterCollisionWithGeneral(hit.collider.gameObject);
			} else if (pushed) {
				ExitCollisionWithGeneral(null);
			}
		} else {
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, parentPlatform.max.y + 1f), -Vector3.forward, out hit,
			                    parentPlatform.height + 2f, LayerMask.NameToLayer("RaycastIgnore"))) {
				if (!pushed)
					EnterCollisionWithGeneral(hit.collider.gameObject);
			} else if (pushed) {
				ExitCollisionWithGeneral(null);
			}
		}*/
	}

	public void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
		//pusher = other.GetComponent<Collider> ();
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
	
	public void ExitCollisionWithGeneral(GameObject other){
		pushed = false;//becomes pushed when it collides with player
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
}
