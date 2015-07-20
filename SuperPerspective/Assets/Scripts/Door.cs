using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	//public Door dest;
	public string myName;
	public string destName;
	Door destDoor;
	public Color particleColor;
	public AudioSource warp;
	
	public void Awake(){
		//update particle color
		ParticleSystem p = transform.GetChild(1).GetComponent<ParticleSystem>();
		p.startColor = particleColor;
		p.Simulate(2f);
		p.Play();

		//Adding in reference to door mesh, where I added in the warp sound -Nick
		AudioSource[] comps = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource comp in comps)
		{
			if ( comp.gameObject.name == "DoorMesh" )
			{
				warp = comp;
				warp.volume = 0.35f;
				break;
			}
		}
		//Debug.Log (warp);
	}

	public override void Triggered(){
		if (destDoor != null) {
			player.GetComponent<PlayerController> ().Teleport (
				destDoor.transform.position + new Vector3 (0, 0, -2));
			//Playing warp sound -Nick
			warp.Play ();
		}
		else
			Debug.Log("Door not linked");
	}
	
	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}
	
}

