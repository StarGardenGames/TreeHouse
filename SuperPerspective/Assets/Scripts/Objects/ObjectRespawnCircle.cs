using UnityEngine;
using System.Collections;

public class ObjectRespawnCircle : MonoBehaviour{

	public Camera playerCam;

	public GameObject plane;
	public float setAlpha = 1f, fadeSpeed = 0.15f;
	public Renderer rend;
	GameObject player;


	// Use this for initialization
	void Start () {
		playerCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(playerCam.transform, Vector3.up);

	}

}


