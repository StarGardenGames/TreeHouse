using UnityEngine;
using System.Collections;

public class Checkpoint : PlayerInteractable {

	public int id;
	public bool triggered = false;
	
	public override void PlayerEnteredRange(){
		triggered = true;
		CheckpointManager.instance.showMenu(id);
	}
	
	public override void PlayerExitedRange(){
		triggered = false;
	}
	
	
}
