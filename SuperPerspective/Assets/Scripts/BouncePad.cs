using UnityEngine;
using System.Collections;

public class BouncePad : MonoBehaviour {

	public float bouncePower = 50;
	private float t, damp;
	private bool animating;
	private Vector3 startScale;

	void Start() {
		startScale = transform.localScale;
	}

	void FixedUpdate() {
		if (animating) {
			Vector3 newScale = transform.localScale;
			newScale.x = startScale.x + (startScale.x * 0.2f) * Mathf.Sin(t) * damp;
			newScale.y = startScale.y - (startScale.y * 0.2f) * Mathf.Sin(t) * damp;
			newScale.z = startScale.z + (startScale.z * 0.2f) * Mathf.Sin(t) * damp;
			transform.localScale = newScale;
			t += Mathf.PI / 8;
			if (t >= Mathf.PI * 2) {
				if (damp < 0.1) {
					animating = false;
					transform.localScale = startScale;
				} else
					damp /= 2;
				t = 0;
			}
		}
	}

	public float GetBouncePower() {
		return bouncePower;
	}

	public void Animate() {
		damp = 1;
		animating = true;
	}
}
