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

	public override float GetDistance() {
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
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

