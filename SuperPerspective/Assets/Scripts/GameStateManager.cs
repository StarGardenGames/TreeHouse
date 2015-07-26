using UnityEngine;
using System.Collections;

using SuperPerspective.Singleton;

/// <summary>
///     This class is in charge of controlling the game state transitions. 
///     It receives input from InputManager, and tells the camera, player, and any other necessary objects of behavior changes.
///     This keeps all transitions logic in one location and allows the player, camera, and menu scripts to have simple behavior and remain modular.
/// </summary>
public class GameStateManager : PersistentSingleton<GameStateManager>
{
	//suppress warnings
	#pragma warning disable 414, 649
	
	#region Properties & Variables
	
	// State variables
	public ViewType currentState { get; private set; }
	public ViewType previousState { get; private set; }
	public ViewType targetState { get; private set; }
	//TODO combine PerspectiveType and Matrix4x4 to have same purpose
	public PerspectiveType currentPerspective { get; private set; }
	public bool paused {get; private set;}

	// Flip failure timer variables
	private float failureTime = .3f;    // How long the transition lasts before the flip fails and switches back
	private float failureTimer = 0f;    // The current timer used in the FailTimer coroutine

	// view mounts & settings
	private const int NUM_VIEW_TYPES = 8;
	private Transform[] view_mounts = new Transform[NUM_VIEW_TYPES];
	private PerspectiveType[] view_perspectives = new PerspectiveType[NUM_VIEW_TYPES];
	
	// Events to notify listeners of state changes
	public event System.Action<bool> GamePausedEvent;
	public event System.Action<PerspectiveType> PerspectiveShiftEvent;//called at end  of shift
	//either PerspectiveShiftSuccessEvent or PerspectiveShiftFailEvent will be call at the beginning of shifts
	public event System.Action PerspectiveShiftSuccessEvent;
	public event System.Action PerspectiveShiftFailEvent;
	
	#endregion Properties & Variables
	
	#region Managing Backward Movement
	void Update(){
		if(currentPerspective == PerspectiveType.p3D){
			if(InputManager.instance.GetForwardMovement() == -1 && currentState == ViewType.STANDARD_3D){
				EnterState(ViewType.BACKWARD);
			}
			if(InputManager.instance.GetForwardMovement() == 1 && currentState == ViewType.BACKWARD){
				EnterState(ViewType.STANDARD_3D);
			}			
		}
	}
	#endregion Managing Backward Movement
	
	#region Monobehavior Implementation

	void Start () {
		InitViewPerspectives();
		InitViewMounts();
		
		//determine wheather or not to start on menu
		if(view_mounts[(int)ViewType.MENU] == null){
			currentPerspective = PerspectiveType.p2D;
			StartGame();
		}else{
			currentPerspective = PerspectiveType.p3D;
			EnterMenu();
		}

		// Register event handlers to InputManagers
		InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
		InputManager.instance.PausePressedEvent += HandlePausePressed;
		InputManager.instance.LeanLeftPressedEvent += HandleLeanLeftPressed;
		InputManager.instance.LeanRightPressedEvent += HandleLeanRightPressed;
		InputManager.instance.LeanLeftReleasedEvent += HandleLeanLeftReleased;
		InputManager.instance.LeanRightReleasedEvent += HandleLeanRightReleased;

		// Register to switch state to proper gameplay when shift is complete
		CameraController.instance.ShiftCompleteEvent += HandleShiftComplete;
	}
	
	void InitViewPerspectives(){
		view_perspectives[(int)ViewType.STANDARD_3D] =   PerspectiveType.p3D;
		view_perspectives[(int)ViewType.STANDARD_2D] =   PerspectiveType.p2D;
		view_perspectives[(int)ViewType.PAUSED] =        PerspectiveType.p3D;  
		view_perspectives[(int)ViewType.MENU] =          PerspectiveType.p2D; 
		view_perspectives[(int)ViewType.LEAN_LEFT] =		 PerspectiveType.p3D; 
		view_perspectives[(int)ViewType.LEAN_RIGHT] =	 PerspectiveType.p3D; 
		view_perspectives[(int)ViewType.BACKWARD] =		 PerspectiveType.p3D;
	}
	
	void InitViewMounts(){
		// Gather mount gameobjects
		GameObject[] mounts = new GameObject[NUM_VIEW_TYPES];
		mounts[0] = GameObject.Find("3DCameraMount");
		mounts[1] = GameObject.Find("2DCameraMount");
		mounts[2] = GameObject.Find("MenuMount");
		mounts[3] = GameObject.Find("PauseMount");
		mounts[4] = GameObject.Find("LeanLeftCameraMount");
		mounts[5] = GameObject.Find("LeanRightCameraMount");
		mounts[6] = GameObject.Find("BackwardCameraMount");
		
		//find transforms
		for(int i = 0; i < mounts.Length; i++){
			if(mounts[i] != null)
				view_mounts[i] = mounts[i].transform;
		}
	}

	#endregion Monobehavior Implementation

	#region State Change Functions
	
	private void EnterState(ViewType targetState){
		previousState = currentState;
		this.targetState = targetState;
		CameraController.instance.SetMount(view_mounts[(int)targetState],view_perspectives[(int)targetState]);
	}

	// Enter transition state between 2D and 3D or vice-versa
	private void EnterTransition(ViewType targetState){
		// Pause the game during the transition
		RaisePauseEvent(true);
		
		EnterState(targetState);
	}

	// Pause the game
	private void EnterPause(){
		EnterState(ViewType.PAUSED);
		RaisePauseEvent(true);
	}

	// Exits the pause state
	private void ExitPause(){
	  EnterState(previousState);
	  RaisePauseEvent(false);
	}

	private void EnterMenu(){
		EnterState(ViewType.MENU);
		RaisePauseEvent(true);
	}

	private void ExitMenu(){
		EnterState(previousState);
		RaisePauseEvent(false);
	}
	
   #endregion State Change Functions

	#region Event Handlers

	//TODO review
	private void HandlePausePressed(){   
		if (!IsPauseState(currentState)){
			// If the current state is a gameplay state pause the game
			EnterMenu();
		}else if (currentState == ViewType.MENU){
			// If the current state is paused then unpause
			ExitMenu();
		}
	}

	private void HandleShiftPressed(){
		// Shift perspective if current state is a gameplay state
		if (!IsPauseState(currentState)){
			// Find the target state to switch to
			ViewType newPerspective = (currentState == ViewType.STANDARD_2D) ? ViewType.STANDARD_3D : ViewType.STANDARD_2D;

			// Find the corresponding perspective to store for external reference
			currentPerspective = (newPerspective == ViewType.STANDARD_2D) ? PerspectiveType.p2D : PerspectiveType.p3D;

			// Begin transition to that state (since this involves the shift animation we use the transition state)
			EnterTransition(newPerspective);

			if (PlayerController.instance.Check2DIntersect()){
				RaisePerspectiveShiftFailEvent();
				StartCoroutine(FailTimer());
			}else{
				RaisePerspectiveShiftSuccessEvent();
			}
		}else{
			PlayerController.instance.Flip();
		}
	}

	// TODO Review (handle pause and handle menu are pretty much the same)
	private void HandleMenuEnterPressed(){
		if (!IsPauseState(currentState)){
			EnterMenu();
		}
		if (currentState == ViewType.MENU){
			ExitMenu();
		}
	}
    
	private void HandleShiftComplete(){
		currentState = targetState;
		
	
		RaisePerspectiveShiftEvent();
		
		if (!IsPauseState(currentState)){	
			// Unpause Game
			RaisePauseEvent(false);
		}
	}

	private void HandleLeanLeftPressed(){
		if(!IsPauseState(currentState) && currentPerspective == PerspectiveType.p3D)
			EnterState(ViewType.LEAN_LEFT);
	}
	
	private void HandleLeanRightPressed(){
		if(!IsPauseState(currentState) && currentPerspective == PerspectiveType.p3D)
			EnterState(ViewType.LEAN_RIGHT);
	}
	
	private void HandleLeanLeftReleased(){
		if(currentState == ViewType.LEAN_LEFT && currentPerspective == PerspectiveType.p3D)
			EnterState(ViewType.STANDARD_3D);
	}
	
	private void HandleLeanRightReleased(){
		if(currentState == ViewType.LEAN_RIGHT && currentPerspective == PerspectiveType.p3D)
			EnterState(ViewType.STANDARD_3D);
	}
  
	#endregion Event Handlers

   #region Event Raising Functions

	// Alert listeners that the game is being paused or unpaused
	private void RaisePauseEvent(bool paused){
		this.paused = paused;
		if (GamePausedEvent != null)
			GamePausedEvent(paused);
	}

	// Alert listeners that the perspective should shift
	private void RaisePerspectiveShiftEvent(){
		if (PerspectiveShiftEvent != null)
			PerspectiveShiftEvent(currentPerspective);
	}

	// Alert listeners that the perspective should shift
	private void RaisePerspectiveShiftSuccessEvent(){
		if (PerspectiveShiftSuccessEvent != null)
			PerspectiveShiftSuccessEvent();
	}

	// Alert listeners that the perspective should shift
	private void RaisePerspectiveShiftFailEvent(){
		if (PerspectiveShiftFailEvent != null)
			PerspectiveShiftFailEvent();
	}

	#endregion Event Raising Functions

	#region Public Interface

	// Called by main menu to begin gameplay
	public void StartGame(){
		EnterTransition(ViewType.STANDARD_2D);
		currentPerspective = PerspectiveType.p2D;
		RaisePauseEvent(false);
	}
	#endregion Public Interface

	#region Helper Functions

	private IEnumerator FailTimer(){
		failureTimer = 0f;

		while (failureTimer < failureTime){
			failureTimer += Time.deltaTime;
			yield return null;
		}

		// Find the corresponding perspective to store for external reference
		currentPerspective = (targetState == ViewType.STANDARD_2D) ? PerspectiveType.p3D : PerspectiveType.p2D;

		// Find the corresponding perspective to store for external reference
		if (targetState == ViewType.STANDARD_2D)
			EnterTransition(ViewType.STANDARD_3D);
		else
			EnterTransition(ViewType.STANDARD_2D);
	}

	private bool IsPauseState(ViewType targetState){
		return targetState == ViewType.PAUSED || targetState == ViewType.MENU;
	}
	
   #endregion

}

/// <summary>
///     Enumeration to dictate which perspective the current state is in. 
///     Objects can reference this and it is passed in shioft events so they know which behavior mode to execute.
/// </summary>
public enum PerspectiveType{
    p3D, p2D
}

public enum ViewType{
	STANDARD_3D, STANDARD_2D, MENU, PAUSED, LEAN_LEFT, LEAN_RIGHT, BACKWARD
}
