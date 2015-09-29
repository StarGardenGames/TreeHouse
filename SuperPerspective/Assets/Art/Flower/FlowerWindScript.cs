
using UnityEngine;
using System.Collections;

public class FlowerWindScript : MonoBehaviour {

	public Animator anim;                      // Reference to the animator component.

	// Use this for initialization
	void Start () {
		anim.SetBool ("WindOn", true);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
