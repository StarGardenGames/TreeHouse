using UnityEngine;
using System.Collections;

using SuperPerspective.Singleton;

public class ShiftTester : Singleton<ShiftTester>
{

    public event System.Action<PerspectiveType> perspectiveShiftEvent;
    private PerspectiveType p = PerspectiveType.p3D;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //Debug.Log("SHIFTING");

            if (perspectiveShiftEvent != null)
            {
                if (p == PerspectiveType.p2D)
                {
                    perspectiveShiftEvent(PerspectiveType.p3D);
                    p = PerspectiveType.p3D;
                }
                else
                {
                    perspectiveShiftEvent(PerspectiveType.p2D);
                    p = PerspectiveType.p2D;
                }
            }
        }
    }
}


