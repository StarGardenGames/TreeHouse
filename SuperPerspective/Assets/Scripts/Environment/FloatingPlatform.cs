using UnityEngine;
using System.Collections;

public class FloatingPlatform : MonoBehaviour {
		
		#pragma warning disable 414	
	
		private Vector3 origin;
		private bool returning = false;
		private float precision = 0.1f;

		private void Start(){

		}

		private void Update(){
			//CollisionChecker collide = new CollisionChecker(this.transform.FindChild("Core Block").getComponent<Collider>());
		}
}
