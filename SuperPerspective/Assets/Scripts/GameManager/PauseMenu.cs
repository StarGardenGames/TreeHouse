using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public static PauseMenu instance;

	bool menuVisible = false;
	float menuAlpha = 0f;
	Canvas menu;
	float fadeTime = .3f;
	
	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(instance);
	}
	
	//init settings
	void Start () {
		//Find menu
		menu = transform.GetChild(0).GetComponent<Canvas>();
	}
	
	//called every frame
	void Update () {
		//enable/disable canvas component
		menu.GetComponent<Canvas>().enabled = (menuAlpha != 0f);
		//update alpha
		menuAlpha += ((menuVisible)? (1/fadeTime) : -(1/fadeTime))*Time.deltaTime;
		menuAlpha = Mathf.Clamp(menuAlpha,0f,1f);
		menu.GetComponent<CanvasGroup>().alpha = menuAlpha;
	}
	
	public void UpdateMenuVisible(bool visible){
		menuVisible = visible;
	}
}
