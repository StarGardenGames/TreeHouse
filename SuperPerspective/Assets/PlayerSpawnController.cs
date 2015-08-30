using UnityEngine;
using System.Collections;

public class PlayerSpawnController : MonoBehaviour {

	public bool startAtDoor = true;
	public string startDoorName;
	Door destDoor;

	void Start () {
		//find door with name
		Door[] doorList = Object.FindObjectsOfType(
			typeof(Door)) as Door[];

		foreach(Door door in doorList){
			if(door.getName() == startDoorName){
				destDoor = door;
				break;
			}
		}

		//spawn player at door
		if(destDoor != null && startAtDoor)
			this.gameObject.GetComponent<PlayerController>().Teleport(
				destDoor.transform.position + new Vector3(0,0,-2));
		else
			Debug.Log("Door not linked");


	}

	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
