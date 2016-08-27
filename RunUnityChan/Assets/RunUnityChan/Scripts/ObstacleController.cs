using UnityEngine;
using System;

public class ObstacleController : MonoBehaviour {
	private bool isMoving = true;
	public event Action CollidedWithUnityChan = delegate{};

	// Update is called once per frame
	void Update () {
		if(this.isMoving){
			Vector3 diff = new Vector3 (0.0f, 0.0f, 1.0f) * Time.deltaTime;
			this.gameObject.transform.position = this.gameObject.transform.position - diff;
		}

		if(this.gameObject.transform.position.z <= -1.0f){
			Destroy (this.gameObject);	
		}
	}

	void OnCollisionEnter(Collision collision){
		this.isMoving = false;
		if(collision.gameObject.tag.Contains("UnityChan")){
			this.CollidedWithUnityChan ();
		}
	}
}