using UnityEngine;
using System.Collections;

public class PushSwitch : MonoBehaviour {

	#pragma warning disable 114, 414

	public GameObject parentPlatform;

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pushed = false; //whether switch is currently pushed

	private ArrayList pushers = new ArrayList();

	Renderer rune;

	Vector3 baseScale;

	void Awake()
	{
		if (parentPlatform == null) {
			parentPlatform = GameObject.Find("Ground");
		}
		
	}

	void Start() {
		rune = GetComponentInChildren<Renderer>();
		baseScale = rune.transform.localScale;
	}

	void Update(){
		if (pushed) {
			rune.transform.localScale = baseScale * 0.8f;
		} else {
			rune.transform.localScale = baseScale;
		}
		RaycastHit hit;
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D) {
			if (Physics.Raycast(transform.position + Vector3.forward, -Vector3.forward, out hit, 2f, LayerMask.NameToLayer("RaycastIgnore"))) {
				if (!pushed)
					EnterCollisionWithGeneral(hit.collider.gameObject);
			} else if (pushed) {
				ExitCollisionWithGeneral(null);
			}
		} else {
			if (Physics.Raycast(transform.position + Vector3.forward * parentPlatform.transform.lossyScale.z / 2f, -Vector3.forward, out hit, parentPlatform.transform.lossyScale.z, LayerMask.NameToLayer("RaycastIgnore"))) {
				if (!pushed)
					EnterCollisionWithGeneral(hit.collider.gameObject);
			} else if (pushed) {
				ExitCollisionWithGeneral(null);
			}
		}
		/*if (pushers.Count > 0) {
			foreach (Collider pusher in pushers) {
				check = pusher.bounds;
				if (!check.Intersects (GetComponent<Collider> ().bounds)) {
					ExitCollisionWithGeneral(pusher.gameObject);
				}
			}
		}*/
	}

	void FixedUpdate() {
		if (!pushed)
			rune.transform.RotateAround (transform.position, Vector3.up, 1);
		else
			rune.transform.RotateAround (transform.position, Vector3.up, 2);
	}

	public void EnterCollisionWithPlayer() {
		EnterCollisionWithGeneral(null);
	}

	public void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
	
	public void ExitCollisionWithGeneral(GameObject other){
		pushed = false;//becomes pushed when it collides with player
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
}
