using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject followTarget;
	public bool followZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(followTarget != null){
			Vector3 targetPos = followTarget.transform.position;
			if(followZ){
				transform.position = targetPos;
			}else{
				transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
			}
		}
	}
}
