using UnityEngine;
using System.Collections;

public class CameraMount2D : MonoBehaviour
{

    #region Properties & Variables

    // Leaning Parameters
    public bool active = false;             // Determines whether this mount is active and should listen for lean inpuut
    public float horizontalLimits = 1f;     // How far to the left or right the camera can lean
    public float verticalLimits = .75f;     // How far Up or down the camera can lean

    #endregion Properties & Variables


    #region MonoBehavior Implementation

    // Use this for initialization
	void Start () 
    {
        // Register to become active when switching to 2D
        GameStateManager.instance.PerspectiveShiftEvent += SetActive;
	}
	
	// Update is called once per frame
	void Update () 
    {
        // TODO: Replace these checks with calls to InputManager
    }

    #endregion MonoBehavior Implementation


    #region Event Handlers

    // Set this mount as the active mount if we shift into 2D
    private void SetActive(PerspectiveType p)
    {
        active = (p == PerspectiveType.p2D);
    }

    #endregion Event Handlers
}
