using UnityEngine;
using System.Collections;

public class ShiftSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameStateManager.instance.PerspectiveShiftSuccessEvent += PlayShiftAudio;
		GameStateManager.instance.PerspectiveShiftFailEvent += PerspectiveShiftFailAudio;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlayShiftAudio(){

		if (GameStateManager.instance.currentPerspective == PerspectiveType.p2D) {
			gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Player/Shift/To2D")  as AudioClip;
			gameObject.GetComponent<AudioSource>().Play();
		}

		else if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D) {
			gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Player/Shift/To3D")  as AudioClip;
			gameObject.GetComponent<AudioSource>().Play();
		}

	}

	void PerspectiveShiftFailAudio(){
		gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Player/Shift/ShiftFail")  as AudioClip;
		gameObject.GetComponent<AudioSource>().Play();
	}
}
