using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour {

	public static SaveManager instance;

	public static int currentSave = 0;
	public static int recentPoint = 0;

	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
	}

	void Start(){
		string pr = PlayerPrefs.GetString(currentSave + "_pointsReached");
		if(pr == "" || pr.Length!=CheckpointManager.instance.getNumCheckpoints())
			for(int i = 0; i < 3; i++ )
				resetSave(i);
	}

	//called from menu when Play is clicked
	public void loadSave(){
		//init points reached
		string pointsReachedStr = PlayerPrefs.GetString(currentSave + "_pointsReached");
		char[] chars = pointsReachedStr.ToCharArray(0,pointsReachedStr.Length);
		bool[] pointsReached = new bool[pointsReachedStr.Length];
		if(pointsReached.Length != 0){
			for(int i = 0; i < pointsReached.Length; i++){
				pointsReached[i] = (chars[i] - '0') > 0; 
			}
			CheckpointManager.instance.setPointReached(pointsReached);
		}
		//Go to recent point
		int recentPoint = PlayerPrefs.GetInt(currentSave + "_recentPoint",-1);
		if(recentPoint != -1)
			CheckpointManager.instance.goToCheckPoint(recentPoint);
		else
			CameraControl.instance.MountToPlayer();
	}
	
	public void setSave(int i){
		currentSave = i;
	}

	public void setSaveName(int i, string name){
		PlayerPrefs.SetString(i+"_name",name);
	}

	public string getSaveName(int i){
		return PlayerPrefs.GetString(i+"_name");
	}

	
	public void addPointReached(int point){
		string pointsReachedStr = PlayerPrefs.GetString(currentSave + "_pointsReached");
		char[] chars = pointsReachedStr.ToCharArray(0,pointsReachedStr.Length);
		chars[point] = '1';
		PlayerPrefs.SetString(currentSave + "_pointsReached",new string(chars));
	}

	public void setRecentPoint(int r){
		PlayerPrefs.SetInt(currentSave + "_recentPoint",r);
	}

	public void resetSave(int i){
		int numCheckpoints = CheckpointManager.instance.getNumCheckpoints();
		char[] reached = new char[numCheckpoints];
		for(int j = 0; j < numCheckpoints; j++)
			reached[j] = '0';
		string pointsReached = new string(reached);
		PlayerPrefs.SetString(i + "_pointsReached",pointsReached);
		PlayerPrefs.SetInt(i + "_recentPoint",-1);
		PlayerPrefs.SetString(i+"_name","");
	}
}
