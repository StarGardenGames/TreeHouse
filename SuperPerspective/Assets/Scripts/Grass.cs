using UnityEngine;
using System.Collections;

public class Grass : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.Rotate(Vector3.up, Mathf.Rad2Deg * transform.position.x * transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
