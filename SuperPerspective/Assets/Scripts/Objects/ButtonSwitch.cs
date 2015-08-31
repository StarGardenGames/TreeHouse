using UnityEngine;
using System.Collections;

public class ButtonSwitch : Activatable {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Animator a = this.GetComponent<Animator>();
		a.SetBool ("activated", activated);
	}
}
