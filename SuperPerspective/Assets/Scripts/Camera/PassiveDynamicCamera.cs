using UnityEngine;
using System.Collections;

public class PassiveDynamicCamera : Activatable {
	
	public override void setActivated(bool a){
		bool startA = activated;
		base.setActivated(a);
		if(a == startA)
			return;
		if(a){
			GameStateManager.instance.EnterDynamicState(transform);
		}else{
			GameStateManager.instance.ExitDynamicState();
		}
	}
}