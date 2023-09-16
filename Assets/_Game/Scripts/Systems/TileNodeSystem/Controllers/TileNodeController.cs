using System.Collections.Generic;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using _Game.Scripts.Utility;
using JoostenProductions;
using UnityEngine;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeController : OverridableMonoBehaviour
    {
        public TileNode TileNode => tileNode;
        
        [Space]
        [SerializeField] private GameObject tileObjectPrefab; // for test
        
        [SerializeField] private TileNodeObjectController tileNodeObjectController;
        [SerializeField] private TileNode tileNode;

        #region Init Functions

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeEvents();
        }
        
        public void Init(TileObject tileObject)
        {
            tileNodeObjectController.Init(tileObject, tileNode);
            tileNode.Init(GetTileObjectValue(tileObject));
        }

        #endregion

        private bool CanMerge(TileObject tileObject)
        {
            return tileObject != null && TileObjectMergeHelper.CanMerge(tileObject.TileNode, tileNode, tileObject.TileObjectValue);
        }

        private bool TryMerge(TileObject tileObject)
        {
            return TileObjectMergeHelper.TryMerge(tileObject.TileNode,tileNode, tileObject.TileObjectValue);
        }

        private void TileObjectMerged(TileObjectValue newValue)
        {
            UpdateTileNodeValue(newValue);
            if (!TileObjectValue.IsEmptyTileObjectValue(newValue))
            {
                var tileObject = CreateNewTileObject(newValue);
                tileNodeObjectController.UpdateTileObject(tileObject);
            }
            else
            {
                tileNodeObjectController.UpdateTileObject(null);
            }
        }

        private TileObject CreateNewTileObject(TileObjectValue newValue)
        {
            Vector3 pos = transform.position + Vector3.up;
            var tileObjectGo = Instantiate(tileObjectPrefab, pos, Quaternion.identity, transform);
            var tileObject = tileObjectGo.GetComponent<TileObject>();
            tileObject.Init(newValue);
            return tileObject;
        }
        
        private void PlacedTileObjectChanged(TileObject tileObject)
        {
            UpdateTileNodeValue(GetTileObjectValue(tileObject));
        }

        private void UpdateTileNodeValue(TileObjectValue tileObjectValue)
        {
            tileNode.SetValue(tileObjectValue);
        }

        private TileObjectValue GetTileObjectValue(TileObject tileObject)
        {
            return tileObject != null ? tileObject.TileObjectValue : TileObjectValue.GetEmptyTileObjectValue();
        }
        
        #region Subscribe & Unsubscribe Events
        
        private bool isSubscribedEvents;
        private void SubscribeEvents()
        {
            if(isSubscribedEvents) return;
            
            tileNodeObjectController.onPlacedTileObjectChanged += PlacedTileObjectChanged;
            tileNodeObjectController.onTryMerge += TryMerge;
            tileNodeObjectController.onCanMerge += CanMerge;
            tileNode.onTileObjectMerged += TileObjectMerged;

            isSubscribedEvents = true;
        }

        private void UnsubscribeEvents()
        {
            if(!isSubscribedEvents) return;
            
            tileNodeObjectController.onPlacedTileObjectChanged -= PlacedTileObjectChanged;
            tileNodeObjectController.onTryMerge -= TryMerge;
            tileNodeObjectController.onCanMerge -= CanMerge;
            tileNode.onTileObjectMerged -= TileObjectMerged;
            
            isSubscribedEvents = false;
        }

        #endregion
    }
}
