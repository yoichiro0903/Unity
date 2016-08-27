using UnityEngine;
using System.Runtime.InteropServices;

public class Binding {
	[DllImport("__Internal")]
	private static extern string BluetoothConnectionUnity_ ();

	public static string BluetoothConnectionUnity () {
		Debug.Log("Bluetooth Connection Unity debug");
		if (Application.platform != RuntimePlatform.OSXEditor) {
			return BluetoothConnectionUnity_ ();
		} else {
			Debug.Log("Not iOS Device");
			return "It's not iOS Device";
		}
	}
}