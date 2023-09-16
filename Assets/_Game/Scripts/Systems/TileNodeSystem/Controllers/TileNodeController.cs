using System.Collections.Generic;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
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

        #region Init Functions

        protected override void OnEnable()
        {
            base.OnEnable();
            tileNodeObjectController.onPlacedTileObjectChanged += PlacedTileObjectChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            tileNodeObjectController.onPlacedTileObjectChanged -= PlacedTileObjectChanged;
        }

        public void Init(TileObject tileObject)
        {
            tileNodeObjectController.Init(tileObject);
            tileNode.Init(GetTileObjectValue(tileObject));
        }

        private void PlacedTileObjectChanged(TileObject tileObject)
        {
            UpdateTileNodeValue(tileObject);
        }

        private void UpdateTileNodeValue(TileObject tileObject)
        {
            tileNode.SetValue(GetTileObjectValue(tileObject));
        }

        private TileObjectValue GetTileObjectValue(TileObject tileObject)
        {
            return tileObject != null ? tileObject.TileObjectValue : new TileObjectValue(-1, 0);
        }
        
        #endregion

    }
}
