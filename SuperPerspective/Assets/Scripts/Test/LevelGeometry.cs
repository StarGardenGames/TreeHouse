using UnityEngine;
using System.Collections;

/// <summary>
///     Attach this script to any level geometry that must be collidable in 2D perspective.
///     Stretches the collider of the geometry to match the Z depth of the platform the geometry is part of.
/// </summary>
public class LevelGeometry : MonoBehaviour
{

    #region Properties & Variables

    public GameObject parentPlatform;   // The platform this geometry belongs to

    private BoxCollider boxCollider;    // Reference to this object's BoxCollider
    private Vector3 colliderSize;       // Stores the collider's beginning size, usually (1, 1, 1)
    private float zScaleRatioParent;    // Ratio of this object's z scale to the parent platform's
    private float zScaleRatioWorld;     // Ratio of this object's z scale to the world

    #endregion Properties & Variables


    #region Monobehavior Implementation

    void Awake()
    {
        // Get the BoxCollider component
        boxCollider = GetComponent<BoxCollider>();

        // Store the ratio of our scale to the parent's
        zScaleRatioParent = parentPlatform.transform.lossyScale.z / transform.lossyScale.z;
        zScaleRatioWorld = (1 / transform.lossyScale.z);

        // Store the normal collider bounds and center
        colliderSize = new Vector3(1f, 1f, 1f);
    }
	
	void Start () 
    {
        // Register to perspective shift event
        GameStateManager.instance.PerspectiveShiftEvent += AdjustCollider;
	}

    #endregion Monobehavior Implementation


    #region Perspective Shift Event

    
    // Adjusts the collider to the appropriate shape when the perspective shift event occurs.
    private void AdjustCollider()
    {
        if (GameStateManager.instance.targetState == "2D")
        {
            // Stretch the collider's Z depth and center z value to match parent platform
            boxCollider.center = new Vector3(0f, 0f, (parentPlatform.transform.position.z - transform.position.z) * zScaleRatioWorld);
            boxCollider.size = new Vector3(colliderSize.x, colliderSize.y, zScaleRatioParent);
        }
        else if (GameStateManager.instance.targetState == "3D")
        {
            // Return collider to initial state
            boxCollider.size = colliderSize;
            boxCollider.center = Vector2.zero;
        }
    }

    #endregion Perspective Shift Event
}
