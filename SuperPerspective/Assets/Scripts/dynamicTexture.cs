using UnityEngine;
using System.Collections;

public class dynamicTexture : MonoBehaviour {
	float planeOffset = 0;
	public Texture topTexture;
	public Texture bottomTexture;
	public Texture leftTexture;
	public Texture rightTexture;
	public Texture backTexture;
	public Texture frontTexture;

	void Awake(){
		//create the 6 sided cube

		if(topTexture) createSidePlane("top", topTexture);
		if(bottomTexture) createSidePlane("bottom", bottomTexture);
		if(leftTexture) createSidePlane("left", leftTexture);
		if(rightTexture) createSidePlane("right", rightTexture);
		if(backTexture) createSidePlane("back", backTexture);
		if(frontTexture) createSidePlane("front", frontTexture);

		//remove the builder prototype cube
		Destroy(this.GetComponent<MeshRenderer>());
	}

	//creates a plane on the side of a cube.
	GameObject createSidePlane(string side, Texture texture){
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.name = "dynamic-plane-"+side;

		Transform pTransform = plane.transform;
		pTransform.parent = this.transform;
		Vector3 parentPos = this.transform.position;
		Vector3 newPos = new Vector3(parentPos.x, parentPos.y, parentPos.z);
		Vector3 rotation = Vector3.zero;

		switch(side){
		case "top":
			newPos.y = newPos.y + this.transform.localScale.y/2.0f + planeOffset;
			rotation = Vector3.zero;
			break;
		case "bottom":
			newPos.y = newPos.y - this.transform.localScale.y/2.0f - planeOffset;
			rotation = new Vector3(0,0,180);
			break;
		case "right":
			newPos.x = newPos.x + this.transform.localScale.x/2.0f + planeOffset;
			rotation = new Vector3(0,0,270);
			break;
		case "left":
			newPos.x = newPos.x - this.transform.localScale.x/2.0f - planeOffset;
			rotation = new Vector3(0,0,90);
			break;
		case "back":
			newPos.z = newPos.z + this.transform.localScale.z/2.0f + planeOffset;
			rotation = new Vector3(90,0,0);
			break;
		case "front":
			newPos.z = newPos.z - this.transform.localScale.z/2.0f - planeOffset;
			rotation = new Vector3(270,0,0);
			break;
		}

		pTransform.position = newPos;
		plane.transform.Rotate(rotation);

		pTransform.localScale = Vector3.one*.1f;
		plane.GetComponent<Renderer>().material.mainTexture = texture;
		//plane.GetComponent<Renderer>().material.mainTexture = Resources.Load(resourcePath, typeof(Texture)) as Texture;
		TileTextureScaler tts = plane.gameObject.AddComponent<TileTextureScaler>();
		tts.CreateComponent(side);

		return plane;
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
