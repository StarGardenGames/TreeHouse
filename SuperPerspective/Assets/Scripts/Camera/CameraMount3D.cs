using UnityEngine;
using System.Collections;

public class CameraMount3D : MonoBehaviour
{

    #region Properties & Variables

    public float maxLeanAngle = 30f;
    public GameObject player;

    #endregion Properties & Variables

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(KeyCode.Semicolon))
        {
            transform.RotateAround(player.transform.position, Vector3.up, maxLeanAngle * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Quote))
        {
            transform.RotateAround(player.transform.position, Vector3.up, -maxLeanAngle * Time.deltaTime);
        }
	}
}
