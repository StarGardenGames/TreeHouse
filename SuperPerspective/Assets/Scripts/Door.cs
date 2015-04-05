using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	public Door dest;
	public Color particleColor;
	
	public void Awake(){
		//update particle color
		ParticleSystem p = transform.GetChild(1).GetComponent<ParticleSystem>();
		p.startColor = particleColor;
		p.Simulate(2f);
		p.Play();
	}

	public override void Triggered(){
		player.GetComponent<PlayerController>().Teleport(dest.transform.position + new Vector3(0,0,-2));
	}
	
}

