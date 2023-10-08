using UnityEngine;

namespace GameDepends
{
	[CreateAssetMenu(fileName = "GameConfigurations", menuName = "lib/GameConfigurations")]
	public class GameConfigurations : ScriptableObject
	{
		public LayerMask dragDropLayerMask;
	
		[Header("Merge")]
		public int mergeRequiredObject = 3;
	
		[Header("Test")]
		public int mergeableObjectTypeCount = 3;
	}
}

