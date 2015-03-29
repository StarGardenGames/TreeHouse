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

    // Object references for the Player and the Main Camera
    PlayerController3 playerController;

    // State variables
    public string currentState { get; private set; }
    public string previousState { get; private set; }
    public string targetState { get; private set; }
    public PerspectiveType currentPerspective { get; private set; }
    private const string STATE_GAMEPLAY_2D = "2D";
    private const string STATE_GAMEPLAY_3D = "3D";
    private const string STATE_MENU = "menu";
    private const string STATE_PAUSED = "pause";
    private const string STATE_TRANSITION = "transition";

    // TODO: Change camera mounts to be dynamic once menus and platforms are improved
    // Camera Mounts
    private Transform MOUNT_GAMEPLAY_2D;
    private Transform MOUNT_GAMEPLAY_3D;
    private Transform MOUNT_PAUSED;         // This will be dynamic once separate platforms can change this to their specified pause mount
    private Transform MOUNT_MENU;           // This will be dynamic once we know the final menu structure

    // Camera Settings
    private Matrix4x4 VIEW_SETTINGS_GAMEPLAY_3D =   CameraMatrixTypes.Standard3D;
    private Matrix4x4 VIEW_SETTINGS_GAMEPLAY_2D =   CameraMatrixTypes.Standard2D;
    private Matrix4x4 VIEW_SETTINGS_PAUSED =        CameraMatrixTypes.Standard3D;   // Consider adding specific settings for pause
    private Matrix4x4 VIEW_SETTINGS_MENU =          CameraMatrixTypes.Standard3D;   // Consider adding specific settings for menus

    // Events to notify listeners of state changes
    public event System.Action<bool> GamePausedEvent;
    public event System.Action<PerspectiveType> PerspectiveShiftEvent;

    #endregion Properties & Variables


    #region Monobehavior Implementation

    void Start () {
	    
        // Find Mounts in scene
        MOUNT_GAMEPLAY_2D = GameObject.Find("2DCameraMount").transform;
        MOUNT_GAMEPLAY_3D = GameObject.Find("3DCameraMount").transform;
        //MOUNT_PAUSED = GameObject.Find("PauseMount").transform;             // Consider switching this to be more dynamic in future
        //MOUNT_MENU = GameObject.Find("MenuMount").transform;                // Consider switching this to be more dynamic in future

        // TODO: Change this line of code to use the final player object name and script name
        // Find Player and Main Camera
        playerController = GameObject.Find("NewPlayer").GetComponent<PlayerController3>();

        // TODO: Change the game's starting state to dynamic behavior at some point
        // Start the game in 2D state
        currentState = STATE_GAMEPLAY_2D;
		currentPerspective = PerspectiveType.p2D;
        CameraController2.instance.SetMount(MOUNT_GAMEPLAY_2D, VIEW_SETTINGS_GAMEPLAY_2D);

        // Register event handlers to InputManagers
        InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
        InputManager.instance.PausePressedEvent += HandlePausePressed;
	}

    #endregion Monobehavior Implementation


    #region State Change Functions

    // Change the satte and set the new mount and view settings
    private void EnterGameplay2D()
    {
        currentState = STATE_GAMEPLAY_2D;
        CameraController2.instance.SetMount(MOUNT_GAMEPLAY_2D, VIEW_SETTINGS_GAMEPLAY_2D);
    }

    // Change the satte and set the new mount and view settings
    private void EnterGameplay3D()
    {
        currentState = STATE_GAMEPLAY_3D;
        CameraController2.instance.SetMount(MOUNT_GAMEPLAY_3D, VIEW_SETTINGS_GAMEPLAY_3D);
    }

	//called by Main Menu when play button is pressed
	public void GameStart(){

	}

    // Enter transition state between 2D and 3D or vice-versa
    private void EnterTransition(string targetState)
    {
        // Pause the game during the transition
        RaisePauseEvent(true);

        // Set transition state and target state
        currentState = STATE_TRANSITION;
        this.targetState = targetState;

        // Set the camer'as mount to the target settings
        if (targetState == STATE_GAMEPLAY_2D)
            CameraController2.instance.SetMount(MOUNT_GAMEPLAY_2D, VIEW_SETTINGS_GAMEPLAY_2D);
        else
            CameraController2.instance.SetMount(MOUNT_GAMEPLAY_3D, VIEW_SETTINGS_GAMEPLAY_3D);

        // Register to switch state to proper gameplay when shift is complete
        CameraController2.instance.ShiftCompleteEvent += HandleShiftComplete;
    }

    // Pause the game
    private void EnterPause()
    {
        previousState = currentState;
        currentState = STATE_PAUSED;
        CameraController2.instance.SetMount(MOUNT_PAUSED, VIEW_SETTINGS_PAUSED);
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
        CameraController2.instance.SetMount(MOUNT_MENU, VIEW_SETTINGS_MENU);
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
            EnterPause();
        }
        else if (currentState == STATE_PAUSED)
        {
            // If the current state is paused then unpause
            ExitPause();
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
        }
    }

    // Determine if the menu state event should be raised
    private void HandleMenuEnterPressed()
    {
        if (currentState == STATE_GAMEPLAY_2D || currentState == STATE_GAMEPLAY_3D)
        {
            EnterMenu();
        }
        if (currentState == STATE_MENU)
        {
            ExitMenu();
        }
    }
    
    // Handle the event raised when the camera completes its shift. 
    private void HandleShiftComplete()
    {
        // Alert listeners to change in perspective
        RaisePerspectiveShiftEvent();
        
        // Transition to target state
        if (targetState == STATE_GAMEPLAY_2D)
            EnterGameplay2D();
        else
            EnterGameplay3D();

        // Unpause Game
        RaisePauseEvent(false);

        // Unregister this function
        CameraController2.instance.ShiftCompleteEvent -= HandleShiftComplete;
    }

    #endregion Event Handlers


    #region Event Raising Functions

    // Alert listeners that the game is being paused or unpaused
    private void RaisePauseEvent(bool paused)
    {
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

    public void StartGame()
    {

    }
    #endregion Public Interface

}

/// <summary>
///     Enumeration to dictate which perspective the current state is in. 
///     Objects can reference this and it is passed in shioft events so they know which behavior mode to execute.
/// </summary>
public enum PerspectiveType
{
    p3D, p2D
}
