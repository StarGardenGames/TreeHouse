using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject followTarget;
	public bool followZ;

	Rect myBounds;
	Rect[] bounds;

	// Use this for initialization
	void Start () {
		bounds = IslandControl.instance.islandBounds;
	}
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
		BoundObject playerBoundObject = (BoundObject) player.GetComponent("BoundObject");
		Rect groundBound = playerBoundObject.GetBounds();
		//follow camera
		if(followTarget != null){
			Vector3 targetPos = followTarget.transform.position;
			if(followZ){
				transform.position = targetPos;
			}else{
				transform.position = new Vector3(targetPos.x, targetPos.y, targetPos.z + 45.0f);
			}
		}
	}
}
