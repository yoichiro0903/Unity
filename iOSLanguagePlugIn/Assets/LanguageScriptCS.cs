using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LanguageScriptCS : MonoBehaviour {
	private string lang;
	// Use this for initialization
	void Start () {
		Debug.Log ("hello.");
		lang = Binding.CurrentLanguage ();
	}
	
	// Update is called once per frame
	void Update () {
//		this.GetComponent<Text> ().text = lang;
//		Debug.Log ("debug lang text");
//		Debug.Log (lang);
	}

	void OnGUI () {
		GUI.Label(new Rect(20, 20, 100, 100), lang);
	}

}
