using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfigurations", menuName = "lib/GameConfigurations")]
public class GameConfigurations : ScriptableObject
{
	public LayerMask dragDropLayerMask;
	public int mergeRequiredObject = 3;
	
	[Header("Test")]
	public int mergeableObjectTypeCount = 3;
}

