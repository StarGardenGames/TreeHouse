using UnityEngine;
using System.Collections;

//Work In Progress

public class PassiveDynamicCamera : Activatable {

	private PerspectiveType previousPerspective ;

	float range = 10;
	
	public void FixedUpdate(){
		checkRange();
	}
	
	private void checkRange(){
		float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
		
		if(dist < range && !activated)
			setActivated(true);
		
		if(dist > range && activated)
			setActivated(false);
	}
	
	public override void setActivated(bool a){
		base.setActivated(a);
		if(a){
			previousPerspective = GameStateManager.instance.currentPerspective;
			GameStateManager.instance.EnterDynamicState(transform);
		}else{
			GameStateManager.instance.ExitDynamicState(previousPerspective);
		}
	}
}
