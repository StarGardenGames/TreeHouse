using UnityEngine;
using System.Collections;
using System;

public class LightingManager : MonoBehaviour {
	
	#pragma warning disable 168

	public float lightTransitionTime = 2;	
	
	Light[][] lights;
	float[][] intensities;
	
	float lightTransitionProgress;	
	bool transitioning = false;
	PerspectiveType targetPerspective = PerspectiveType.p3D;
	
	#region Initialization

	void Start () {
		loadLightsFromScene();
		initStartingIntensities();
		disableIrrelevantLights();
		linkFunctions();
	}
	
	private void loadLightsFromScene(){
		GameObject[][] light_objs = new GameObject[2][];
		light_objs[(int)PerspectiveType.p2D] = GameObject.FindGameObjectsWithTag("Light_2D");
		light_objs[(int)PerspectiveType.p3D] = GameObject.FindGameObjectsWithTag("Light_3D");
		
		try{
			lights = new Light[2][];
			for(int p = 0; p < 2; p++){
				lights[p] = new Light[light_objs[p].Length];
				for(int i = 0; i < light_objs[p].Length; i++){
					lights[p][i] = light_objs[p][i].GetComponent<Light>();
				}
			}
		}catch(Exception e){
			Debug.Log("Something's tagged as a light but doesn't have a light component");
		}	
	}
	
	private void initStartingIntensities(){
		intensities = new float[2][];
		for(int p = 0; p < 2; p++){
			intensities[p] = new float[lights[p].Length];
			for(int i = 0; i < intensities[p].Length; i++){
				intensities[p][i] = lights[p][i].intensity;
			}
		}
	}
	
	private void linkFunctions(){
		GameStateManager.instance.PerspectiveShiftSuccessEvent += startLightTransition;
	}	
	
	#endregion Initialization
	
	#region Transition Handlers
	
	private void startLightTransition(){
		transitioning = true;
		targetPerspective = GameStateManager.instance.currentPerspective;
		lightTransitionProgress = 0;
	}
	
	void FixedUpdate(){
		if (transitioning){
			updateLightTransition();
			checkForTransitionEnd();
		}
	}
	
	private void updateLightTransition(){
		lightTransitionProgress += Time.deltaTime;
		float percentComplete = ((float)lightTransitionProgress ) / lightTransitionTime;
		
		int targetIndex = (int)targetPerspective;
		for(int i = 0; i < lights[targetIndex].Length; i++)
			lights[targetIndex][i].intensity = intensities[targetIndex][i] * percentComplete;
		
		int otherIndex = 1 - targetIndex; 
		for(int i = 0; i < lights[otherIndex].Length; i++)
			lights[otherIndex][i].intensity = intensities[otherIndex][i] * (1 - percentComplete); 
	}
	
	private void checkForTransitionEnd(){
		if(lightTransitionProgress > lightTransitionTime){
			transitioning = false;
			disableIrrelevantLights();
			enableRelevantLights();
		}
	}
	
	private void disableIrrelevantLights(){
		int perspectiveIndex = 1 - (int) GameStateManager.instance.currentPerspective;
		for(int i = 0; i < lights[perspectiveIndex].Length; i++){
			lights[perspectiveIndex][i].intensity = 0;
		}
	}
	
	private void enableRelevantLights(){
		int perspectiveIndex = (int) GameStateManager.instance.currentPerspective;
		for(int i = 0; i < lights[perspectiveIndex].Length; i++){
			lights[perspectiveIndex][i].intensity = intensities[perspectiveIndex][i]	;
		}
	}
	
	#endregion Transition Handlers
}