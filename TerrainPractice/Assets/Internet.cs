using UnityEngine;
using System.Collections;

public class Internet : MonoBehaviour {

	public string url = "http://ecx.images-amazon.com/images/I/51r0NvTdWxL._AA160_.jpg";
	IEnumerator Start() {
		WWW www = new WWW(url);
		yield return www;
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = www.texture;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
