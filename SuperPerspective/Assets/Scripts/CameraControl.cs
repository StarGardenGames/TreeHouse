using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	//singleton
	public static CameraControl instance;

	//public variables
	public bool startOnPlayer = true;
	public Transform mount;//unused if startOnPlayer = true;
	public float speedFactor;

	//player mount variables
	private Transform pcam, ocam;

	//camera status
	private bool lockedToPlayer = false;
	private bool is3D;

	
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
		pcam = p.GetComponent<Transform>().FindChild("CamPosPersp");
		ocam = p.GetComponent<Transform>().FindChild("CamPosOrtho");

		//check if we're starting on player
		if(startOnPlayer){
			is3D = false;
			SetMount(ocam);
		}else{
			is3D = true;
		}

		//update 3d
		update3D();
	}
	
	void LateUpdate () {
		//update position & orientation
		if(!lockedToPlayer){
			transform.position = Vector3.Lerp(transform.position,mount.position, speedFactor);
			transform.rotation = Quaternion.Slerp(transform.rotation, mount.rotation, speedFactor);
		}else{
			//fix position
			Snap();
		}
		
		//update locked status
		if(Vector3.Distance(transform.position, mount.position) < .1f && MountedToPlayer()) {
			lockedToPlayer = true;
			update3D();
		}
	}
	
	public void SetMount(Transform newMount){
		mount = newMount;
		lockedToPlayer = false;
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