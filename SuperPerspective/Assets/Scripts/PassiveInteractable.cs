using UnityEngine;
using System.Collections;

//represents classes which interact with player
public class PassiveInteractable : MonoBehaviour {
	//player
	protected GameObject player;
			
	//current object which we're colliding with
	GameObject collisionObj;
	
	//at start find player
	public virtual void Start(){
		//find player
		player = PlayerController.instance.gameObject;
	}
	
	//called by player when object collides with it
	public virtual void EnterCollisionWithPlayer(){
		EnterCollisionWithGeneral(player);
	}
	
	//called by player when it leaves collision
	public virtual void ExitCollisionWithPlayer(){
		ExitCollisionWithGeneral(player);
	}
	
	//called when something in general collides with
	public virtual void EnterCollisionWithGeneral(GameObject other){}
	
	//called when something in general collidios with 
	public virtual void ExitCollisionWithGeneral(GameObject other){}
}
