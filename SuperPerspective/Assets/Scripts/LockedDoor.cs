using UnityEngine;
using System.Collections;

public class LockedDoor : ActiveInteractable {

	bool opened;

	public override void Triggered() {
		if (!opened && Key.UseKey()) {
			GetComponent<Mover>().setActivated(true);
			opened = true;
		}
	}
}
