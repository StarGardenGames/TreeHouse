using UnityEngine;
using System.Collections;
/*
 * This script changes the tiling of a given material based on its scale.
 * Works with six sided cubes that have a gameobject to have individual textures for each side. 
 * */
public class MaterialTiling : MonoBehaviour {
	public float scale;
	// Use this for initialization
	void Start () {
		findFaces ();
		//scale texture to the following size
		float scaleX = (gameObject.transform.localScale.x)/scale;//x scale
		float scaleY = (gameObject.transform.localScale.y)/scale;//y scale
		float scaleZ = (gameObject.transform.localScale.z)/scale;//y scale
		//apply to each face
		GameObject backFace = getChildGameObject (gameObject, "Back");
		backFace.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleY);

		GameObject bottomFace = getChildGameObject (gameObject, "Bottom");
		bottomFace.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleZ);

		GameObject frontFace = getChildGameObject (gameObject, "Front");
		frontFace.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleY);

		GameObject leftFace = getChildGameObject (gameObject, "Left");
		leftFace.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleZ, scaleY);

		GameObject rightFace = getChildGameObject (gameObject, "Right");
		rightFace.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleZ, scaleY);

		GameObject topFace = getChildGameObject (gameObject, "Top");
		topFace.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleZ);


	}
	//finds each face of this six sided cue
	void findFaces(){

	}
	//finds a gameobject in fromGameObject based on name, would be better served static
	GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		//Author: Isaac Dart, June-13.
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
