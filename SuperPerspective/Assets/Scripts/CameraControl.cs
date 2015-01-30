using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public static CameraControl instance;
	public Transform mount;
	public float speedFactor;
	public bool locked = false;


	void Awake(){
		//singleton pattern
		if( instance == null)
			instance = this;
		else if( instance != this)
			Destroy(gameObject);
	}

	// Update is called once per frame
	void LateUpdate () {
		//update position & orientation
		if(!locked){
			//move towards mount
			transform.position = Vector3.Lerp(transform.position,mount.position, speedFactor);
			transform.rotation = Quaternion.Slerp(transform.rotation, mount.rotation, speedFactor);
		}else{
			//fix position
			Snap();
		}

		//update locked status
		if(Vector3.Distance(transform.position, mount.position) < .01f)
			locked = true;
	}

	public void SetMount(Transform newMount){
		mount = newMount;
		locked = false;
	}

	public void Snap(){
		transform.position = mount.position;
		transform.rotation = mount.rotation;
	}
}
