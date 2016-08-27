using UnityEngine;
using System.Collections;

public class GravityController : MonoBehaviour {

	const float Gravity = 9.81f;

	public float gravityScale = 1.0f;
		
	void Update () {
		
		Vector3 vector = new Vector3 ();
		if (Application.isEditor) {
			vector.x = Input.GetAxis ("Horizontal");
			vector.z = Input.GetAxis ("Vertical");

			if (Input.GetKey ("z")) {
				vector.y = 1.0f;
			} else {
				vector.y = -1.0f;
			}
		} else {
			vector.x = Input.acceleration.x;
			vector.y = Input.acceleration.z;
			vector.z = Input.acceleration.y;
		}

		Physics.gravity = Gravity * gravityScale * vector.normalized;
	}
}
