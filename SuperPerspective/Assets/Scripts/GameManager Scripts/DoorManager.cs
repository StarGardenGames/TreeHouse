using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour {

	Dictionary<string,Door> doors = 
		new Dictionary<string,Door>();

	// Use this for initialization
	void Start () {
		//search all doors
		Door[] doorList = Object.FindObjectsOfType(
			typeof(Door)) as Door[];
		//fill up doors
		foreach(Door door in doorList){
			if(door.myName!="")
				doors.Add(door.myName,door);
		}
		//for each door assign destination
		foreach(Door door in doorList){
			Door destDoor;
			doors.TryGetValue(door.destName, out destDoor);
			door.setDoor(destDoor);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
