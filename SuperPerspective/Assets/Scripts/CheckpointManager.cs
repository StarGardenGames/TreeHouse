using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckpointManager : MonoBehaviour {
	public Canvas menu;
	public Image targetGraphic;
	Image map;

	public static CheckpointManager instance;
	public bool menuVisible = false;

	Vector2[] points = {
		new Vector2(10,10),
		new Vector2(30,30)
	};
	bool[] pointReached;

	float menuAlpha = 0;
	float fadeTime = 1f;

	void Awake(){
		//sigleton pattern
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);

		//init pointReached
		pointReached = new bool[points.Length];

		//TODO: read points from file 
	}

	// Use this for initialization
	void Start () {
		initMenu();
	}
	
	// Update is called once per frame
	void Update () {
		//update alpha
		menuAlpha += ((menuVisible)? (1/fadeTime) : -(1/fadeTime))*Time.deltaTime;
		menuAlpha = Mathf.Clamp(menuAlpha,0f,1f);
		menu.GetComponent<CanvasGroup>().alpha = menuAlpha;
	}

	public void showMenu(int id){
		menuVisible = true;
		pointReached[id] = true;
	}

	public void exitMenu(){
		menuVisible = false;
	}

	void initMenu(){
		//get map
		map = menu.transform.FindChild("Map").GetComponent<Image>();
		float mapX = map.transform.position.x - map.transform.lossyScale.x/2f;
		float mapY = map.transform.position.y - map.transform.lossyScale.y/2f;
		//add checkpoints to map
		for(int i= 0; i < points.Length;i++){
			//create button
			GameObject button = new GameObject("cp");//checkpoint obj
			button.AddComponent<Button>();
			button.AddComponent<Image>();
			button.GetComponent<Button>().targetGraphic = button.GetComponent<Image>();
			//position
			button.transform.parent = map.transform;
			Vector3 pos = button.transform.position;
			pos.x = mapX + points[i].x;
			pos.y = mapY + points[i].y;
			button.transform.position = pos;
		}
	}

	void goToCheckPoint(int id){
		Debug.Log("Going to "+id);
	}

}
