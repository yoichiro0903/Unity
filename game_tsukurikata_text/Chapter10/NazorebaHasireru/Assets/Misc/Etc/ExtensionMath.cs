using UnityEngine;
using System.Collections;

// 拡張メソッド.
namespace MathExtension {

	static class Vector {

		public static Vector3	XZ(this Vector3 v, float x, float z)
		{
			return(new Vector3(x, v.y, z));
		}

		// Vector3.Y()
		// Y成分をセットする.
		public static Vector3	Y(this Vector3 v, float y)
		{
			return(new Vector3(v.x, y, v.z));
		}

		// Vector3.xz()
		// xz 成分から Vector2 をつくる.
		public static Vector2	xz(this Vector3 v)
		{
			return(new Vector2(v.x, v.z));
		}

		// Vector3.xy()
		// xy 成分から Vector2 をつくる.
		public static Vector2	xy(this Vector3 v)
		{
			return(new Vector2(v.x, v.y));
		}

		public static Vector3	to_vector3(this Vector2 v, float z = 0.0f)
		{
			return(new Vector3(v.x, v.y, z));
		}
	};
};
