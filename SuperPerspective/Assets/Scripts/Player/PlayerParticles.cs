using UnityEngine;
using System.Collections;

public class PlayerParticles : MonoBehaviour {

	private PlayerController player;

	public ParticleSystem dustEmitter;

	void Start () {
		initPlayerReference();
		initEmitters();
	}
	
	private void initPlayerReference(){ player = PlayerController.instance; }
	
	private void initEmitters(){ dustEmitter.enableEmission = false; }
	
	
	void FixedUpdate () {
		if(!player.isDisabled())
			updateParticleEmission();
	}
	
	private void updateParticleEmission(){
		dustEmitter.enableEmission =
			(player.isRunning() || player.isWalking()) && player.isGrounded();
	}
}
