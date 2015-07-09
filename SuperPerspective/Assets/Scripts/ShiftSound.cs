using UnityEngine;
using System.Collections;

public class ShiftSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameStateManager.instance.PerspectiveShiftEvent += PlayShiftAudio;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlayShiftAudio(PerspectiveType p){

		if (p == PerspectiveType.p3D) {

		}

		else if (p == PerspectiveType.p2D) {

		}

	}
}
