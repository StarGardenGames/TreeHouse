using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	//public Door dest;
	public string myName;
	public string destName;
	Door destDoor;
	public Color particleColor;
	
	public void Awake(){
		//update particle color
		ParticleSystem p = transform.GetChild(1).GetComponent<ParticleSystem>();
		p.startColor = particleColor;
		p.Simulate(2f);
		p.Play();
	}

	public override void Triggered(){
		if(destDoor!=null)
			player.GetComponent<PlayerController>().Teleport(
				destDoor.transform.position + new Vector3(0,0,-2));
		else
			Debug.Log("Door not linked");
	}
	
	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}
	
}

