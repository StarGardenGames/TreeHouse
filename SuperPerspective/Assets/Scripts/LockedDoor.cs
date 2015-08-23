using UnityEngine;
using System.Collections;

public class LockedDoor : ActiveInteractable {

	public int keysRequired = 1;
	bool opened;

	public override void Triggered() {
		if (!opened && Key.UseKey(keysRequired)) {
			GetComponent<Mover>().setActivated(true);
			opened = true;
		}
	}
}
