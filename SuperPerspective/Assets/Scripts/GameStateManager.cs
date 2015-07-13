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
	
	public ViewType testState;
	
	public void Update(){
		testState = currentState;
	}
	
	public ViewType currentState { get; private set; }
	public ViewType previousState { get; private set; }
	public ViewType targetState { get; private set; }
	public PerspectiveType currentPerspective { get; private set; }
	public bool paused {get; private set;}

	// Flip failure timer variables
	private float failureTime = .3f;    // How long the transition lasts before the flip fails and switches back
	private float failureTimer = 0f;    // The current timer used in the FailTimer coroutine

	// view mounts & settings
	private const int NUM_VIEW_TYPES = 8;
	private Transform[] view_mounts = new Transform[NUM_VIEW_TYPES];
	private Matrix4x4[] view_settings = new Matrix4x4[NUM_VIEW_TYPES];
	
	// Events to notify listeners of state changes
	public event System.Action<bool> GamePausedEvent;
	public event System.Action<PerspectiveType> PerspectiveShiftEvent;//called at end  of shift
	//either PerspectiveShiftSuccessEvent or PerspectiveShiftFailEvent will be call at the beginning of shifts
	public event System.Action PerspectiveShiftSuccessEvent;
	public event System.Action PerspectiveShiftFailEvent;

	
	
	#endregion Properties & Variables
	
	#region Monobehavior Implementation

	void Start () {
		InitViewSettings();
		InitViewMounts();
		
		//initial settings
		
		
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

		// Register to switch state to proper gameplay when shift is complete
		CameraController.instance.ShiftCompleteEvent += HandleShiftComplete;
	}
	
	void InitViewSettings(){
		view_settings[(int)ViewType.STANDARD_3D] =   CameraMatrixTypes.Standard3D;
		view_settings[(int)ViewType.STANDARD_2D] =   CameraMatrixTypes.Standard2D;
		view_settings[(int)ViewType.PAUSED] =        CameraMatrixTypes.Standard3D;  
		view_settings[(int)ViewType.MENU] =          CameraMatrixTypes.Standard2D; 
		view_settings[(int)ViewType.LEAN_LEFT] =		CameraMatrixTypes.Standard3D; 
		view_settings[(int)ViewType.LEAN_RIGHT] =		CameraMatrixTypes.Standard3D; 
		view_settings[(int)ViewType.BACKWARD] =		CameraMatrixTypes.Standard3D;
	}
	
	void InitViewMounts(){
		// Gather mount gameobjects
		GameObject mount2dObj = GameObject.Find("2DCameraMount");
		GameObject mount3dObj = GameObject.Find("3DCameraMount");
		GameObject mountPausedObj = GameObject.Find("PauseMount");
		GameObject mountMenuObj = GameObject.Find("MenuMount");
		
		//find transforms
		if(mount2dObj != null) 		view_mounts[(int)ViewType.STANDARD_2D] = mount2dObj.transform;
		if(mount3dObj != null) 		view_mounts[(int)ViewType.STANDARD_3D] = mount3dObj.transform;
		if(mountPausedObj != null) view_mounts[(int)ViewType.PAUSED] = mountPausedObj.transform; 
		if(mountMenuObj != null)	view_mounts[(int)ViewType.MENU] = mountMenuObj.transform;
	}

	#endregion Monobehavior Implementation

	#region State Change Functions
	
	private void EnterState(ViewType targetState){
		previousState = currentState;
		currentState = targetState;
		CameraController.instance.SetMount(view_mounts[(int)targetState],view_settings[(int)targetState]);
	}

	// Enter transition state between 2D and 3D or vice-versa
	private void EnterTransition(ViewType targetState){
		// Pause the game during the transition
		RaisePauseEvent(true);
		
		this.targetState = targetState;
		
		EnterState(targetState);
	}

	// Pause the game
	private void EnterPause(){
		EnterState(ViewType.PAUSED);
		RaisePauseEvent(true);
	}

    // Exits the pause state
    private void ExitPause()
    {
        // Return to previous state
		  EnterState(previousState);
        /*if (currentState == ViewType.STANDARD_2D)
            EnterGameplay2D();
        else
            EnterGameplay3D();*/

        // Alert gameobjects of unpause
        RaisePauseEvent(false);
    }

    private void EnterMenu(){
		  EnterState(ViewType.MENU);
        RaisePauseEvent(true);
    }

    private void ExitMenu()
    {
			EnterState(previousState);

        // Unpause the game
        RaisePauseEvent(false);

    }
	 
	

    #endregion State Change Functions

    #region Event Handlers

    // Determine if the pause event should be aised. Do nothing if in a menu 
    private void HandlePausePressed()
    {   
        if (currentState == ViewType.STANDARD_2D || currentState == ViewType.STANDARD_3D)
        {
            // If the current state is a gameplay state pause the game
            EnterMenu();
        }
        else if (currentState == ViewType.MENU)
        {
            // If the current state is paused then unpause
            ExitMenu();
        }
    }

    // Determine if the perspective shift event should be raised
    private void HandleShiftPressed()
    {
        // Shift perspective if current state is a gameplay state
        if (currentState == ViewType.STANDARD_2D || currentState == ViewType.STANDARD_3D)
        {
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

	// Determine if the menu state event should be raised
	private void HandleMenuEnterPressed(){
		if (currentState == ViewType.STANDARD_2D || currentState == ViewType.STANDARD_3D){
			EnterMenu();
		}
		if (currentState == ViewType.MENU){
			ExitMenu();
		}
	}
    
	// Handle the event raised when the camera completes its shift. 
	private void HandleShiftComplete(){
		if (!IsPauseState(currentState)){
			// Alert listeners to change in perspective
			RaisePerspectiveShiftEvent();

			// Transition to target state
			EnterState(targetState);

			// Unpause Game
			RaisePauseEvent(false);
		}
		// Unregister this function (we can probably remove this)
		// CameraController.instance.ShiftCompleteEvent -= HandleShiftComplete;
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
public enum PerspectiveType
{
    p3D, p2D
}

public enum ViewType{
	STANDARD_3D, STANDARD_2D, MENU, PAUSED, TRANSITION, LEAN_LEFT, LEAN_RIGHT, BACKWARD
}
