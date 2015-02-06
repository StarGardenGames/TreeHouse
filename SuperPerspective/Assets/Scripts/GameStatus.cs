using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {

	//singleton
	public static GameStatus instance;

	//check if paused
	//note, marked private because I only want it to be modified in this class
	public static bool paused = false;


	void Awake(){
		//singleton
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
	}

	void Update(){
		/*considerations:
			pausedWhenMenu is visible
			pausedWhen cam isn't locked to player
		*/
		paused = !CameraControl.instance.isLockedToPlayer() || CheckpointManager.instance.menuVisible;
	}

	public bool isPaused(){
		return paused;
	}


}
