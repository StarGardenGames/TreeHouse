using UnityEngine;
using System.Collections;

public class NotificationController : MonoBehaviour {
	
	//GameObjects representing the different planes for the notification
	Renderer plane3D, plane2D;
	//whether or not it's visible
	bool visible = false;
	
	//find planes
	void Start(){
		plane3D = transform.GetChild(0).GetComponent<Renderer>();
		plane2D = transform.GetChild(1).GetComponent<Renderer>();
	}
	
	//update visibility of planes
	void FixedUpdate () {
		plane3D.enabled = visible &&  PlayerController.instance.is3D();
		plane2D.enabled = visible && !PlayerController.instance.is3D();
	}
	
	public void updateVisible(bool visible){
		this.visible = visible;
	}
}
