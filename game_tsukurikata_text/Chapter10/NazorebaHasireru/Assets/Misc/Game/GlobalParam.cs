#if false
using UnityEngine;
using System.Collections;

// シーンをまたいで使いたいパラメーター.
public class GlobalParam : MonoBehaviour {
	
	// ================================================================ //

	public void		initialize()
	{
	}

	// ================================================================ //

	private static	GlobalParam instance = null;

	public static GlobalParam	get()
	{
		if(instance == null) {

			GameObject	go = new GameObject("GlobalParam");

			instance = go.AddComponent<GlobalParam>();

			instance.initialize();

			DontDestroyOnLoad(go);
		}

		return(instance);
	}

}
#endif