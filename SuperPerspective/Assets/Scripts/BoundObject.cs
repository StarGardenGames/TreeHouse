using UnityEngine;
using System.Collections;

public class BoundObject : MonoBehaviour {

	public Rect myBounds;
	Rect[] bounds;
	//float altLeftBound = -1;//-1 means no alternate left bound
	//float altRightBound = -1;
	float groundY = 0;

	// Use this for initialization
	void Start () {
		bounds = IslandControl.instance.islandBounds;
		updateBounds();
	}

	public void updateBounds(){
		Vector3 pos = transform.position;
		int boundIndex = IslandControl.instance.getBound (pos.x, pos.y, pos.z, !PlayerController.instance.is3D());
		//update myBounds
		float halfWidth = transform.lossyScale.x / 2f;
		float halfDepth = transform.lossyScale.z / 2f;
		myBounds = new Rect(
			bounds[boundIndex].xMin + halfWidth,
			bounds[boundIndex].yMin + halfDepth,
			bounds[boundIndex].width - (halfWidth * 2),
			bounds[boundIndex].height - (halfDepth *2)
		);

		//get bounds for 2d mode
		/*altLeftBound = IslandControl.instance.altBounds [boundIndex, 0];
		if (altLeftBound != -1)
			altLeftBound += halfWidth;
		altRightBound = IslandControl.instance.altBounds [boundIndex, 1];
		if (altRightBound != -1)
			altRightBound -= halfWidth;*/

		//bind to new bounds
		bind ();
	}

	// Update is called once per frame
	void LateUpdate () {
		bind ();
	}

	void bind(){
		bool pMode = PlayerController.instance.is3D();
		Vector3 pos = transform.position;
		//left
		/*if (altLeftBound == -1 || pMode)
			pos.x = Mathf.Max (myBounds.xMin, pos.x);
		else
			pos.x = Mathf.Max (altLeftBound, pos.x);
		if (pos.y < groundY && pos.x < myBounds.xMin)
			updateBounds ();*/
		pos.x = Mathf.Max (myBounds.xMin, pos.x);
		//right
		/*if (altRightBound == -1 || pMode)
			pos.x = Mathf.Min (myBounds.xMax, pos.x);
		else
			pos.x = Mathf.Min (altRightBound, pos.x);
		if (pos.y < groundY && pos.x > myBounds.xMax)
			updateBounds ();*/
		pos.x = Mathf.Min (myBounds.xMax, pos.x);
		//bind z 
		if(pMode /*|| PlayerMovement.instance.flipping*/)
			pos.z = Mathf.Max (myBounds.yMin, Mathf.Min (myBounds.yMax, transform.position.z));
		transform.position = pos;
	}
}
