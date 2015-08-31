using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	float speed;

	// Use this for initialization
	void Start () {
		speed = 5 * Random.Range (0.8f, 1.2f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(speed*Time.deltaTime, speed*Time.deltaTime, speed*Time.deltaTime));
	}
}
