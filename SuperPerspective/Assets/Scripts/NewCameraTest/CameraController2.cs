using UnityEngine;
using System.Collections;

using SuperPerspective.Singleton;

/// <summary>
///     TODO: Summarize this prolly?
/// </summary>
[RequireComponent(typeof(MatrixBlender))]   // Required to blend the camera's view settings between states
[RequireComponent(typeof(Camera))]          // The camera needs to actually be, you know, a camera
public class CameraController2 : PersistentSingleton<CameraController2>
{
    #region Properties & Variables

    // This is the camera component and matrix blender script (see above)
    Camera cam;
    MatrixBlender blender;

    // Movement variables
    Transform mount;                // The current transform the camera is "mounted" to
    Vector3 velocity;               // Used by SmoothDamp function
    Quaternion startRotation;       // Used to Slerp rotation to new mount
    Matrix4x4 targetMatrix;         // The matrix containing camera settings to blend to

    // TODO: Consider taking these last three as extra parameters in SetMount to dictate blend speeds on the fly
    float smoothTime = .2f;         // Used to control the damp speed   
    float turnSpeed = 3f;           // USed to dictate how quickly the camera can match its target rotation
    float cameraBlendSpeed = .07f;  // USed to determine how quickly the camera changes from one setting to another and vice versa
    float shiftThreshold = .5f;     // Use to determine if the camera is close enough to the mount's position and rotation to consider the shift complete

    // Event to alert Gameplay State Manager of completed shift
    public event System.Action ShiftCompleteEvent;
    private bool shiftComplete = false;

    #endregion Properties & Variables


    #region Monobehavior Implementation

    void Awake()
    {
        // Get the MatrixBlender script
        blender = gameObject.GetComponent<MatrixBlender>();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Since the behavior in each state is the same we execute behavior in Update and just check conditions to change state
    void Update()
    {
        if (mount != null && targetMatrix != null)
        {
            // Smoothdamp the camera towards the mount and blend the camera matrix to the target settings
            transform.position = Vector3.SmoothDamp(transform.position, mount.position, ref velocity, smoothTime);

            // If we haven't matched the 2D mount's rotation yet rotate to match
            if (!(transform.rotation == mount.rotation))
                transform.rotation = Quaternion.RotateTowards(transform.rotation, mount.rotation, turnSpeed);

            // Check if the shift is complete
            if (!shiftComplete)
            {
                // IF the shift is over alert listeners
                if (CheckTransition())
                {
                    shiftComplete = true;
                    RaiseShiftCompleteEvent();
                }
            }
        }
    }

    #endregion Monobehavior Implementation


    #region Event Raising

    // Alert listeners (Gameplay state manager) that the perspective shift is complete
    private void RaiseShiftCompleteEvent()
    {
        if (ShiftCompleteEvent != null)
            ShiftCompleteEvent();
    }

    #endregion Event Raising


    #region Public Interface

    public void SetMount(Transform newMount, Matrix4x4 newViewSettings)
    {
        mount = newMount;
        targetMatrix = newViewSettings;
        shiftComplete = false;
        if (blender != null)
            blender.BlendToMatrix(targetMatrix, cameraBlendSpeed);
    }

    #endregion Public Interface


    #region Helper Functions

    private bool CheckTransition()
    {
        Vector3 positionDif = transform.position - mount.position;
        if (positionDif.magnitude > shiftThreshold)
            return false;

        float rotationDif = Quaternion.Angle(transform.rotation, mount.rotation);
        if (rotationDif > shiftThreshold)
            return false;
        
        return true;
    }

    #endregion Helper Functions
}

// TODO: Consider moving the CameraMatrixTypes helper class to the GameplayStateManager.cs file for organization purposes.
/// <summary>
///     Helper class to get frequently used view settings matrices
///     Referenced from gameplay state manager
/// </summary>
public static class CameraMatrixTypes
{
    // Camera setting variables
    private static float fov = 60f;                 // The camera's field of view while in 3D mode
    private static float near = .01f;                // The near clipping plane of the camera
    private static float far = 1000f;               // The far clipping plane of the camera
    private static float orthographicSize = 10f;    // The orthographic size variable of the camera in 2D mode

    // Used to quickly compute the aspect ratio
    private static float aspect 
    {
        get { return (float)Screen.width / (float)Screen.height; }
    }

    // Returns the camera settings used in 2D mode
    public static Matrix4x4 Standard2D
    {
        get { return Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far); }  
    }

    // Returns the camera settings used in 3D mode
    public static Matrix4x4 Standard3D
    {
        get { return Matrix4x4.Perspective(fov, aspect, near, far); }
    }
}
