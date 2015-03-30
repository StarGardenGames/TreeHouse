using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	public Door dest;

	public override void Triggered(){
		player.transform.position = dest.transform.position + new Vector3(0,0,-2);
	}
	
}

