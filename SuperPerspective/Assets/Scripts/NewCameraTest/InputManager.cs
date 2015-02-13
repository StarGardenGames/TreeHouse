using UnityEngine;
using System.Collections;

using SuperPerspective.Singleton;

/// <summary>
///     Sends notifications on user input and allows polling for current input state of buttons and axes.
///     Also notifies listeners when the game's perspective changes.
/// </summary>
public class InputManager : Singleton<InputManager>
{
    #region Properties & Variables

    // Button press events
    public event System.Action JumpPressed;         // Jump
    public event System.Action InteractPressed;     // Interaction
    public event System.Action GrabPressed;         // Grab

    // Perspective change event
    public event System.Action<PerspectiveType> perspectiveShiftEvent;
    private PerspectiveType currentPerspective;
    
    // Game pause event
    public event System.Action<bool> GamePaused;

    // Game's pause state
    private bool _paused = false;
    public bool paused
    {
        get { return _paused; }
    }

    #endregion Properties & Variables


    #region Monobehavior Implementation

    // Use this for initialization
	void Start () {
	    currentPerspective = PerspectiveType.p3D;
	}
	
	// listens to player input and raises events for listeners.
	void Update () {

        // Check pause button
        if (Input.GetButtonDown("Pause"))
            RaiseGamePauseEvent();

        // Check jump button
        if (Input.GetButtonDown("Jump"))
            RaiseJumpPressedEvent();

        // Check interaction button
        if (Input.GetButtonDown("Interaction"))
            RaiseInteractPressedEvent();

	    // Check grab button
        if (Input.GetButtonDown("Grab"))
            RaiseGrabPressedEvent();

        // Check perspective shift
        if (Input.GetButtonDown("PerspectiveShift"))
            RaisePerspectiveShiftEvent();

    }

    #endregion MonobehaviorImplementation


    #region Public Interface

    // Returns the player's movement on the horizontal axis in 2D and the vertical axis in 3D 
    public float GetForwardMovement()
    {
        if (currentPerspective == PerspectiveType.p3D)
        {
            return Input.GetAxis("Vertical");
        }
        else
        {
            return Input.GetAxis("Horizontal");
        }
    }

    // Returns the player's movement on the horizontal axis in 3D and zero in 2D
    public float GetSideMovement()
    {
        if (currentPerspective == PerspectiveType.p3D)
            return Input.GetAxis("Horizontal");
        else
            return 0f;
    }

    // Returns true if the jump button is currently pressed
    public bool JumpStatus()
    {
        return Input.GetButton("Jump");
    }

    // Returns true if the interaction button is currently pressed
    public bool InteractStatus()
    {
        return Input.GetButton("Interaction");
    }

    // Returns true if the grab button is currently pressed
    public bool GrabStatus()
    {
        return Input.GetButton("Grab");
    }

    #endregion Public Interface


    #region Event Raising Functions

    // Called when the player shifts perspective
    private void RaisePerspectiveShiftEvent()
    {
        // Alert listeners of the new perspective
        if (perspectiveShiftEvent != null)
        {
            if (currentPerspective == PerspectiveType.p3D)
            {
                // Change to 2D
                currentPerspective = PerspectiveType.p2D;
                perspectiveShiftEvent(currentPerspective);

            }
            else if (currentPerspective == PerspectiveType.p2D)
            {
                // Change to 3D
                currentPerspective = PerspectiveType.p3D;
                perspectiveShiftEvent(currentPerspective);
            }
        }
    }

    // Called when the player pauses the game
    private void RaiseGamePauseEvent()
    {
        // Change the pause state
        _paused = !_paused;

        // Alert listeners of the new pause state
        if (GamePaused != null)
            GamePaused(_paused);
    }

    // Called when the player presses the jump button
    private void RaiseJumpPressedEvent()
    {
        if (JumpPressed != null)
            JumpPressed();
    }

    // Called when the player presses the interaction button
    private void RaiseInteractPressedEvent()
    {
        if (InteractPressed != null)
            InteractPressed();
    }

    // Called when the player presses the grab button
    private void RaiseGrabPressedEvent()
    {
        if (GrabPressed != null)
            GrabPressed();
    }



    #endregion Event Raising Functions
}
