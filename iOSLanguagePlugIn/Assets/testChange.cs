using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class testChange : MonoBehaviour {
	private string bt;
	// Use this for initialization
	void Start () {
		Debug.Log ("hello debug.");
		bt = Binding.BluetoothConnectionUnity ();
	}

	// Update is called once per frame
	void Update () {
//		this.GetComponent<Text> ().text = bt;
//		Debug.Log ("debug bt text");
//		Debug.Log (bt);
	}

	private void recieveAccel(string bta){
		this.GetComponent<Text> ().text = bta;
		Debug.Log ("debug bt text receive");
		Debug.Log (bta);
	}

	void OnGUI () {
	}

}
