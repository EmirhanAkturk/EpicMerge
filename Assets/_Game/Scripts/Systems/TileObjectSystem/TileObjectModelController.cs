using System.Collections.Generic;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class TileObjectModelController : MonoBehaviour
    {
        [Space]
        [HelpBox("Bottom Part For Test", HelpBoxMessageType.Info)]        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private List<ValueMaterialPair> valueMaterialPairs;

        #region ValueMaterialPairs
    
        private Dictionary<int, ValueMaterialPair> ValueMaterialMaps
        {
            get
            {
                if (valueMaterialMaps is null)
                {
                    InitValueMaterialMaps();
                }

                return valueMaterialMaps;
            }
        }
        private Dictionary<int, ValueMaterialPair> valueMaterialMaps;
        
        private void InitValueMaterialMaps()
        {
            valueMaterialMaps ??= new Dictionary<int, ValueMaterialPair>();

            foreach (var pair in valueMaterialPairs)
            {
                valueMaterialMaps.Add(pair.value, pair);
            }
        }
    
        #endregion

        public void InitVisual(TileObjectValue tileObjectValue)
        {
            var valueMaterialPair = GetValueMaterialPair(tileObjectValue);
            SetMaterial(valueMaterialPair.material);
        }
    
        #region Get Functions

        private ValueMaterialPair GetValueMaterialPair(TileObjectValue tileObjectValue)
        {
            // TODO Get Model from TileObjectManager or something similar
            int objectId = tileObjectValue.objectId;
            return ValueMaterialMaps.TryGetValue(objectId, out var valueMaterialPair) ? valueMaterialPair : null;
        }

        #endregion
    
        #region Set Functions

        private void SetMaterial(Material material)
        {
            if(material is null) return;
            meshRenderer.material = material;
        }


        #endregion
    }
}
