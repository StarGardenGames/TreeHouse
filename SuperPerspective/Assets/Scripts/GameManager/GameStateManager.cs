using UnityEngine;
using System.Collections;

/// <summary>
///     This class is in charge of controlling the game state transitions. 
///     It receives input from InputManager, and tells the camera, player, and any other necessary objects of behavior changes.
///     This keeps all transitions logic in one location and allows the player, camera, and menu scripts to have simple behavior and remain modular.
/// </summary>
public class GameStateManager : MonoBehaviour
{
	public static GameStateManager instance;
	
	//suppress warnings
	#pragma warning disable 414, 649, 472
	
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
	private bool[] view_pause = new bool[NUM_VIEW_TYPES];
	
	// Events to notify listeners of state changes
	public event System.Action<bool> GamePausedEvent;
	public event System.Action<PerspectiveType> PerspectiveShiftEvent;//called at end  of shift
	//either PerspectiveShiftSuccessEvent or PerspectiveShiftFailEvent will be call at the beginning of shifts
	public event System.Action PerspectiveShiftSuccessEvent;
	public event System.Action PerspectiveShiftFailEvent;
	
	#endregion Properties & Variables
	
	#region Monobehavior Implementation

	public void Awake () {
		//singleton
		if (instance == null)
			instance = this;
		else
			Destroy (this);		
	}
	
	void Start(){
		//init variables
		InitViewPerspectives();
		InitViewMounts();
		InitViewPauseStates();
		
		//determine wheather or not to start on menu
		if(view_mounts[(int)ViewType.MENU] == null){
			StartGame();
		}else{
			EnterState(ViewType.MENU);
		}
		
		// Register event handlers to InputManagers
		InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
		InputManager.instance.PausePressedEvent += HandlePausePressed;
		InputManager.instance.LeanLeftPressedEvent += HandleLeanLeftPressed;
		InputManager.instance.LeanRightPressedEvent += HandleLeanRightPressed;
		InputManager.instance.LeanLeftReleasedEvent += HandleLeanLeftReleased;
		InputManager.instance.LeanRightReleasedEvent += HandleLeanRightReleased;
		InputManager.instance.BackwardMovementEvent += HandleBackwardMovement;
		InputManager.instance.ForwardMovementEvent += HandleForwardMovement;

		// Register to switch state to proper gameplay when shift is complete
		CameraController.instance.TransitionCompleteEvent += HandleTransitionComplete;
		
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
		mounts[(int)ViewType.STANDARD_3D] = GameObject.Find("3DCameraMount");
		mounts[(int)ViewType.STANDARD_2D] = GameObject.Find("2DCameraMount");
		mounts[(int)ViewType.MENU] = GameObject.Find("MenuMount");
		mounts[(int)ViewType.LEAN_LEFT] = GameObject.Find("LeanLeftCameraMount");
		mounts[(int)ViewType.LEAN_RIGHT] = GameObject.Find("LeanRightCameraMount");
		mounts[(int)ViewType.BACKWARD] = GameObject.Find("BackwardCameraMount");
		
		//find transforms
		for(int i = 0; i < mounts.Length; i++){
			if(mounts[i] != null)
				view_mounts[i] = mounts[i].transform;
		}
	}

	void InitViewPauseStates(){
		view_pause[(int)ViewType.STANDARD_3D] =   false;
		view_pause[(int)ViewType.STANDARD_2D] =   false;
		view_pause[(int)ViewType.PAUSED] =        true; 
		view_pause[(int)ViewType.MENU] =          true; 
		view_pause[(int)ViewType.LEAN_LEFT] =		 false; 
		view_pause[(int)ViewType.LEAN_RIGHT] =	 false; 
		view_pause[(int)ViewType.BACKWARD] =		 false;
	}
	
	#endregion Monobehavior Implementation
	
	#region Event Handlers

	//TODO review
	private void HandlePausePressed(){   
		switch(currentState){
			case ViewType.MENU:
				if(previousState != null)
					EnterState(previousState);
				break;
			case ViewType.PAUSED:
				EnterState(previousState);
				break;
			default:
				EnterState(ViewType.PAUSED);
				break;
		}
	}

	private void HandleShiftPressed(){
		if (!IsPauseState(currentState) && !PlayerController.instance.GrabbedCrate()){
			ViewType newState = (view_perspectives[(int)currentState] == PerspectiveType.p3D) ?
				ViewType.STANDARD_2D : ViewType.STANDARD_3D;

			EnterState(newState);

			if(PlayerController.instance.Check2DIntersect()){
				RaisePerspectiveShiftFailEvent();
				StartCoroutine(FailTimer());
			}else{
				RaisePerspectiveShiftSuccessEvent();
			}
		}
	}

	// TODO Review (handle pause and handle menu are pretty much the same)
	private void HandleMenuEnterPressed(){
		if (currentState == ViewType.MENU){
			EnterState(previousState);
		}else{
			EnterState(ViewType.MENU);
		}
	}
    
	private void HandleTransitionComplete(){
		currentState = targetState;

		RaisePerspectiveShiftEvent();
	
		RaisePauseEvent(view_pause[(int)currentState]);
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
  
	private void HandleBackwardMovement(){
		if(InputManager.instance.GetForwardMovement() == -1 && currentState == ViewType.STANDARD_3D){
			EnterState(ViewType.BACKWARD);
		}
	}
	
	private void HandleForwardMovement(){
		if(InputManager.instance.GetForwardMovement() == 1 && currentState == ViewType.BACKWARD){
			EnterState(ViewType.STANDARD_3D);
		}	
	}
	
	#endregion Event Handlers
	
	#region State Change Functions
	
	private void EnterState(ViewType targetState){
		RaisePauseEvent(IsPauseState(targetState));
		previousState = currentState;
		this.targetState = targetState;
		currentPerspective = view_perspectives[(int)targetState];
		if(targetState == ViewType.PAUSED){
			
			
		}
		CameraController.instance.SetMount(view_mounts[(int)targetState],currentPerspective);
	}
	
	private void UpdatePauseMount(){
		Transform newMount = IslandControl.instance.findCurrentPauseMount();
		if(newMount == null){
			view_mounts[(int)ViewType.PAUSED] = view_mounts[(int)ViewType.STANDARD_3D];
		}else{
			view_mounts[(int)ViewType.PAUSED] = newMount;
		}
		
	}
	
   #endregion State Change Functions

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
		EnterState(ViewType.STANDARD_2D);
	}

	public void Reset(){
		InitViewPerspectives();
		InitViewMounts();
		InitViewPauseStates();
		
		//determine wheather or not to start on menu
		if(view_mounts[(int)ViewType.MENU] == null){
			StartGame();
		}else{
			EnterState(ViewType.MENU);
		}

		// Register event handlers to InputManagers
		InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
		InputManager.instance.PausePressedEvent += HandlePausePressed;
		InputManager.instance.LeanLeftPressedEvent += HandleLeanLeftPressed;
		InputManager.instance.LeanRightPressedEvent += HandleLeanRightPressed;
		InputManager.instance.LeanLeftReleasedEvent += HandleLeanLeftReleased;
		InputManager.instance.LeanRightReleasedEvent += HandleLeanRightReleased;
		InputManager.instance.BackwardMovementEvent += HandleBackwardMovement;
		InputManager.instance.ForwardMovementEvent += HandleForwardMovement;
		
		// Register to switch state to proper gameplay when shift is complete
		CameraController.instance.TransitionCompleteEvent += HandleTransitionComplete;
	}
	
	public void UpdatePauseMount(Transform newMount){
		view_mounts[(int)ViewType.PAUSED] = newMount;
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
		currentPerspective = PerspectiveType.p3D;

		// Find the corresponding perspective to store for external reference
		EnterState(ViewType.STANDARD_3D);
	}

	private bool IsPauseState(ViewType targetState){
		return view_pause[(int)targetState] || currentPerspective != view_perspectives[(int)targetState];
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
