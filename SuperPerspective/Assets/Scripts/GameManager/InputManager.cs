using UnityEngine;
using System.Collections;

/// <summary>
///     Sends notifications on user input and allows polling for current input state of buttons and axes.
///     Also notifies listeners when the game's perspective changes.
/// </summary>
public class InputManager : MonoBehaviour{
	
	public static InputManager instance;
	
	//suppress warnings
	#pragma warning disable 414

	#region Properties & Variables

	// Button press events
	public event System.Action JumpPressedEvent;         // Jump
	public event System.Action InteractPressedEvent;     // Interaction
	public event System.Action GrabPressedEvent;         // Grab
	public event System.Action ShiftPressedEvent;
	public event System.Action PausePressedEvent;
	public event System.Action LeanLeftPressedEvent;
	public event System.Action LeanRightPressedEvent;
	public event System.Action LeanLeftReleasedEvent;
	public event System.Action LeanRightReleasedEvent;
	public event System.Action BackwardMovementEvent;
	public event System.Action ForwardMovementEvent;
	public event System.Action DevConsoleEvent;
	
	// Game's pause state
	private bool continuePressed = false;//used as an alternate way to unpause

	// Perspective shift properties
	// TODO: Move this funcionality to the camera script
	private const float FAIL_TIME = 0.5f;
	private float flipTimer = 0;
	private bool flipFailed = false;
	
	private float previousForwardMovement = 0;

	#endregion Properties & Variables

	#region Monobehavior Implementation

	void Awake(){
		//singleton
		if (instance == null)
			instance = this;
		else
			Destroy (this);
	}

	// listens to player input and raises events for listeners.
	void Update () {
		if(Input.GetButtonDown("Pause") || continuePressed){
			RaiseEvent(PausePressedEvent);
			continuePressed = false;
		}
		if(Input.GetButtonDown("Jump")) 					
			RaiseEvent(JumpPressedEvent);
		
		if(Input.GetButtonDown("Interaction"))			
			RaiseEvent(InteractPressedEvent);
		
		if(Input.GetButtonDown("Grab"))						
			RaiseEvent(GrabPressedEvent);
		
		if(Input.GetButtonDown("PerspectiveShift"))		
			RaiseEvent(ShiftPressedEvent);
		
		if(Input.GetButtonDown("LeanLeft"))					
			RaiseEvent(LeanLeftPressedEvent);
		
		if(Input.GetButtonDown("LeanRight"))				
			RaiseEvent(LeanRightPressedEvent);
		
		if(Input.GetButtonUp("LeanLeft"))
			RaiseEvent(LeanLeftReleasedEvent);
		
		if(Input.GetButtonUp("LeanRight"))
			RaiseEvent(LeanRightReleasedEvent);
		
		if(previousForwardMovement != -1 && GetForwardMovement() == -1)
			RaiseEvent(BackwardMovementEvent);
		
		if(previousForwardMovement != 1 && GetForwardMovement() == 1)
			RaiseEvent(ForwardMovementEvent);

		if(Input.GetButtonUp("DevConsoleToggle"))
			RaiseEvent(DevConsoleEvent);
		
		previousForwardMovement = GetForwardMovement();
	}

	#endregion MonobehaviorImplementation


	#region Public Interface

	// Returns the player's movement on the horizontal axis in 2D and the vertical axis in 3D 
	public float GetForwardMovement(){
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Input.GetAxis("Vertical");
		else
			return Input.GetAxis("Horizontal");
	}

	// Returns the player's movement on the horizontal axis in 3D and zero in 2D
	public float GetSideMovement(){
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Input.GetAxis("Horizontal");
		else
			return 0f;
	}

	// Returns true if the jump button is currently pressed
	public bool JumpStatus(){
		return Input.GetButton("Jump");
	}

	// Returns true if the interaction button is currently pressed
	public bool InteractStatus(){
		return Input.GetButton("Interaction");
	}

	// Returns true if the grab button is currently pressed
	// NOTE: refers to crate grabbing
	public bool GrabStatus(){
		return Input.GetButton("Grab");
	}

	public void SetFailFlag(){
		flipFailed = true;
	}

	#endregion Public Interface

	#region Event Raising Functions
	
	private void RaiseEvent(System.Action gameEvent){
		if(gameEvent != null)
			gameEvent();
	}
	
	public void ContinuePressed(){
		continuePressed = true;
	}
	

	#endregion Event Raising Functions
}
