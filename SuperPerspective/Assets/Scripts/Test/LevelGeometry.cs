using UnityEngine;
using System.Collections;

public class LevelGeometry : MonoBehaviour {

    public GameObject parentPlatform;

    private BoxCollider boxCollider;
    private Vector3 colliderSize;
    private Vector3 colliderCenter;
    private float zScaleRatioParent;
    private float zScaleRatioWorld;
    private float ratio;

    void Awake()
    {
        // Get the BoxCollider component
        boxCollider = GetComponent<BoxCollider>();

        // Store the ratio of our scale to the parent's
        zScaleRatioParent = parentPlatform.transform.lossyScale.z / transform.lossyScale.z;
        zScaleRatioWorld = (1 / transform.lossyScale.z);

        // Store the normal collider bounds and center
        colliderSize = new Vector3(1f, 1f, 1f);
        colliderCenter = Vector3.zero;
    }
	
	void Start () 
    {
        // Register to perspective shift event
        ShiftTester.instance.perspectiveShiftEvent += AdjustCollider;
	}

    private void AdjustCollider(PerspectiveType p)
    {
        if (p == PerspectiveType.p2D)
        {
            boxCollider.center = new Vector3(colliderCenter.x, colliderCenter.y, (parentPlatform.transform.position.z - transform.position.z) * zScaleRatioWorld);
            boxCollider.size = new Vector3(colliderSize.x, colliderSize.y, zScaleRatioParent);
        }
        else if (p == PerspectiveType.p3D)
        {
            boxCollider.size = colliderSize;
            boxCollider.center = colliderCenter;
        }
    }
}
