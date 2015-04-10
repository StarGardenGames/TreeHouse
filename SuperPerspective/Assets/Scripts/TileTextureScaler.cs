using UnityEngine;
using System.Collections;
/*
 * This script changes the tiling of a given material based on its scale.
 * Works with six sided cubes that have a gameobject to have individual textures for each side. 
 * */
public class TileTextureScaler : MonoBehaviour {

	// Use this for initialization
	public void CreateComponent(string side) {
		float multiplier = .5f;//default 1
		//scale texture to the following size
		float scaleX = (this.transform.parent.localScale.x)*multiplier;//x scale
		float scaleY = (this.transform.parent.localScale.y)*multiplier;//y scale
		float scaleZ = (this.transform.parent.localScale.z)*multiplier;//y scale
		//apply to face

		switch(side){
		case "top":
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleZ);
			break;
		case "bottom":
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleZ);
			break;
		case "left":
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleZ, scaleY);
			break;
		case "right":
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleZ, scaleY);
			break;
		case "back":
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleY);
			break;
		case "front":
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX, scaleY);
			break;
		}

	}
}
