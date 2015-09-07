using UnityEngine;
using System.Collections;

public class NestedPrefabsManager : MonoBehaviour {

	public GameObject[] prefabList;

	// Use this for initialization
	void Start () {
		foreach(GameObject g in prefabList){
			if(g != null){
				Instantiate(g, Vector3.zero, Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
