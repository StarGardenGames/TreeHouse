using UnityEngine;
using System.Collections;

public class GateEffects : MonoBehaviour {
	public string gateType;
	public string gateText;

	// Use this for initialization
	void Start () {
		//gateType changes the sprite icon
		TextMesh gateTextMesh = this.transform.FindChild("Gate Text").GetComponent<TextMesh>();
		gateTextMesh.text = this.gateText;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
