using UnityEngine;
using System.Collections;

public class BoundBasic : MonoBehaviour {
	
	Rect bounds;
	bool left=true,right=true,front=true,back=true;//probably won't need this
	// Use this for initialization
	void Start () {
		GameObject grnd=GameObject.Find ("Ground");
		float gX=grnd.transform.position.x;
		float gZ=grnd.transform.position.z;
		float gW=grnd.transform.lossyScale.x;
		float gD=grnd.transform.lossyScale.z;
		float pW=transform.lossyScale.x;
		float pD=transform.lossyScale.z;
		bounds = new Rect (gX - (gW / 2f)+(pW/2f), gZ - (gD / 2f)+(pD/2f), gW - pW, gD - pD);
	}
	// Update is called once per frame
	void LateUpdate () {
		//Note to self
		//Why not : transform.x = Mathf.Max (bounds.xMin, Mathf.Min (bounds.xMax, transform.position.x));
		Vector3 pos = transform.position;
		if(front) pos.x = Mathf.Min (bounds.xMax, pos.x);
		if(back) pos.x = Mathf.Max (bounds.xMin, pos.x);
		if(right) pos.z = Mathf.Max (bounds.yMin, pos.z);
		if(left) pos.z = Mathf.Min (bounds.yMax, pos.z);
		if (pos != transform.position) {
			if (gameObject.GetComponent<Ice>()) {
				transform.position -= gameObject.GetComponent<Ice>().getVelocity();
				gameObject.GetComponent<Ice>().setVelocity(Vector3.zero);
			}
		}
		transform.position = pos;
	}
}
