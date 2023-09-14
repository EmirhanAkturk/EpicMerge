using UnityEngine;

namespace Utils.Extensions
{
	public static class Vector3Extensions
	{
		// public static Vector3 FlattenX(this Vector3 v) => new Vector3(0, v.y, v.z);
		public static Vector3 FlattenX(this Vector3 v) => new Vector3(0, v.y, v.z);
		public static Vector3 FlattenY(this Vector3 v) => new Vector3(v.x, 0, v.z);
		public static Vector3 FlattenZ(this Vector3 v) => new Vector3(v.x, v.y, 0);
		public static Vector3 GetFlattenPos(this Vector3 v, bool flattenX, bool flattenY, bool flattenZ)
		{
			return new Vector3(flattenX ? 0 : v.x, flattenY ? 0 : v.y, flattenZ ? 0 : v.z);
		}

		// public static Vector3 FlattenZ(this Vector3 v) => new Vector3(v.x, v.y, 0);
	}
}
