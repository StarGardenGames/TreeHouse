﻿using UnityEngine;
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

	// Object references for the Player and the Main Camera
	PlayerController playerController;

	// State variables
	public string currentState { get; private set; }
	public string previousState { get; private set; }
	public string targetState { get; private set; }
	public PerspectiveType currentPerspective { get; private set; }
	public bool paused {get; private set;}

	private const string STATE_GAMEPLAY_2D = "2D";
	private const string STATE_GAMEPLAY_3D = "3D";
	private const string STATE_MENU = "menu";
	private const string STATE_PAUSED = "pause";
	private const string STATE_TRANSITION = "transition";
	private const string STATE_LEAN_LEFT = "lean_left";
	private const string STATE_LEAN_RIGHT = "lean_right";
	private const string STATE_BACKWARD = "backward";

	// Flip failure timer variables
	private float failureTime = .3f;    // How long the transition lasts before the flip fails and switches back
	private float failureTimer = 0f;    // The current timer used in the FailTimer coroutine

	// TODO: Change camera mounts to be dynamic once menus and platforms are improved
	// Camera Mounts
	private Transform MOUNT_GAMEPLAY_2D;
	private Transform MOUNT_GAMEPLAY_3D;
	private Transform MOUNT_PAUSED;         // This will be dynamic once separate platforms can change this to their specified pause mount
	private Transform MOUNT_MENU;           // This will be dynamic once we know the final menu structure
	private Transform MOUNT_LEAN_LEFT;
	private Transform MOUNT_LEAN_RIGHT;
	private Transform MOUNT_BACKWARD;
	
	// Camera Settings
	private Matrix4x4 VIEW_SETTINGS_GAMEPLAY_3D =   CameraMatrixTypes.Standard3D;
	private Matrix4x4 VIEW_SETTINGS_GAMEPLAY_2D =   CameraMatrixTypes.Standard2D;
	private Matrix4x4 VIEW_SETTINGS_PAUSED =        CameraMatrixTypes.Standard3D;  
	private Matrix4x4 VIEW_SETTINGS_MENU =          CameraMatrixTypes.Menu; 
	private Matrix4x4 VIEW_SETTINGS_LEAN_LEFT =		CameraMatrixTypes.Standard3D; 
	private Matrix4x4 VIEW_SETTINGS_LEAN_RIGHT =		CameraMatrixTypes.Standard3D; 
	private Matrix4x4 VIEW_SETTINGS_BACKWARD =		CameraMatrixTypes.Standard3D; 
	
	// Events to notify listeners of state changes
	public event System.Action<bool> GamePausedEvent;
	public event System.Action<PerspectiveType> PerspectiveShiftEvent;

	#endregion Properties & Variables
	
	#region Monobehavior Implementation

	void Start () {
		playerController = PlayerController.instance;
		// Gather mount gameobjects
		GameObject mount2dObj = GameObject.Find("2DCameraMount");
		GameObject mount3dObj = GameObject.Find("3DCameraMount");
		GameObject mountPausedObj = GameObject.Find("PauseMount");
		GameObject mountMenuObj = GameObject.Find("MenuMount");
		
		//find transforms
		if(mount2dObj != null) 		MOUNT_GAMEPLAY_2D = mount2dObj.transform;
		if(mount3dObj != null) 		MOUNT_GAMEPLAY_3D = mount3dObj.transform;
		if(mountPausedObj != null) MOUNT_PAUSED = mountPausedObj.transform; 
		if(mountMenuObj != null)	MOUNT_MENU = mountMenuObj.transform;
		
		//initial settings
		currentPerspective = PerspectiveType.p2D;
		
		//determine wheather or not to start on menu
		//if(MOUNT_MENU == null)
			StartGame();
		/*else
			EnterMenu();*/

		// Register event handlers to InputManagers
		InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
		InputManager.instance.PausePressedEvent += HandlePausePressed;

		// Register to switch state to proper gameplay when shift is complete
		CameraController.instance.ShiftCompleteEvent += HandleShiftComplete;
	}

	#endregion Monobehavior Implementation

    #region State Change Functions

    // Change the satte and set the new mount and view settings
    private void EnterGameplay2D()
    {
        currentState = STATE_GAMEPLAY_2D;
        CameraController.instance.SetMount(MOUNT_GAMEPLAY_2D, VIEW_SETTINGS_GAMEPLAY_2D);
    }

    // Change the satte and set the new mount and view settings
    private void EnterGameplay3D()
    {
        currentState = STATE_GAMEPLAY_3D;
        CameraController.instance.SetMount(MOUNT_GAMEPLAY_3D, VIEW_SETTINGS_GAMEPLAY_3D);
    }

	// Enter transition state between 2D and 3D or vice-versa
	private void EnterTransition(string targetState)
	{
		// Pause the game during the transition
		RaisePauseEvent(true);

		/*if(targetState == STATE_GAMEPLAY_2D)
			EnterGameplay2D();
		
		if(targetState == STATE_GAMEPLAY_3D)
			EnterGameplay3D();*/


		// Set transition state and target state
		currentState = STATE_TRANSITION;
		this.targetState = targetState;

		// Set the camer'as mount to the target settings
		if (targetState == STATE_GAMEPLAY_2D)
			CameraController.instance.SetMount(MOUNT_GAMEPLAY_2D, VIEW_SETTINGS_GAMEPLAY_2D);
		else
			CameraController.instance.SetMount(MOUNT_GAMEPLAY_3D, VIEW_SETTINGS_GAMEPLAY_3D);

	}

    // Pause the game
    private void EnterPause()
    {
        previousState = currentState;
        currentState = STATE_PAUSED;
        CameraController.instance.SetMount(MOUNT_PAUSED, VIEW_SETTINGS_PAUSED);
        RaisePauseEvent(true);
    }

    // Exits the pause state
    private void ExitPause()
    {
        // Return to previous state
        currentState = previousState;
        if (currentState == STATE_GAMEPLAY_2D)
            EnterGameplay2D();
        else
            EnterGameplay3D();

        // Alert gameobjects of unpause
        RaisePauseEvent(false);
    }

    private void EnterMenu()
    {
        previousState = currentState;
        currentState = STATE_MENU;
        CameraController.instance.SetMount(MOUNT_MENU, VIEW_SETTINGS_MENU);
        RaisePauseEvent(true);
    }

    private void ExitMenu()
    {
        currentState = previousState;
        if (currentState == STATE_GAMEPLAY_2D)
            EnterGameplay2D();
        else
            EnterGameplay3D();

        // Unpause the game
        RaisePauseEvent(false);

    }

    #endregion State Change Functions

    #region Event Handlers

    // Determine if the pause event should be aised. Do nothing if in a menu 
    private void HandlePausePressed()
    {   
        if (currentState == STATE_GAMEPLAY_2D || currentState == STATE_GAMEPLAY_3D)
        {
            // If the current state is a gameplay state pause the game
            EnterMenu();
        }
        else if (currentState == STATE_MENU)
        {
            // If the current state is paused then unpause
            ExitMenu();
        }
    }

    // Determine if the perspective shift event should be raised
    private void HandleShiftPressed()
    {
        // Shift perspective if current state is a gameplay state
        if (currentState == STATE_GAMEPLAY_2D || currentState == STATE_GAMEPLAY_3D)
        {
            // Find the target state to switch to
            string newPerspective = (currentState == STATE_GAMEPLAY_2D) ? STATE_GAMEPLAY_3D : STATE_GAMEPLAY_2D;

            // Find the corresponding perspective to store for external reference
            currentPerspective = (newPerspective == STATE_GAMEPLAY_2D) ? PerspectiveType.p2D : PerspectiveType.p3D;

            // Begin transition to that state (since this involves the shift animation we use the transition state)
            EnterTransition(newPerspective);

            if (playerController.Check2DIntersect())
                StartCoroutine(FailTimer());
			else
				playerController.Flip();
        }
    }

	// Determine if the menu state event should be raised
	private void HandleMenuEnterPressed(){
		if (currentState == STATE_GAMEPLAY_2D || currentState == STATE_GAMEPLAY_3D){
			EnterMenu();
		}
		if (currentState == STATE_MENU){
			ExitMenu();
		}
	}
    
	// Handle the event raised when the camera completes its shift. 
	private void HandleShiftComplete(){
		if (currentState != STATE_MENU && currentState != STATE_PAUSED){
			// Alert listeners to change in perspective
			RaisePerspectiveShiftEvent();

			// Transition to target state
			if (targetState == STATE_GAMEPLAY_2D)
				EnterGameplay2D();
			else
				EnterGameplay3D();

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
    private void RaisePerspectiveShiftEvent()
    {
        if (PerspectiveShiftEvent != null)
            PerspectiveShiftEvent(currentPerspective);
    }

    #endregion Event Raising Functions

    #region Public Interface

    // Called by main menu to begin gameplay
    public void StartGame(){
        EnterTransition(STATE_GAMEPLAY_2D);
        currentPerspective = PerspectiveType.p2D;
		  RaisePauseEvent(false);
    }
    #endregion Public Interface

    #region Helper Functions

    private IEnumerator FailTimer()
    {
        failureTimer = 0f;

        while (failureTimer < failureTime)
        {
            failureTimer += Time.deltaTime;

            yield return null;
        }

        // Find the corresponding perspective to store for external reference
        currentPerspective = (targetState == STATE_GAMEPLAY_2D) ? PerspectiveType.p3D : PerspectiveType.p2D;

        // Find the corresponding perspective to store for external reference
        if (targetState == STATE_GAMEPLAY_2D)
            EnterTransition(STATE_GAMEPLAY_3D);
        else
            EnterTransition(STATE_GAMEPLAY_2D);

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
