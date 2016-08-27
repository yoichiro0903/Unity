private var lang: String;
private var bt: String;

function Start () {
	Debug.Log("hello!");
	lang = Binding.CurrentLanguage();
	bt = Binding.BluetoothConnectionUnity();
}

function OnGUI () {
	GUI.Label(Rect(20, 20, 100, 100), lang);
	GUI.Label(Rect(50, 50, 100, 100), bt);
}
