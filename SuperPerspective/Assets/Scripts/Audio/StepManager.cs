using UnityEngine;
using System.Collections;

public class StepManager : MonoBehaviour {

	//suppress warnings
	#pragma warning disable 414

	//init vars

	AudioClip[] grassSteps;
	AudioSource source;

	float stepTimer;

	// Use this for initialization
	void Start () {
		grassSteps = new AudioClip[4];
		grassSteps [0] = Resources.Load ("Sound/SFX/Player/Steps/Grass1")  as AudioClip;
		grassSteps [1] = Resources.Load ("Sound/SFX/Player/Steps/Grass2")  as AudioClip;
		grassSteps [2] = Resources.Load ("Sound/SFX/Player/Steps/Grass3")  as AudioClip;
		grassSteps [3] = Resources.Load ("Sound/SFX/Player/Steps/Grass4")  as AudioClip;
		
		source = gameObject.GetComponent<AudioSource> ();
		stepTimer = 0.312f;
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)
			|| Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.RightArrow)
		    || Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A)
		    || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D)) {

			stepTimer -= Time.deltaTime;
		} 

		else {
			stepTimer = 0.312f;
		}

		if (stepTimer <= 0) {
			GrassStep ();
			stepTimer = 0.312f;
		}*/
	}

	public void GrassStep(){
		source.clip = grassSteps [Random.Range (0, 4)];
		source.pitch = Random.Range (0.95f, 1.05f);
		source.volume = 0.15f;
		source.Play ();
	}
}
