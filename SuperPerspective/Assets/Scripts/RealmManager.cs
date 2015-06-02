using UnityEngine;
using System.Collections;

public class RealmManager : Activatable {
	
	static int dimension=RED;
	
	int leftDimension=0;
	
	const int BLUE=0;
	const int RED=1;
	const int NOT_DASHED=0;
	const int DASHED=1;

	public Material[] materials=new Material[4];
	
	static GameObject player;

	bool hor=false;

	public GameObject[] reds,blues;

	GameObject on, off;
	
	// Use this for initialization
	void Awake () {
		hor=transform.localScale.z>transform.localScale.y;

		player=GameObject.Find("Player");
		
		//update stuff
		updateBlocks();
	}
	
	
	public override void setActivated(bool a){
		dimension = a?RED:BLUE;
		updateBlocks();
	}
	
	void updateBlocks(){
		if(dimension==BLUE){
			foreach(GameObject r in reds){
				r.GetComponent<Renderer>().material=materials[RED*2+DASHED];
				r.GetComponent<Collider>().enabled=false;
			}
			foreach(GameObject b in blues){
				b.GetComponent<Renderer>().material=materials[BLUE*2+NOT_DASHED];
				b.GetComponent<Collider>().enabled=true;
			}
		}
		
		//changing to red dimension
		if(dimension==RED){
			foreach(GameObject b in blues){
				b.GetComponent<Renderer>().material=materials[BLUE*2+DASHED];
				b.GetComponent<Collider>().enabled=false;
			}
			foreach(GameObject r in reds){
				r.GetComponent<Renderer>().material=materials[RED*2+NOT_DASHED];
				r.GetComponent<Collider>().enabled=true;
			}
		}
	}
	
	public static void Reset() {
		dimension = RED;
	}
}
