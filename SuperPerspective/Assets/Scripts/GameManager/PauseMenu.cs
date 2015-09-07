using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	bool menuVisible = false;
	float menuAlpha = 0f;
	Canvas menu;
	float fadeTime = .3f;
	
	//init settings
	void Start () {
		//Find menu
		menu = transform.GetChild(0).GetComponent<Canvas>();
		//link toggle pause to pause button
		InputManager.instance.PausePressedEvent += TogglePauseMenu;
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
	
	public void TogglePauseMenu(){
		menuVisible = !menuVisible;
	}
	
	public void RaisePauseEvent(){
		InputManager.instance.ContinuePressed();
	}
}
