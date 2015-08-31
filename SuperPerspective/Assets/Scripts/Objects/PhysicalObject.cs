using UnityEngine;
using System.Collections;

public class PhysicalObject : MonoBehaviour {
	protected Vector3 velocity;

	// TODO potentially add grounded, gravity, and terminalvelocity
	// TODO potentially add collision checking
	
	void Start(){
		Init();
	}
	
	protected void Init(){
		velocity = Vector3.zero;
	}
	
	public Vector3 getVelocity(){
		return velocity;
	}
	public void setVelocity(Vector3 velocity){	
		this.velocity = velocity;
	}
}
