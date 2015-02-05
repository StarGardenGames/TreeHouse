using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckpointManager : MonoBehaviour {
	static int destination = -1;

	public Canvas menu;
	public GameObject marker;
	public Camera playerCam;
	Image map;

	public static CheckpointManager instance;
	public bool menuVisible = false;

	GameObject[] buttons;

	static Vector2[] points = {
		new Vector2(-50,-20),
		new Vector2(150,50),
		new Vector2(0,0)
	};
	static bool[] pointReached = new bool[points.Length];
	string[] scenes ={
		"LevelTest",
		"LevelTest",
		"LevelTest"
	};

	float menuAlpha = 0;
	float fadeTime = 1f;

	void Awake(){
		//sigleton pattern
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);

		//TODO: read points from file 

		//move to destination if we fast travelled
		if(destination != -1){
			//manage camera
			CameraControl.instance.SetMount(playerCam.GetComponent<Transform>());
			//search for correct checkpoint
			GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
			GameObject player = GameObject.Find("Player");
			bool checkpointFound = false;
			for(int i = 0; i < cps.Length; i++){
				if(cps[i].GetComponent<Checkpoint>().id == destination){
					player.transform.position = cps[i].transform.position;
					checkpointFound = true;
				}
			}
			if(!checkpointFound){
				Debug.Log("You Lied >:( (i.e. Destination scene does not contain the correct checkpoint)");
			}
		}
	}

	// Use this for initialization
	void Start () {
		initMenu();
	}
	
	// Update is called once per frame
	void Update () {
		//check if we entered new room
		if(destination != -1 && menuVisible){
			menuAlpha = 1;
			destination = -1;
			CameraControl.instance.Snap();
		}
		//update alpha
		menuAlpha += ((menuVisible)? (1/fadeTime) : -(1/fadeTime))*Time.deltaTime;
		menuAlpha = Mathf.Clamp(menuAlpha,0f,1f);
		menu.GetComponent<CanvasGroup>().alpha = menuAlpha;

	}

	public void showMenu(int id){
		pointReached[id] = true;
		showMenu();
	}

	public void showMenu(){
		menuVisible = true;
		for(int i = 0; i < buttons.Length; i++)
			if(pointReached[i])
				buttons[i].SetActive(true);
	}

	public void exitMenu(){
		menuVisible = false;
		for(int i = 0; i < buttons.Length; i++)
			buttons[i].SetActive(false);
	}

	void initMenu(){
		buttons = new GameObject[points.Length];
		//get map
		map = menu.transform.FindChild("Map").GetComponent<Image>();
		float mapX = map.transform.position.x;
		float mapY = map.transform.position.y;
		//add checkpoints to map
		for(int i= 0; i < points.Length;i++){
			//create button
			buttons[i] = Instantiate(marker, new Vector3(0,0,0), Quaternion.identity) as GameObject;
			//position
			buttons[i].transform.SetParent(map.transform);
			Vector3 pos = buttons[i].transform.position;
			pos.x = mapX + points[i].x;
			pos.y = mapY + points[i].y;
			buttons[i].transform.position = pos;
			//ad listenes
			int j = i;
			buttons[i].GetComponent<Button>().onClick.AddListener(() => { goToCheckPoint(j);});
			buttons[i].SetActive(false);
		}
	}

	void goToCheckPoint(int id){
		destination = id;
		exitMenu();
		Application.LoadLevel(scenes[id]);
	}

}
