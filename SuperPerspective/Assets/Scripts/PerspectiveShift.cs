using UnityEngine;
using System.Collections;

public class PerspectiveShift : MonoBehaviour {

	public static bool is3D = false;
	

	void Update () {
		//check if game is paused
		if(GameStatus.instance.isPaused())
			return;
		//Tell the camera to the flip when flip is pressed
		if (Input.GetButtonDown("Flip") && !CameraControl.instance.IsFlipping() && CameraControl.instance.isLockedToPlayer()) {
			CameraControl.instance.Flip();
		}
	}
}
