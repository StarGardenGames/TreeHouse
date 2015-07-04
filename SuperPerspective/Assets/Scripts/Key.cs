using UnityEngine;
using System.Collections;

public class Key : Interactable {

	static int keysHeld = 0;
	bool active = true;

	void FixedUpdate() {
		if (active)
			GetComponentInChildren<Renderer>().transform.Rotate(Vector3.up, Mathf.PI / 16);
	}

	public static bool UseKey() {
		if (keysHeld > 0) {
			keysHeld--;
			return true;
		}
		return false;
	}

	public static void CollectKey() {
		keysHeld++;
	}

	public override void EnterCollisionWithPlayer () {
		if (!active)
			return;
		base.EnterCollisionWithPlayer();
		CollectKey();
		Destroy(GetComponentInChildren<Renderer>());
		active = false;
	}
}
