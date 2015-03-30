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
	
	
	public virtual void FixedUpdate(){
		//do collision checking and make calls to methods
	}
	
	//called by player when object collides with it
	public virtual void EnterCollisionWithPlayer(){
		EnterCollisionWithGeneral();
	}
	
	//called by player when it leaves collision
	public virtual void ExitCollisionWithPlayer(){
		ExitCollisionWithGeneral();
	}
	
	//called when something in general collides with
	public virtual void EnterCollisionWithGeneral(){}
	
	//called when something in general collidios with 
	public virtual void ExitCollisionWithGeneral(){}
}
