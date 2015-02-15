using UnityEngine;
using System.Collections;

public class Unchild : MonoBehaviour {
	void Awake(){
		gameObject.GetComponent<Transform>().DetachChildren();
		Destroy(gameObject);
	}

}
