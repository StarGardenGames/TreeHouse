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
	private Vector3 startCenter;
	private Quaternion startRotation;
	private float zDiff;

    #endregion Properties & Variables


    #region Monobehavior Implementation

    void Awake()
    {
		if (parentPlatform == null) {
			parentPlatform = GameObject.Find("Ground");
		}

    }
	
	void Start () 
    {
        // Register to perspective shift event
        GameStateManager.instance.PerspectiveShiftEvent += AdjustPosition;
		boxCollider = GetComponent<BoxCollider>();
		startCenter = boxCollider.center;
		colliderSize = boxCollider.size;

		AdjustPosition(PerspectiveType.p2D);
	}

    #endregion Monobehavior Implementation


    #region Perspective Shift Event

    
    // Adjusts the collider to the appropriate shape when the perspective shift event occurs.
    private void AdjustPosition(PerspectiveType p)
    {
		//Mathf.Pow(Mathf.Sin(rot * Mathf.Deg2Rad), 2)
		float rot = Mathf.Floor(transform.rotation.eulerAngles.y);
		float sign = 1;
		if (rot >= 90 && rot <= 270)
			sign = -1;
		if (p == PerspectiveType.p2D)
		{
			boxCollider.size = new Vector3(colliderSize.x * Mathf.Cos(rot * Mathf.Deg2Rad) + (parentPlatform.transform.lossyScale.z / transform.lossyScale.x) * Mathf.Sin(rot * Mathf.Deg2Rad), colliderSize.y, 
			                               colliderSize.z * Mathf.Sin(rot * Mathf.Deg2Rad) + (parentPlatform.transform.lossyScale.z / transform.lossyScale.z) * Mathf.Cos(rot * Mathf.Deg2Rad));
			boxCollider.center = new Vector3(sign * (parentPlatform.transform.position.z - transform.position.z) * (1 / (transform.localScale.x)) * Mathf.Sin(rot * Mathf.Deg2Rad), startCenter.y, 
			                                 sign * (parentPlatform.transform.position.z - transform.position.z) * (1 / (transform.localScale.z)) * Mathf.Cos(rot * Mathf.Deg2Rad));
		}
        else if (p == PerspectiveType.p3D)
        {
			boxCollider.center = startCenter;
			boxCollider.size = colliderSize;
        }
    }

    #endregion Perspective Shift Event
}
