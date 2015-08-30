using UnityEngine;
using System.Collections;

public class PlayerSpawnController : MonoBehaviour {

	public bool startAtDoor = true;
	public string startDoorName;
	Door destDoor;

	void Start () {
		destDoor = this.findDoor(startDoorName);
		this.moveToDoor(destDoor);
	}

	//spawn player at door
	public void moveToDoor(Door doorObject){
		if(doorObject != null && startAtDoor)
			this.gameObject.GetComponent<PlayerController>().Teleport(
				doorObject.transform.position + new Vector3(0,0,-2));
	}

	//find door with name
	public void findDoor(string doorName){
		Door[] doorList = Object.FindObjectsOfType(
			typeof(Door)) as Door[];

		foreach(Door door in doorList){
			if(door.getName() == startDoorName){
				return door;
			}
		}
	}

	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
