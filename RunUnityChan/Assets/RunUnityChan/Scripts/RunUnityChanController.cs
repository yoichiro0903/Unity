using UnityEngine;
using System.Collections;

public class RunUnityChanController : MonoBehaviour {
	[SerializeField]
	private UnityChanController unityChanController;
	[SerializeField]
	private GameObject obstaclePrefab;
	private float elapsedTime = 0.0f;

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			if(Physics.Raycast(ray, out raycastHit)){
				if(raycastHit.transform.gameObject.tag.Contains("UnityChan")){
					this.unityChanController.OnTapped();
				}
			}
		}
		elapsedTime += Time.deltaTime;
		if (1.5f <= elapsedTime) {
			GameObject obstacle = Instantiate (this.obstaclePrefab);
			obstacle.transform.position = new Vector3 (0.0f, 0.0f, 3.0f);
			elapsedTime = 0.0f;
		}
	}
}
