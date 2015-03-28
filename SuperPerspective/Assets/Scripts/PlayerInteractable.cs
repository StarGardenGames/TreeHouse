using UnityEngine;
using System.Collections;

//represents classes which interact with player
public class PlayerInteractable : MonoBehaviour {
	
	PlayerController3 player;
	float dist;
	float range = 3.0f;
	
	//at start find player
	void Start(){
		player = PlayerController3.instance;
		dist = Vector3.Distance(transform.position, player.gameObject.transform.position);
	}
	
	
	void FixedUpdate(){
		//check distance and determine if range methods need to be called
		float oldDist = dist;
		dist = Vector3.Distance(transform.position, player.gameObject.transform.position);
		//player left range
		if(oldDist < range && dist >= range)
			PlayerExitedRange();
		//player entered range
		if(oldDist > range && dist <= range)
			PlayerEnteredRange();
	}
	
	//called by player when object collides with it
	public virtual void CollisionWithPlayer(){}

	//called when player enters a certain range
	public virtual void PlayerEnteredRange(){}
	
	//called when player exits a certain range
	public virtual void PlayerExitedRange(){}
}
