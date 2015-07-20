using UnityEngine;
using System.Collections;

public class ShiftSound : MonoBehaviour {

	AudioSource source;

	// Use this for initialization
	void Start () {
		GameStateManager.instance.PerspectiveShiftSuccessEvent += PlayShiftAudio;
		GameStateManager.instance.PerspectiveShiftFailEvent += PlayFailAudio;
		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlayShiftAudio(){

		PerspectiveType p = GameStateManager.instance.currentPerspective;

		if (p == PerspectiveType.p3D) {
			source.clip = Resources.Load ("Sound/SFX/Player/Shift/To3D")  as AudioClip;
			source.Play();
		}

		else if (p == PerspectiveType.p2D) {
			source.clip = Resources.Load ("Sound/SFX/Player/Shift/To2D")  as AudioClip;
			source.Play();
		}

	}

	void PlayFailAudio(){
		source.clip = Resources.Load ("Sound/SFX/Player/Shift/ShiftFail")  as AudioClip;
		source.Play();
	}
}
