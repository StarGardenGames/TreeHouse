using UnityEngine;
using System.Collections;

public class ResetController : MonoBehaviour {

	static bool camReset = false;

	void Update () {
		if (Input.GetKey(KeyCode.R)) {
			Reset();
		}
	}

	void FixedUpdate() {
		if (camReset) {
			GameStateManager.instance.StartGame();
			CameraController.instance.SetMount(GameObject.Find("2DCameraMount").transform, PerspectiveType.p2D);
			camReset = false;
		}
	}

	public static void Reset() {
		Key.ClearKeys();
		camReset = true;
		Application.LoadLevel(Application.loadedLevel);
		//GameStateManager.Destroy(GameStateManager.instance);
	}
}
