using UnityEngine;
using System.Collections;

//represents classes which interact with player
public class PlayerInteractable : MonoBehaviour {
	//called by player when object collides with it
	public virtual void CollisionWithPlayer(){}
}
