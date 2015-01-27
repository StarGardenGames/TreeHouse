using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public int id;
	bool triggered = false;
	float triggerMargin = 1;

	GameObject p;

	// Use this for initialization
	void Start() {
		p = GameObject.Find("Player");
	}


	/*void OnTriggerEnter(Collider col) {
		Debug.Log("Collision Entered");
		Debug.Log(col.gameObject.name);
		if(col.gameObject.name.Equals("Player") && !CheckpointManager.instance.menuVisible){
			Debug.Log("Collision w/ Player");
			CheckpointManager.instance.showMenu(id);
		}
	}*/

	// Update is called once per frame
	void Update () {
		//trigger menu when player is close enough
		float dist = Vector3.Distance(transform.position, p.transform.position);
		if(!CheckpointManager.instance.menuVisible){
			if(dist < 1f && !triggered){
				triggered = true;
				CheckpointManager.instance.showMenu(id);
			}
			if(dist > 1f)
				triggered = false;
		}
	}
}
