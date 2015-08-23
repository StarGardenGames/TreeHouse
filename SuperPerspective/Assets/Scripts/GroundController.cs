using UnityEngine;
using System.Collections;

public class GroundController : MonoBehaviour {
	
	Transform tex;
	public Vector2 scaleValue;
		
	// Use this for initialization
	void Start () {
		Vector3 ls = this.transform.Find("Ground").transform.localScale;
		tex = this.transform.Find("Ground Texture");

		var newScale = new Vector2(ls.x * scaleValue.x, ls.z * scaleValue.y);
		tex.GetComponent<Renderer>().material.mainTextureScale = new Vector2(newScale.x, newScale.y);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
