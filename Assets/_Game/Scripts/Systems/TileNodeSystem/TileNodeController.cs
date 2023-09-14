using System.Collections.Generic;
using JoostenProductions;
using UnityEngine;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeController : OverridableMonoBehaviour
    {
        public TileNode TileNode => tileNode;
        
        [Space]
        [SerializeField] private TileNodeObjectController tileNodeObjectController;
        [SerializeField] private TileNode tileNode;

        #region Test
        
        [Space]
        [HelpBox("Bottom Part For Test", HelpBoxMessageType.Info)]        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private List<ValueMaterialPair> valueMaterialPairs;
        
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

        #region Init Functions

        public void Init(int value)
        {
            tileNodeObjectController.Init();
            tileNode.Init(value);
            InitVisual();
        }

        private void InitVisual()
        {
            var valueMaterialPair = GetValueMaterialPair(GetTileObjectValue());
            SetMaterial(valueMaterialPair.material);
        }
        
        #endregion
        
        #region Set Functions

        private void SetMaterial(Material material)
        {
            if(material is null) return;
            meshRenderer.material = material;
        }


        #endregion
    
        #region Get Functions

        private int GetTileObjectValue()
        {
            return tileNode.Value;
        }

        private ValueMaterialPair GetValueMaterialPair(int value)
        {
            return ValueMaterialMaps.TryGetValue(value, out var valueMaterialPair) ? valueMaterialPair : null;
        }

        #endregion
    }
}
