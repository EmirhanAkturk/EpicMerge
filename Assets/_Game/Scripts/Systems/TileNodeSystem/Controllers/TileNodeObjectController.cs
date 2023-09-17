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

        public TileNode TileNode { get; private set; }
        
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
            TileNode = tileNode;
            // tileNode.Init(GetTileObjectValue(initObject));
            ResetVariables();
            SubscribeInitEvents();
            InitPlacedObject(initObject);
        }

        public void UpdateMergedTileObjectValue(TileObjectValue tileObjectValue)
        {
            if (TileObjectValue.IsEmptyTileObjectValue(tileObjectValue))
            {
                Destroy(placedTileObject.gameObject);
                placedTileObject = null;
                movingTileObjectOnThisTile = null;
            }
            else if (placedTileObject != null)
            {
                placedTileObject.Init(tileObjectValue);
            }
            
            UnsubscribeAllObjectEvents();
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

        private void ObjectPlacedToTile(TileNodeObjectController tileNodeObjectController, TileObject tileObject)
        {
            if(placedTileObject == null || placedTileObject != tileObject) return;
            if (tileNodeObjectController != this)
            {
                // The object previously placed in this tile is now placed in another tile
                SetPlacedObject(null);
            }
        }

        private void AfterObjectDragEnd(TileObject tileObject)
        {
            if(tileObject != placedTileObject) return;
            Debug.Log("ObjectDragEnd : " + gameObject.name);
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
                Debug.Log("###TryPlaceObjectInTile");
                TryPlaceObjectInTile(tileObject);
                return;
            }

            bool isMerged = false;
            if (placedTileObject != tileObject && placedTileObject.TileObjectValue.Equals(tileObject.TileObjectValue))
            {
                Debug.Log("###TryPlaceObjectInTile");
                isMerged = TryMerge(tileObject);
            }
            if (isMerged)
            {
                // TODO Try Merge
                Debug.Log("###isMerged true : " + gameObject.name);
            }
            else
            {
                Debug.Log("###isMerged False" + gameObject.name);
            }
            // TODO Move Or Swap objects pos 
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
            EventService.onTileObjectPlacedToTile?.Invoke(this, tileObject);
        }

        private void SetPlacedObject(TileObject tileObject)
        {
            placedTileObject = tileObject;
            if (placedTileObject != null) tileObject.TileNode = TileNode; 
            CheckTileObjectEventSubState();
            // onPlacedTileObjectChanged?.Invoke(tileObject);
            PlacedTileObjectChanged(tileObject);
            Debug.Log("Set placedTileObject" + (placedTileObject is null ? "NULL" : "tile object") + ", Node Name : " + gameObject.name);
        }

        private void MoveObjectToTileCenter(TileObject tileObject)
        {
            Vector3 targetPos = GetCenterPoint();
            tileObject.CanDrag = false;
            // tileObject.MoveWithoutDetection(targetPos, _ => TileObjectMoveEnd(tileObject));
            tileObject.Move(targetPos, _ => TileObjectMoveEnd(tileObject));
        }

        private void TileObjectMoveEnd(TileObject tileObject)
        {
            tileObject.CanDrag = true;
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

        private bool CanMerge(TileObject tileObject)
        {
            return tileObject != null && TileObjectMergeHelper.CanMerge(tileObject.TileNode, TileNode, tileObject.TileObjectValue);
        }

        private bool TryMerge(TileObject tileObject)
        {
            return TileObjectMergeHelper.TryMerge(tileObject.TileNode,TileNode, tileObject.TileObjectValue);
        }

        private void TileObjectMerged(TileObjectValue newValue)
        {
            UpdateTileNodeValue(newValue);
            UpdateMergedTileObjectValue(newValue);
        }
        
        private void PlacedTileObjectChanged(TileObject tileObject)
        {
            UpdateTileNodeValue(GetTileObjectValue(tileObject));
        }
        
        #endregion
        
        #region Node Functions
        
        private void UpdateTileNodeValue(TileObjectValue tileObjectValue)
        {
            TileNode.SetValue(tileObjectValue);
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
            TileNode.onTileObjectMerged += TileObjectMerged;
            SubscribeTileNodeDetectorEvents();
            isSubscribedInitEvents = true;
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

        private void UnSubscribeInitEvents()
        {
            if(!isSubscribedInitEvents) return;
            TileNode.onTileObjectMerged -= TileObjectMerged;
            UnsubscribeTileNodeDetectorEvents();
            isSubscribedInitEvents = false;
        }

        #endregion
    }
}
