using UnityEngine;
using System.Collections;

public class Activatable : MonoBehaviour {
	public bool activated = false; //whether or not object is activated

	//update activated
	public void setActivated(bool a){
		activated = a;
	}

}
