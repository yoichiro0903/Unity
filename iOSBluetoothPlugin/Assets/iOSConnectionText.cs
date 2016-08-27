using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Added

public class iOSConnectionText : MonoBehaviour {
	private string bt;

	// Use this for initialization
	void Start () {
		Debug.Log ("iOSConnectionText debug");
		bt = Binding.BluetoothConnectionUnity ();
		this.GetComponent<Text> ().text = bt;
	}

	// Update is called once per frame
	void Update () {
	}

	private void recieveAccel(string bta){
		this.GetComponent<Text> ().text = bta;
		Debug.Log ("Acceleration received");
		Debug.Log (bta);
	}

	void OnGUI () {
	}

}
