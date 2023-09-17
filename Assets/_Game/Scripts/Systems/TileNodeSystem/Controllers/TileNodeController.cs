using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using JoostenProductions;
using UnityEngine;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeController : OverridableMonoBehaviour
    {
        public TileNode TileNode => tileNodeObjectController.ThisTileNode;
        
        [Space]
        [SerializeField] private GameObject tileObjectPrefab; // for test
        
        [SerializeField] private TileNodeObjectController tileNodeObjectController;
        // [SerializeField] private TileNode tileNode;

        #region Init Functions

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     SubscribeEvents();
        // }
        //
        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //     UnsubscribeEvents();
        // }
        
        public void Init(TileNode tileNode, TileObject tileObject)
        {
            tileNodeObjectController.Init(tileNode, tileObject);
        }

        #endregion
        

        private TileObject CreateNewTileObject(TileObjectValue newValue)
        {
            Vector3 pos = transform.position + Vector3.up;
            var tileObjectGo = Instantiate(tileObjectPrefab, pos, Quaternion.identity, transform);
            var tileObject = tileObjectGo.GetComponent<TileObject>();
            tileObject.Init(newValue);
            return tileObject;
        }
        
        
        #region Subscribe & Unsubscribe Events
        
        // private bool isSubscribedEvents;
        // private void SubscribeEvents()
        // {
        //     if(isSubscribedEvents) return;
        //     
        //     tileNodeObjectController.onPlacedTileObjectChanged += PlacedTileObjectChanged;
        //     isSubscribedEvents = true;
        // }
        //
        // private void UnsubscribeEvents()
        // {
        //     if(!isSubscribedEvents) return;
        //     
        //     tileNodeObjectController.onPlacedTileObjectChanged -= PlacedTileObjectChanged;
        //     isSubscribedEvents = false;
        // }

        #endregion
    }
}
