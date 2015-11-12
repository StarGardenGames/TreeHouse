using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {
	
	public GameObject collisionParent;
	public GameObject artParent;
	
	public bool collisionLayerVisible = false;
	public bool artLayerVisible = true;
		
	void Awake () {
		collisionParent.SetActive(true);
		artParent.SetActive(true);
		if(!collisionLayerVisible) makeChildrenInvisible(collisionParent);
		if(!artLayerVisible) makeChildrenInvisible(artParent);
	}
	
	private void makeChildrenInvisible(GameObject par){
		bool parentExists = par!=null;
		if(parentExists){
			MeshRenderer[] renderableChildren = par.GetComponentsInChildren<MeshRenderer>();
			for(int i = 0; i < renderableChildren.Length; i++){
				renderableChildren[i].enabled = false;
			}
		}
	}
	
}
