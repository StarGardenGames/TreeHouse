using UnityEngine;
using System.Collections;

public class ResetController : MonoBehaviour {

	static bool camReset = false;

	void Update () {
	}

	void FixedUpdate() {
		if (camReset) {
			GameStateManager.instance.Reset();
			PlayerController.instance.Reset();
			camReset = false;
		}
	}

	public static void Reset() {
		Key.ClearKeys();
		camReset = true;
		Application.LoadLevel(Application.loadedLevel);
		InputManager.Destroy(InputManager.instance);
	}
}
