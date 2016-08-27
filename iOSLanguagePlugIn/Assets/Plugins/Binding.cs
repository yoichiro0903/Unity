using UnityEngine;
using System.Runtime.InteropServices;

public class Binding {
    [DllImport("__Internal")]
	private static extern string CurrentLanguage_ ();
	[DllImport("__Internal")]
	private static extern string BluetoothConnectionUnity_ ();
 
    public static string CurrentLanguage () {
		Debug.Log("Current Language");
        if (Application.platform != RuntimePlatform.OSXEditor) {
            return CurrentLanguage_ ();
        } else {
			Debug.Log("Not iOS Lang");
        	return "It's not iOS Lang";
        }
    }

	public static string BluetoothConnectionUnity () {
		Debug.Log("Bluetooth Connection");
		if (Application.platform != RuntimePlatform.OSXEditor) {
			return BluetoothConnectionUnity_ ();
		} else {
			Debug.Log("Not iOS BT");
			return "It's not iOS BT";
		}
	}
}