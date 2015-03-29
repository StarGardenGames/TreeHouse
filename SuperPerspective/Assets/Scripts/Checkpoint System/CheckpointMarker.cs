using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CheckpointMarker : Button {

	public Sprite sprDef;
	public Sprite sprHover;
	public Sprite sprSelected;
	
	public override void OnPointerEnter(PointerEventData data){
		//Debug.Log("Enter");
		gameObject.GetComponent<Image>().sprite = sprHover;
	}

	void OnMouseExit(){
		gameObject.GetComponent<Image>().sprite = sprDef;
	}

	public override void OnPointerClick(PointerEventData data){
		//Debug.Log("Clicked");
	}
}
