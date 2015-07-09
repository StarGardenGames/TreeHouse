using UnityEngine;
using System.Collections;

public class ToggleSwitch : ActiveInteractable {

	//suppress warnings
	#pragma warning disable 414

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool toggleEnabled = false; //whether switch is currently toggleEnabled

	float distThresh = 1.5f; //distance threshhold where it will become unpressed
	
	public override void Triggered(){
		Toggle();//toggle switch
	}

	void Toggle(){

		//Triggers sound -Nick

		if (!toggleEnabled) {
			gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Objects/Switch/SwitchOn")  as AudioClip;
		}

		else {
			gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Objects/Switch/SwitchOff")  as AudioClip;
		}

		gameObject.GetComponent<AudioSource>().Play ();

		//End Nick stuff

		toggleEnabled = !toggleEnabled;//enable toggles
		//toggleEnabled is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(toggleEnabled);
	}
}
