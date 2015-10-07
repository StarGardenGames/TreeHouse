using UnityEngine;
using System.Collections;

public class ActiveDynamicCamera : Activatable {

	public float lockDuration = 3;
	
	private float activateTime = -1f;
	
	public void FixedUpdate(){
		checkForLockEnd();
	}	
	
	private void checkForLockEnd(){
		bool stillWaiting = activateTime > 0;
		bool timesUp = Time.time - activateTime > lockDuration;
		if(stillWaiting && timesUp)
			setActivated(false);
	}
	
	public override void setActivated(bool a){
		bool startA = activated;
		base.setActivated(a);
		if(a == startA)
			return;
		if(a){
			GameStateManager.instance.EnterDynamicState(transform);
			PlayerController.instance.setCutsceneMode(true);
			activateTime = Time.time;
		}else{
			GameStateManager.instance.ExitDynamicState();
			PlayerController.instance.setCutsceneMode(false);
			activateTime = -1f;
		}
	}
}
