using UnityEngine;
using System.Collections;

public class PlatformWobble : LandOnObject {
	Vector3 origin, targetPos;
	bool isWobbling;

	public bool wobbleOnce;

	public int defaultWobbleTime;
	int wobbleTimer;

	public float floatStrength;
	float currStrength;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(isWobbling && wobbleTimer>0){
    		wobbleTimer--;
    		float strengthRemaning = ((float)wobbleTimer / (float)defaultWobbleTime) * floatStrength;//move closer to zero
    		float x = Mathf.PI * Time.time;
    		float p = strengthRemaning * Mathf.Sin(x);
    		transform.position = new Vector3(transform.position.x, origin.y - p, transform.position.z);

		}

		if(wobbleTimer <= 0){	
			if(wobbleOnce && isWobbling){
				defaultWobbleTime = 0;
			}
			wobbleTimer = 0;
			isWobbling = false;

		}
	
	}

	public override void LandedOn() {
		if (!isWobbling){
			currStrength = floatStrength;
			isWobbling = true;
			wobbleTimer = defaultWobbleTime;//TODO: take in force of landing 

			origin = transform.position;
		}
	}
}
