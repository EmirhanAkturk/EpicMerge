using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class TileObjectModelController : MonoBehaviour
    {
        [Space]
        [HelpBox("Bottom Part For Test", HelpBoxMessageType.Info)]        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private List<ValueMaterialPair> valueMaterialPairs;
        [SerializeField] private List<ValueModelPair> valueModelPairs;

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
        
        private Dictionary<int, ValueModelPair> ValueModelMaps
        {
            get
            {
                if (valueModelMaps is null)
                {
                    InitValueModelMaps();
                }

                return valueModelMaps;
            }
        }
        private Dictionary<int, ValueModelPair> valueModelMaps;
        
        private void InitValueModelMaps()
        {
            valueModelMaps ??= new Dictionary<int, ValueModelPair>();

            foreach (var pair in valueModelPairs)
            {
                valueModelMaps.Add(pair.value, pair);
            }
        }
    
        #endregion

        public void InitVisual(TileObjectValue tileObjectValue)
        {
            var valueMaterialPair = GetValueMaterialPair(tileObjectValue);
            var valueModelPair = GetValueModelPair(tileObjectValue);
            SetMaterial(valueMaterialPair.material);
            SetMesh(valueModelPair.model);
        }
        
        #region Get Functions

        private ValueMaterialPair GetValueMaterialPair(TileObjectValue tileObjectValue)
        {
            // TODO Get Model from TileObjectManager or something similar
            int objectId = tileObjectValue.objectId;
            return ValueMaterialMaps.TryGetValue(objectId, out var valueMaterialPair) ? valueMaterialPair : null;
        }        
        
        private ValueModelPair GetValueModelPair(TileObjectValue tileObjectValue)
        {
            // TODO Get Model from TileObjectManager or something similar
            int objectLevel = tileObjectValue.objectLevel;
            return ValueModelMaps.TryGetValue(objectLevel, out var valueMaterialPair) ? valueMaterialPair : null;
        }

        #endregion
    
        #region Set Functions

        private void SetMaterial(Material material)
        {
            if(material is null) return;
            meshRenderer.material = material;
        }
        
        
        private void SetMesh(Mesh mesh)
        {
            meshFilter.mesh = mesh;
        }
        #endregion
    }
}
