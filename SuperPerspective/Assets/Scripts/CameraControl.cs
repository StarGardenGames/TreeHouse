using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public static CameraControl instance;
	public Transform mount;
	public float speedFactor;
	public bool locked = false, is3D = false;
	private float t;
	private Transform pcam, ocam;	

	void Awake(){
		//singleton pattern
		if( instance == null)
			instance = this;
		else if( instance != this)
			Destroy(gameObject);
	}

	void Start() {
		//Get 2D and 3D camera positions
		pcam = transform.parent.FindChild("CamPosPersp");
		ocam = transform.parent.FindChild("CamPosOrtho");
	}

	void LateUpdate () {
		//update position & orientation
		if(!locked){
			transform.position = Vector3.Lerp(transform.position,mount.position, speedFactor);
			transform.rotation = Quaternion.Slerp(transform.rotation, mount.rotation, speedFactor);
		}else{
			//fix position
			Snap();
		}
		
		//update locked status
		if(Vector3.Distance(transform.position, mount.position) < .1f) {
			locked = true;
			//if 2D switch to ortho when the camera is in place
			if (!is3D)
				Camera.main.orthographic = true;
		}
	}
	
	public void SetMount(Transform newMount){
		mount = newMount;
		locked = false;
		t = 0;
	}

	public void Snap(){
		transform.position = mount.position;
		transform.rotation = mount.rotation;
	}

	public void Flip() {
		is3D = !is3D;
		if (is3D) {
			SetMount(pcam);
			Camera.main.orthographic = false;
		} else {
			SetMount(ocam);
		}
	}

	public bool IsFlipping() {
		return !locked;
	}
}
