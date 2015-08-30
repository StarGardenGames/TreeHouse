using UnityEngine;
using System.Collections;

public class ConsoleActionsManager : MonoBehaviour {
	GameObject player;
	PlayerSpawnController psc;

	// Use this for initialization
	void Start () {
		this.init();

		
	
	}

	void init(){
		player = GameObject.FindWithTag("Player");
		psc = player.GetComponent<PlayerSpawnController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void resetPlayer(){
		psc.moveToDoor(psc.getDefaultDest());
	}
}
