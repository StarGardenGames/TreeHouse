using UnityEngine;
using System.Collections;

public class RealmTracker : Activatable {

	public GameObject[] reds,blues;
	
	public override void setActivated(bool a){
		RealmManager.instance.toggle(a);
	}
}
