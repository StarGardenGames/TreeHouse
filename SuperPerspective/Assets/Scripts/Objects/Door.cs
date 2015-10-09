using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	//public Door dest;
	public string myName;
	public string destName;
	Door destDoor;
	public Color particleColor;

	public bool isSceneLoad;
	
	public void Awake(){
		//update particle color
		ParticleSystem p = this.transform.FindChild("Particles").GetComponent<ParticleSystem>();
		p.startColor = particleColor;
		p.Simulate(2f);
		p.Play();
        range = 2;
	}

	public override float GetDistance() {
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
	}

	public override void Triggered(){
		if(isSceneLoad && destName != null)
			Application.LoadLevel(destName);

		else if(destDoor!=null)
			player.GetComponent<PlayerController>().Teleport(
				destDoor.GetComponent<Collider>().bounds.center + new Vector3(0,0,-2));
		else
			Debug.Log("Door not linked");
	}

	public string getName(){
		return myName;
	}
	
	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}
	
}

