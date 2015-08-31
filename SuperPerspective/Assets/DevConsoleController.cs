using UnityEngine;
using System.Collections;

public class DevConsoleController : MonoBehaviour {

	GameObject devConsole;

	// Use this for initialization
	void Start () {
		this.gameObject.transform.position = new Vector3(0, 0, 0);

		devConsole = GameObject.Find("Console Menu");
		InputManager.instance.DevConsoleEvent += ToggleDevConsole;
		devConsole.SetActive(false);

	}

	public void ToggleDevConsole(){
		if(isConsoleActive()){
			devConsole.SetActive(false);
		}else{
			devConsole.SetActive(true);
		}
	}

	public bool isConsoleActive(){
		return devConsole.activeInHierarchy;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
