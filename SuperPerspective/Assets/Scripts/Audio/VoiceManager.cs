using UnityEngine;
using System.Collections;

public class VoiceManager : MonoBehaviour {

	AudioClip[] jumps, climbs, pushes;
	AudioSource source;

	// Use this for initialization
	void Start () {
		jumps = new AudioClip[4];
		jumps [0] = Resources.Load ("Sound/SFX/Player/Voice/Jump1")  as AudioClip;
		jumps [1] = Resources.Load ("Sound/SFX/Player/Voice/Jump2")  as AudioClip;
		jumps [2] = Resources.Load ("Sound/SFX/Player/Voice/Jump3")  as AudioClip;
		jumps [3] = Resources.Load ("Sound/SFX/Player/Voice/Jump4")  as AudioClip;

		climbs = new AudioClip[3];
		climbs [0] = Resources.Load ("Sound/SFX/Player/Voice/Climb1")  as AudioClip;
		climbs [1] = Resources.Load ("Sound/SFX/Player/Voice/Climb2")  as AudioClip;
		climbs[2] = Resources.Load ("Sound/SFX/Player/Voice/Climb3")  as AudioClip;

		pushes = new AudioClip[3];
		pushes [0] = Resources.Load ("Sound/SFX/Player/Voice/PushPull1")  as AudioClip;
		pushes [1] = Resources.Load ("Sound/SFX/Player/Voice/PushPull2")  as AudioClip;
		pushes [2] = Resources.Load ("Sound/SFX/Player/Voice/PushPull3")  as AudioClip;

		source = gameObject.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown (KeyCode.Space)) {
			Jump ();
		}*/
	}

	public void Jump(){
		source.clip = jumps [Random.Range (0, 4)];
		source.pitch = Random.Range (0.95f, 1.05f);
		source.Play ();
	}

	public void Climb(){
		source.clip = climbs [Random.Range (0, 3)];
		source.pitch = Random.Range (0.95f, 1.05f);
		source.Play ();
	}

	public void PushPull(){
		source.clip = pushes [Random.Range (0, 3)];
		source.pitch = Random.Range (0.95f, 1.05f);
		source.Play ();
	}
}
