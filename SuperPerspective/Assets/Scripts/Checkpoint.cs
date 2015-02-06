using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public int id;
	public bool triggered = false;
	float triggerMargin = 3;

	GameObject p;

	// Use this for initialization
	void Start() {
		p = GameObject.Find("Player");
	}

	// Update is called once per frame
	void Update () {
		//trigger menu when player is close enough
		float dist = Vector3.Distance(transform.position, p.transform.position);
		if(!CheckpointManager.instance.menuVisible){
			if(dist < triggerMargin && !triggered){
				triggered = true;
				CheckpointManager.instance.showMenu(id);
			}
			if(dist > triggerMargin)
				triggered = false;
		}
	}
}
