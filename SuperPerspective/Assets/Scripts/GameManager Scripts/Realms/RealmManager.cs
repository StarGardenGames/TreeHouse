using UnityEngine;
using System.Collections;

public class RealmManager : MonoBehaviour {
	
	public static RealmManager instance;
	
	static int dimension=BLUE;
	
	int leftDimension=0;
	
	const int BLUE=0;
	const int RED=1;
	const int NOT_DASHED=0;
	const int DASHED=1;

	public Material[] materials=new Material[4];
	
	static GameObject player;

	bool hor=false;

	GameObject[] reds,blues;

	GameObject on, off;
	
	
	
	// Use this for initialization
	void Awake () {
		//singleton
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		//
		hor=transform.localScale.z>transform.localScale.y;

		player=GameObject.Find("Player");
		
		getBlocks();
		
		//update stuff
		updateBlocks();
	}
	
	
	public void toggle(bool a){
		dimension = a?RED:BLUE;
		updateBlocks();
	}
	
	void getBlocks(){
		GameObject[] trackers = GameObject.FindGameObjectsWithTag("RealmTracker");
		
		//determine lengths
		int blue_length = 0;
		int red_length = 0;
		foreach(GameObject tracker in trackers){
			RealmTracker comp = tracker.GetComponent<RealmTracker>();
			red_length += comp.reds.Length;
			blue_length += comp.blues.Length;
		}
		
		//init arrays
		reds = new GameObject[red_length];
		blues = new GameObject[blue_length];
		int red_index = 0;
		int blue_index = 0;
		//populate arrays
		foreach(GameObject tracker in trackers){
			RealmTracker comp = tracker.GetComponent<RealmTracker>();
			foreach(GameObject blue in comp.blues){
				blues[blue_index] = blue;
				blue_index++;
			}
			foreach(GameObject red in comp.reds){
				reds[red_index] = red;
				red_index++;
			}
		}
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
