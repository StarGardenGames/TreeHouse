using UnityEngine;
using System.Collections;

public class FallingPlatform : LandOnObject {

	bool falling, respawning;
	int shake, respawn;
	Vector3 origScale, origPos;

	// Use this for initialization
	void Start () {
		origScale = transform.localScale;
		origPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (shake > 0) {
			transform.position = origPos + (Vector3.right * Random.Range(-0.25f, 0.25f)) + (Vector3.forward * Random.Range(-0.25f, 0.25f));
			shake--;
			if (shake == 0) {
				falling = true;
				GetComponent<Collider>().enabled = false;
			}
		}
		if (respawn > 0) {
			respawn--;
			if (respawn == 0) {
				transform.localScale = origScale;
				transform.position = origPos;
				GetComponent<Renderer>().enabled = true;
				GetComponent<Collider>().enabled = true;
			}
		}
		if (falling) {
			transform.Translate(Vector3.down * (1 / 25f));
			transform.localScale *= 0.9f;
			if (transform.localScale.magnitude < 0.1) {
				GetComponent<Renderer>().enabled = false;
				respawn = 50;
				falling = false;
			}
		}
	}

	public override void LandedOn() {
		if (shake == 0)
			shake = 50;
	}
}
