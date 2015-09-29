using UnityEngine;
using System.Collections;

//This is an object only visible in 2D... unless you tick on the visibleIn3D bool
public class Only2D : MonoBehaviour {
	public bool visibleIn3D = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		SpriteRenderer sR = this.gameObject.GetComponent<SpriteRenderer>();
		Renderer r = this.gameObject.GetComponent<Renderer>();

		if(!visibleIn3D && GameStateManager.is3D()){
			if(sR != null){
				sR.enabled = false;			
			}
			if(r != null){
				r.enabled = false;			
			}
		}else{
			if(sR != null){
				sR.enabled = true;			
			}
			if(r != null){
				r.enabled = true;			
			}
		}
	}
}
