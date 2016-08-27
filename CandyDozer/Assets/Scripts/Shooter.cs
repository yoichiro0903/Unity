using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {

	const int SphereCandyFrequency = 3;

	int sampleCandyCount;

	public GameObject[] candyPrefabs;
	public GameObject[] candySquarePrefabs;
	public GameObject candyHolder;

	public float shotSpeed;
	public float shotTorque;
	public float baseWidth;

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Shot ();
		}
	}

	GameObject SampleCandy(){
		GameObject prefab = null;

		if (sampleCandyCount % SphereCandyFrequency == 0) {
			int index = Random.Range (0, candyPrefabs.Length);
			prefab = candyPrefabs [index];
		} else {
			int index = Random.Range (0, candySquarePrefabs.Length);
			prefab = candySquarePrefabs [index];
		}
		sampleCandyCount++;

		return prefab;
	}

	Vector3 GetInstantiatePosition(){
		float x = baseWidth * (Input.mousePosition.x / Screen.width) - (baseWidth / 2);
		return transform.position + new Vector3 (x, 0, 0);
	}


	public void Shot(){
		GameObject candy = (GameObject)Instantiate (
			SampleCandy(),
			GetInstantiatePosition(),
			Quaternion.identity
		);

		candy.transform.parent = candyHolder.transform;

		Rigidbody candyRigidBody = candy.GetComponent<Rigidbody> ();
		candyRigidBody.AddForce (transform.forward * shotSpeed);
		candyRigidBody.AddTorque (new Vector3 (0, shotTorque, 0));
	}
}
