using UnityEngine;
using System.Collections;

public class CandyDestroyer : MonoBehaviour {

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Candy") {
			Destroy (other.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
