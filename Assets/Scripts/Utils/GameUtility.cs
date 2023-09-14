using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public static class GameUtility
{
	public static float DistanceSqr(Vector3 v1, Vector3 v2)
	{
		return (v2 - v1).sqrMagnitude;
	}

	public static float GetJumpPower(Transform objTrans, Vector3 targetPos)
	{
		return .25f;
		// var jumpPower = Mathf.Max(objTrans.position.y, targetPos.y);
		// jumpPower = Mathf.Min(jumpPower, .25f);
		// return objTrans.localScale.magnitude * ConfigurationService.Configurations.AnimJumpPower + jumpPower;
	}
	
	public static float GetJumpPowerLocal(Transform objTrans, Vector3 targetPos)
	{
		return .25f;
		// var jumpPower = Mathf.Max(objTrans.position.y, targetPos.y);
		// jumpPower = Mathf.Min(jumpPower, 2f);
		// return ConfigurationService.Configurations.AnimJumpPower + jumpPower;
	}
	
	public static bool IsNull(Component component, bool checkIsDestroyed = true)
	{
		return component == null || (!checkIsDestroyed || component.Equals(null));
	}
    
	public static bool IsNull(GameObject go, bool checkIsDestroyed = true)
	{
		return go == null || (!checkIsDestroyed || go.Equals(null));
	}

	public static List<Collider> DisableChildrenColliders(GameObject obj, bool includeInactive = true)
	{
		List<Collider> disabledColliders = new List<Collider>();
		var colliders = obj.GetComponentsInChildren<Collider>(true);
		foreach (var col in colliders)
		{
			if (col.enabled)
			{
				disabledColliders.Add(col);
				col.enabled = false;
			}
		}
		return disabledColliders;
	}
	public static void EnableColliders(List<Collider> colliders)
	{
		foreach (var col in colliders)
		{
			col.enabled = true;
		}
	}

	public static string SplitCamelCase(string input)
	{
		return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
	}
}
