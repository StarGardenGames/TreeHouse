using UnityEngine;
using System.Collections;

public class ThinBridge : MonoBehaviour {

    Collider col;
    Renderer rend;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        GameStateManager.instance.PerspectiveShiftEvent += FlipTo2D;
        GameStateManager.instance.PerspectiveShiftSuccessEvent += FlipTo3D;
    }

    void FlipTo2D(PerspectiveType p)
    {
        if (p == PerspectiveType.p3D)
        {
            col.enabled = false;
            rend.enabled = false;
        }
    }

    void FlipTo3D()
    {
        if (GameStateManager.instance.currentPerspective == PerspectiveType.p2D)
        {
            col.enabled = true;
            rend.enabled = true;
        }
    }
}
