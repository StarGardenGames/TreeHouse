using UnityEngine;
using System.Collections;

public class HideOnStart : MonoBehaviour {
	public bool isHide;
	bool isHidden;

	// Use this for initialization
	void Start () {
		this.hideAll(isHide);
	}
	
	void hideAll (bool hide){
		MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer c in mr){
			c.enabled = !hide;
		}
		this.isHidden = hide;
	}
	// Update is called once per frame
	void Update () {
		if(isHide != isHidden){
			this.hideAll(isHide);
		}
	}
}
