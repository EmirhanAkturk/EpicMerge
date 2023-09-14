using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfigurations", menuName = "lib/GameConfigurations")]
public class GameConfigurations : ScriptableObject
{
	public LayerMask dragDropLayerMask;
}

