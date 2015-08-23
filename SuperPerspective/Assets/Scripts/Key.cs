using UnityEngine;
using System.Collections;

public class Key : Interactable {

	static int keysHeld = 0;
	bool active = true;

	void FixedUpdate() {
		if (active)
			GetComponentInChildren<Renderer>().transform.Rotate(Vector3.up, Mathf.PI / 4, Space.World);
	}

	public static bool UseKey(int keyRequired) {
		if(keyRequired == null){
			keyRequired = 1;
		}
		if (keysHeld > keyRequired) {
			keysHeld--;
			return true;
		}
		return false;
	}

	public static void CollectKey() {
		keysHeld++;
	}

	public static void ClearKeys() {
		keysHeld = 0;
	}

	public static int GetKeysHeld(){
		return keysHeld;
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
