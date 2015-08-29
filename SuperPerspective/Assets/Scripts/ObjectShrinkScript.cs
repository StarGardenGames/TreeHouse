using UnityEngine;
using System.Collections;

public class ObjectShrinkScript : MonoBehaviour {
	public float lifeTimer = 3;
	public float targetScale = 0.0001f;
	public float destroySize = .01f;
	public float shrinkSpeed = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(lifeTimer > 0){
			lifeTimer-= Time.deltaTime;
		}
		else{
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime*shrinkSpeed);
			if(transform.localScale.x < destroySize){
				Destroy(gameObject);
			}
		}
	}
}
