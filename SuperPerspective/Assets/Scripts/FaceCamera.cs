using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {	
	// Update is called once per frame
	void Update () {
		gameObject.transform.rotation = Quaternion.Inverse(
			CameraController.instance.gameObject.transform.rotation);
		Debug.Log(CameraController.instance.gameObject.transform.rotation);
	}
}
