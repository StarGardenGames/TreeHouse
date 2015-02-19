using UnityEngine;
using System.Collections;

/// <summary>
///     Determines if the camera should be in 3D or 2D state. In each state the camera
///     attempts to follow an appropriate camera mount's position and rotation.
/// </summary>
[RequireComponent(typeof(MatrixBlender))]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    #region Properties & Variables

    // This is the camera component and matrix blender script
    Camera cam;
    MatrixBlender blender;

    // Movement variables
    Transform CameraMount2D;        // The transform to move to when changed to 2D
    Transform CameraMount3D;        // The transform to move to when changed to 3D
    Vector3 velocity;               // Used by SmoothDamp function
    Quaternion startRotation;       // Used to Slerp rotation to new mount
    float smoothTime = .2f;         // Used to control the damp speed
    float turnSpeed = 3f;           // USed to dictate how quickly the camera can match its target rotation
    float cameraBlendSpeed = .07f;  // USed to determine how quickly the camera changes from perspective to orthographic and vice versa

    // Camera Matrix Variables
    private Matrix4x4 ortho;                // Used to store orthographic camera parameters
    private Matrix4x4 perspective;          // Used to store perspective camera parameters
    public float fov = 60f;                 // The camera's field of view while in 3D mode
    public float near = -1f;                // The near clipping plane of the camera
    public float far = 1000f;               // The far clipping plane of the camera
    public float orthographicSize = 10f;    // The orthographic size variable of the camera in 2D mode
    private float aspect;                   // THe aspect ratio of the camera

    // State machine variables
    private string currentState;            
    private const string STATE_2D = "2D";   
    private const string STATE_3D = "3D";

    #endregion Properties & Variables


    #region Monobehavior Implementation

    // Use this for initialization
	void Start () {

        // Find the camera mounts and necessary Components
        CameraMount2D = GameObject.Find("2DCameraMount").transform;
        CameraMount3D = GameObject.Find("3DCameraMount").transform;
        cam = this.GetComponent<Camera>();
        blender = this.GetComponent<MatrixBlender>();

        // Set up matrix parameters
        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        camera.projectionMatrix = perspective;

        // Register the state switching function to the perspective shift event
        
		InputManager.instance.perspectiveShiftEvent += SwitchState;  

        // Start the camera in 3D state
        currentState = STATE_3D;
        StartCoroutine(Execute3DBehavior());
	}

    #endregion Monobehavior Implementation


    #region State Functions

    // Registered to InputManager's perspective change event. Switches the current camera state.
    private void SwitchState(PerspectiveType newPerspective)
    {
        if (newPerspective == PerspectiveType.p2D)
        {
            currentState = STATE_2D;
            //Debug.Log("Switching to 2D");
        }
        else if (newPerspective == PerspectiveType.p3D)
        {
            currentState = STATE_3D;
            //Debug.Log("Switching to 3D");
        }
    }

    // Runs when the camera is in 2D mode. The camera will attempt to follow the 2D camera mount with damping.
    private IEnumerator Execute2DBehavior()
    {
        while(currentState == STATE_2D)
        {
            // Smoothdamp the camera towards the 2D camera mount and blend the camera matrix to the 2D settings
            transform.position = Vector3.SmoothDamp(transform.position, CameraMount2D.position, ref velocity, smoothTime);
            blender.BlendToMatrix(ortho, cameraBlendSpeed);

            // If we haven't matched the 2D mount's rotation yet rotate to match
            if (!(transform.rotation == CameraMount2D.rotation))
                transform.rotation = Quaternion.RotateTowards(transform.rotation, CameraMount2D.rotation, turnSpeed);
             
            yield return null;
        }

        // Switch to 3D coroutine on state change
        if (currentState != STATE_2D)
        {
            startRotation = transform.rotation;
            StartCoroutine(Execute3DBehavior());
        }
    }

    // Runs when the camera is in 3D mode. The camera will attempt to follow the 3D camera mount with damping.
    private IEnumerator Execute3DBehavior()
    {
        while (currentState == STATE_3D)
        {
            // SmoothDamp the camera towards the 3D mount's position and blend the camera's matrix to the 3D settings
            transform.position = Vector3.SmoothDamp(transform.position, CameraMount3D.position, ref velocity, smoothTime);
            blender.BlendToMatrix(perspective, cameraBlendSpeed);

            // Rotate to match the 3D mount's rotation. Always do this since it can change when uses tweaks camera
            transform.rotation = Quaternion.RotateTowards(transform.rotation, CameraMount3D.rotation, turnSpeed);

            yield return null;
        }

        // Switch to the 2D coroutine on state change
        if (currentState != STATE_3D)
        {
            startRotation = transform.rotation;
            StartCoroutine(Execute2DBehavior());
        }
    }

    #endregion State Functions
}
