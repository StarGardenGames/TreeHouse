using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConsoleActionsManager : MonoBehaviour {
	GameObject canvas;
	GameObject player;
	PlayerSpawnController psc;
	InputField dcl;


	public UnityEngine.UI.Button buttonInstance;

	// Use this for initialization
	void Start () {
		this.init();
		this.buildDoorButtons();
	}

	void buildDoorButtons(){
		Door[] doorList = Object.FindObjectsOfType(
			typeof(Door)) as Door[];

		//generates a teleport button for each door
		// foreach(Door door in doorList){
		// 	Button b = Object.Instantiate(buttonInstance, Vector3.zero, Quaternion.identity) as Button;
		// 	b.transform.SetParent(canvas.transform);
		// 	Vector2 bPosDelta = new Vector2(100 * 1, 50);
		// 	b.sizeDelta = bPosDelta;
		// }
	}

	void init(){
		canvas = GameObject.Find("Console Menu") as GameObject;
		player = GameObject.FindWithTag("Player");
		psc = player.GetComponent<PlayerSpawnController>();
		dcl = GameObject.Find("DevCommandLine").GetComponent<InputField>();
		Debug.Log(dcl);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Enter") && dcl.text != ""){
			// dcl.text = "";
			this.consoleCommand(dcl.text);
		}
	}

	//will do something based on the command (param c)
	public void consoleCommand(string c){
		// Debug.Log("executing command " + c + "...");
		string[] commandArray = c.Split(" "[0]);

		string debugArray = "";
		foreach(string s in commandArray){
			debugArray = debugArray + s + ", ";
		}
		// Debug.Log(debugArray);

		if(commandArray.Length >= 2){
			string command = commandArray[0];
			string val = commandArray[1];

			if(command != null && val != null){
				switch(command){
					case "tp":
						Debug.Log("... moving player to door " + val);
						movePlayer(val);
						break;
					default:
						Debug.Log("...command " + command + " not found ");
						break;
				}
			}else{
				Debug.Log("missing command or value");
			}
		}
	}

	//moves player to specific door
	public void movePlayer(string doorName){
		psc.moveToDoor(
			psc.findDoor(doorName)
		);
	}

	//sets player to their default door position
	public void resetPlayer(){
		psc.moveToDoor(psc.getDefaultDest());
	}
}
