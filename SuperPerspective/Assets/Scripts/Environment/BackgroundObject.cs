using UnityEngine;
using System.Collections;

public class BackgroundObject : MonoBehaviour {

	#pragma warning disable 219
	
	// Update is called once per frame
	void Update () {
		GameObject me = this.gameObject;
		Color tint = new Color(0, 0.5f, 1f, 0.8f);
		if(!GameStateManager.is3D()){
			Renderer[] rs = me.GetComponentsInChildren<Renderer>();
			foreach(Renderer r in rs){
				Material m = r.material;
				Color c = new Color(m.color.r, m.color.g, m.color.b, 0.5f);
				//m.color = c;

				m.SetColor ("_TintColor", tint);

				//print(m.color);
			}
		}
	}
}
