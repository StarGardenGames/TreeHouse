using UnityEngine;
using System.Collections;

public class EarControl : MonoBehaviour {
	
	float setRot = 0;
	float rt = 0;
	float rotSpeed = 6;
	GameObject lEar, rEar;
	PlayerController player;

	// Use this for initialization
	void Start () {
		lEar = GameObject.Find("EarNL");
		rEar = GameObject.Find("EarNR");
		player = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player.Check2DIntersect()) {
			setRot = 0;
		} else {
			setRot = 90;
		}
	}

	void FixedUpdate() {
		if (rt < setRot) {
			lEar.transform.Rotate(Vector3.right, rotSpeed);
			rEar.transform.Rotate(Vector3.right, rotSpeed);
			rt = Mathf.Min(rt + rotSpeed, setRot);
		} else if (rt > setRot) {
			lEar.transform.Rotate(Vector3.right, -rotSpeed);
			rEar.transform.Rotate(Vector3.right, -rotSpeed);
			rt = Mathf.Max(rt - rotSpeed, setRot);
		}
	}
}
