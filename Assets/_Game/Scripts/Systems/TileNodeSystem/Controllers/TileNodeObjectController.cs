using System;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using _Game.Scripts.Utility;
using GameDepends;
using JoostenProductions;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeObjectController : OverridableMonoBehaviour
    {
        [Space]

        [SerializeField] private TileNodeObjectDetectionHandler tileNodeObjectDetectionHandler;

        public TileNode ThisTileNode { get; private set; }
        
        private TileObject placedTileObject;
        private TileObject movingTileObjectOnThisTile;
        private Vector3? centerPoint;

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeAllEvents();
        }

        public void Init(TileNode tileNode, TileObject initObject)
        {
            ThisTileNode = tileNode;
            // tileNode.Init(GetTileObjectValue(initObject));
            ResetVariables();
            SubscribeInitEvents();
            InitPlacedObject(initObject);
        }

        public void UpdateMergedTileObjectValue(TileObjectValue tileObjectValue)
        {
            movingTileObjectOnThisTile = null;

            if (tileObjectValue.IsEmptyTileObjectValue())
            {
                Destroy(placedTileObject.gameObject);
                placedTileObject = null;
            }
            else if (placedTileObject != null)
            {
                placedTileObject.Init(tileObjectValue);
            }
            
            // UnsubscribeAllObjectEvents();
            // InitPlacedObject(placedTileObject);
        }

        private void InitPlacedObject(TileObject initObject)
        {
            if(initObject == null) return;
            movingTileObjectOnThisTile = initObject;
            TryPlaceObjectInTile(initObject);
        }

        private void ResetVariables()
        {
            movingTileObjectOnThisTile = null;
            placedTileObject = null;
            
            UnsubscribeAllEvents();
        }

        private void ObjectPlacedToTile(TileNode newTileNode, TileObject tileObject)
        {
            if(placedTileObject == null || placedTileObject != tileObject) return;
            if (newTileNode != ThisTileNode)
            {
                // The object previously placed in this tile is now placed in another tile
                SetPlacedObject(null);
            }
        }

        private void AfterObjectDragEnd(TileObject tileObject)
        {
            if(tileObject != placedTileObject) return;
            // Debug.Log("ObjectDragEnd : " + gameObject.name);
            MoveObjectToTileCenter(tileObject);
        }

        private void ObjectEnterTileArea(TileObject tileObject)
        {
            //TODO Refactor below part!!
            // EventService.onTileObjectEnteredToNode?.Invoke(tileObject, this);
            
            // TODO Due to the CanDrag control, the object may not be centered in the slot when the drag ends
            if (!tileObject.CanDrag)
            {
                Debug.Log("### tileObject.CanDrag return : " + gameObject.name);
                return;
            }
            
            Debug.Log("### tileObject.CanDrag not return : " + gameObject.name);

            
            movingTileObjectOnThisTile = tileObject;
            MoveObjectToTileCenter(tileObject);

            if (placedTileObject != null && placedTileObject != tileObject)
            {
                bool canMerge = CanMerge(tileObject);
                Debug.Log("canMerge : " + canMerge);
            }
        }

        private void ObjectExitTileArea(TileObject tileObject)
        {
            movingTileObjectOnThisTile = null;
        }

        private void TileObjectPlaced(TileObject tileObject)
        {
            if (placedTileObject is null)
            {
                // Debug.Log("###TryPlaceObjectInTile");
                TryPlaceObjectInTile(tileObject);
                return;
            }

            bool isMerged = false;
            if (CanTryMerge(tileObject))
            {
                // Debug.Log("###TryPlaceObjectInTile");
                isMerged = TryMerge(tileObject);
            }
            if (!isMerged)
            {
                var targetTileNode = ThisTileNode.GetEmptyNeighbor() ?? tileObject.TileNode;
                targetTileNode.onTileObjectChanged?.Invoke(placedTileObject);
                
                PlaceObjectOnTile(tileObject);
                Debug.Log("###isMerged False" + gameObject.name);
            }
            else
            {
                // TODO Try Merge
                Debug.Log("###isMerged true : " + gameObject.name);
            }
        }
        
        private void TryPlaceObjectInTile(TileObject tileObject)
        {
            Debug.Log("TryPlaceObjectIn : " + gameObject.name);
            if(tileObject != movingTileObjectOnThisTile) return;

            if (placedTileObject == tileObject)
            {
                // Already on this tile, just move to the center
                MoveObjectToTileCenter(tileObject);
            }
            else
            {
                // Place object on the this slot
                PlaceObjectOnTile(tileObject);
            }
        }

        private void PlaceObjectOnTile(TileObject tileObject)
        {
            MoveObjectToTileCenter(tileObject);
            movingTileObjectOnThisTile = null;
            SetPlacedObject(tileObject);
            EventService.onTileObjectPlacedToTile?.Invoke(ThisTileNode, tileObject);
        }

        private void SetPlacedObject(TileObject tileObject)
        {
            placedTileObject = tileObject;
            if (placedTileObject != null) tileObject.TileNode = ThisTileNode; 
            CheckTileObjectEventSubState();
            // onPlacedTileObjectChanged?.Invoke(tileObject);
            PlacedTileObjectChanged(tileObject);
            Debug.Log("Set placedTileObject" + (placedTileObject is null ? "NULL" : "tile object") + ", Node Name : " + gameObject.name);
        }

        private void MoveObjectToTileCenter(TileObject tileObject)
        {
            Vector3 targetPos = GetCenterPoint();
            tileObject.MoveToTargetNode(targetPos);
        }

        private void PlacedTileObjectChanged(TileObject tileObject)
        {
            UpdateTileNodeValue(GetTileObjectValue(tileObject));
        }
        
        private Vector3 GetCenterPoint()
        {
            centerPoint ??= transform.position;
            return centerPoint.Value;
        }
        
        private void CheckTileObjectEventSubState()
        {
            if (placedTileObject != null)
                SubscribeTileObjectEvents();
            else
                UnsubscribeTileObjectEvents();
        }

        #region Merge Functions

        private bool CanTryMerge(TileObject tileObject)
        {
            return placedTileObject != tileObject && placedTileObject.TileObjectValue.Equals(tileObject.TileObjectValue);
        }
        
        private bool CanMerge(TileObject tileObject)
        {
            return tileObject != null && TileObjectMergeHelper.CanMerge(tileObject.TileNode, ThisTileNode, tileObject.TileObjectValue);
        }

        private bool TryMerge(TileObject tileObject)
        {
            return TileObjectMergeHelper.TryMerge(tileObject.TileNode,ThisTileNode, tileObject.TileObjectValue);
        }

        private void TileObjectMerged(TileObjectValue newValue)
        {
            UpdateTileNodeValue(newValue);
            UpdateMergedTileObjectValue(newValue);
        }

        #endregion
        
        #region Node Functions
        
        private void UpdateTileNodeValue(TileObjectValue tileObjectValue)
        {
            ThisTileNode.SetValue(tileObjectValue);
        }
        
        private TileObjectValue GetTileObjectValue(TileObject tileObject)
        {
            return tileObject != null ? tileObject.TileObjectValue : TileObjectValue.GetEmptyTileObjectValue();
        }
        
        #endregion
        
        #region Subscribe Unsubscribe Events

        private void UnsubscribeAllEvents()
        {
            UnsubscribeAllObjectEvents();
            UnSubscribeInitEvents();
        }        
        
        private void UnsubscribeAllObjectEvents()
        {
            UnsubscribeTileObjectEvents();
        }
        
        private bool isSubTileObjectEvents;
        private void SubscribeTileObjectEvents()
        {
            if(isSubTileObjectEvents) return;
            EventService.onAfterTileObjectDragEnd += AfterObjectDragEnd;
            EventService.onTileObjectPlacedToTile += ObjectPlacedToTile;
            isSubTileObjectEvents = true;
        }

        private void UnsubscribeTileObjectEvents()
        {
            // TODO unsubscribe can only be done in OnDisable 
            if(!isSubTileObjectEvents) return;
            EventService.onAfterTileObjectDragEnd -= AfterObjectDragEnd;
            EventService.onTileObjectPlacedToTile -= ObjectPlacedToTile;
            isSubTileObjectEvents = false;
        }

        private bool isSubscribedInitEvents;
        private void SubscribeInitEvents()
        {
            if(isSubscribedInitEvents) return;
            ThisTileNode.onTileObjectMerged += TileObjectMerged;
            ThisTileNode.onTileObjectChanged += PlaceObjectOnTile;
            
            SubscribeTileNodeDetectorEvents();
            isSubscribedInitEvents = true;
        }

        private void UnSubscribeInitEvents()
        {
            if(!isSubscribedInitEvents) return;
            ThisTileNode.onTileObjectMerged -= TileObjectMerged;
            ThisTileNode.onTileObjectChanged -= PlaceObjectOnTile;
            
            UnsubscribeTileNodeDetectorEvents();
            isSubscribedInitEvents = false;
        }
        
        private void SubscribeTileNodeDetectorEvents()
        {
            tileNodeObjectDetectionHandler.onTileObjectEntered += ObjectEnterTileArea;
            tileNodeObjectDetectionHandler.onTileObjectExited += ObjectExitTileArea;
            tileNodeObjectDetectionHandler.onTileObjectPlaced += TileObjectPlaced;
        }        
        
        private void UnsubscribeTileNodeDetectorEvents()
        {
            tileNodeObjectDetectionHandler.onTileObjectEntered -= ObjectEnterTileArea;
            tileNodeObjectDetectionHandler.onTileObjectExited -= ObjectExitTileArea;
            tileNodeObjectDetectionHandler.onTileObjectPlaced -= TileObjectPlaced;
        }
        #endregion
    }
}
