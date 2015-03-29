using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	//singleton
	public static CameraControl instance;

	//public variables
	public bool startOnPlayer = true;
	public Transform mount;//unused if startOnPlayer = true;
	public float transitionTime;

	//player mount variables
	private Transform pcam, ocam;

	//camera status
	private bool lockedToPlayer = false;
	private bool is3D;

	//transisition variables
	Vector3 startPosition;
	Quaternion startRotation;
	float camProg = 0f;

	
	void Awake(){
		//singleton pattern
		if( instance == null)
			instance = this;
		else if( instance != this)
			Destroy(gameObject);
	}
	
	void Start() {
		//Get 2D and 3D camera positions
		GameObject p = GameObject.Find("Player");
		pcam = p.GetComponent<Transform>().FindChild("2DCameraMount");
		ocam = p.GetComponent<Transform>().FindChild("3DCameraMount");
		
		//check if we're starting on player
		if(startOnPlayer){
			is3D = false;
			SetMount(ocam);
		}else{
			is3D = true;
			SetMount(mount);
		}

		//update 3d
		update3D();
	}
	
	void LateUpdate () {
		//update position & orientation
		if(lockedToPlayer){
			Snap();
		}else if(camProg<=1){
			//update progress
			camProg += Time.deltaTime/transitionTime;
			camProg = Mathf.Min(1,camProg);
			//transition
			float f = 1-Mathf.Pow(camProg-1,2);//smothing algorithm
			transform.position = Vector3.Lerp(startPosition, mount.position, f);
			transform.rotation = Quaternion.Slerp(startRotation, mount.rotation, f);
			//lock to end
			if(camProg == 1 && MountedToPlayer()){
				lockedToPlayer = true;
				update3D();
			}
		}
	}
	
	public void SetMount(Transform newMount){
		mount = newMount;
		lockedToPlayer = false;
		startPosition = new Vector3(
			transform.position.x,
			transform.position.y,
			transform.position.z
		);
		startRotation = new Quaternion(
			transform.rotation.x,
			transform.rotation.y,
			transform.rotation.z,
			transform.rotation.w
		);
		camProg = 0f;
	}

	public void MountToPlayer(){
		if(is3D)
			SetMount(pcam);
		else
			SetMount(ocam);
	}

	
	public void Snap(){
		transform.position = mount.position;
		transform.rotation = mount.rotation;
	}
	
	public void Flip() {
		is3D = !is3D;
		if (is3D) {
			SetMount(pcam);
			update3D();
		} else {
			SetMount(ocam);
			//we waito to update3D till we reach destination
		}
	}
	
	public bool IsFlipping() {
		return MountedToPlayer() && !lockedToPlayer;
	}

	private bool MountedToPlayer(){
		return mount==ocam || mount==pcam;
	}

	private void update3D(){
		Camera.main.orthographic = !is3D;
	}

	public bool isLockedToPlayer(){
		return lockedToPlayer;
	}

}