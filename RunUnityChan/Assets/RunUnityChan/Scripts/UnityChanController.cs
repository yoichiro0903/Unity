using UnityEngine;
using System.Collections;

public class UnityChanController : MonoBehaviour {
	public void OnTapped(){
		this.GetComponent<Animator>().SetTrigger ("Jump");
	}

	public void OnCallChangeFace(){
	}

}
