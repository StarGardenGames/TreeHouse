using UnityEngine;
using System.Collections;

public class ParentedArtElement : MonoBehaviour {
	public GameObject parentedElement;
	
	void Start () {
		transform.parent = parentedElement.transform;
	}
}
