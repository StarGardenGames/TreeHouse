using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {
	
	public GameObject[] collisionParents;
	public GameObject[] artParents;
	
	public bool collisionLayerVisible = false;
	public bool artLayerVisible = true;
		
	void Awake () {
		for(int i = 0; i < collisionParents.Length; i++)
			collisionParents[i].SetActive(true);
		for(int i = 0; i < artParents.Length; i++)
			artParents[i].SetActive(true);
		if(!collisionLayerVisible){
			for(int i = 0; i < collisionParents.Length; i++)
				makeChildrenInvisible(collisionParents[i]);		
		}
		if(!artLayerVisible){
			for(int i = 0; i < artParents.Length; i++)
				makeChildrenInvisible(artParents[i]);
		}
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
