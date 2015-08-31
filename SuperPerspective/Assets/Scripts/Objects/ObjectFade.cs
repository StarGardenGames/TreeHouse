using UnityEngine;
using System.Collections;

/**
 * Causes objects to become transparent when inbetween the camera and player in 3D
 **/
public class ObjectFade : MonoBehaviour {

	float setAlpha = 1, fadeSpeed = 0.15f;
	Renderer[] rends;
	GameObject player;
	
	void Start () {
		player = PlayerController.instance.gameObject;
		if (GetComponent<Renderer>())
			rends = GetComponents<Renderer>();
		else
			rends = GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in rends) {
			rend.material.DisableKeyword("_ALPHATEST_ON");
			rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		}
	}

	void Update() {
		setAlpha = 1;
		float dist;
		if (GetComponent<Collider>().bounds.IntersectRay(new Ray(Camera.main.transform.position, player.transform.position - Camera.main.transform.position), out dist)) { 
			if (dist < Vector3.Distance(player.transform.position, Camera.main.transform.position))
				setAlpha = 0.5f - Mathf.Lerp(0, 0.5f, (transform.position.x - player.transform.position.x) / (Camera.main.transform.position.x - player.transform.position.x));
		}
	}

	void FixedUpdate () {
		foreach (Renderer rend in rends) {
			Color col = rend.material.GetColor("_Color");
			if (col.a < 1) {
				rend.material.SetFloat("_Mode", 2);
				rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				rend.material.SetInt("_ZWrite", 0);
				rend.material.EnableKeyword("_ALPHABLEND_ON");
				rend.material.renderQueue = 3000;
			} else if (rend.material.GetFloat("_Mode") > 0) {
				rend.material.SetFloat("_Mode", 0);
				rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				rend.material.SetInt("_ZWrite", 1);
				rend.material.DisableKeyword("_ALPHABLEND_ON");
				rend.material.renderQueue = 3000;
			}
			if (col.a > setAlpha) {
				col.a = col.a - fadeSpeed <= setAlpha ? setAlpha : col.a - fadeSpeed;
			}
			if (col.a < setAlpha) {
				col.a = col.a + fadeSpeed >= setAlpha ? setAlpha : col.a + fadeSpeed;
			}
			rend.material.SetColor("_Color", col);
		}
	}
}
