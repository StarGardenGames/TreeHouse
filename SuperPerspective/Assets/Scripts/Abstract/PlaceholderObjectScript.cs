using UnityEngine;
using System.Collections;

public class PlaceholderObjectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshRenderer[] meshies = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mr in meshies) {
			mr.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
