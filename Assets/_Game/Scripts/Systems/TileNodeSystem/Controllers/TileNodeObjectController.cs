using System;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem;
using _Game.Scripts.Utility;
using GameDepends;
using JoostenProductions;
using Others.TweenAnimControllers;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeObjectController : OverridableMonoBehaviour
    {
        public Action<TileObject> onPlacedTileObjectChanged;
        public Func<TileObject, bool> onTryMerge;
        public Func<TileObject, bool> onCanMerge;

        [Space]

        [SerializeField] private TileNodeObjectDetectionHandler tileNodeObjectDetectionHandler;

        private TileObject placedTileObject;
        private TileObject movingTileObjectOnThisTile;
        private Vector3? centerPoint;

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeAllEvents();
        }

        public void Init(TileObject initObject)
        {
            ResetVariables();
            SubscribeTileNodeDetectorEvents();
            InitPlacedObject(initObject);
        }

        public void UpdateTileObject(TileObject tileObject)
        {
            if (placedTileObject != null)
            {
                Destroy(placedTileObject.gameObject);
            }
            
            InitPlacedObject(tileObject);
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
            if(!tileObject.CanDrag) return;
            
            movingTileObjectOnThisTile = tileObject;
            SubscribeObjectDragEnd();
            MoveObjectToTileCenter(tileObject);

            if (placedTileObject != null)
            {
                if (onCanMerge != null)
                {
                    bool canMerge = onCanMerge.Invoke(tileObject);
                    Debug.Log("canMerge : " + canMerge);
                } 
            }
        }

        private void ObjectExitTileArea(TileObject tileObject)
        {
            movingTileObjectOnThisTile = null;
            UnsubscribeObjectDragEnd();
        }

        private void TileObjectDragEnd(TileObject tileObject)
        {
            if (placedTileObject is null)
            {
                TryPlaceObjectInTile(tileObject);
                return;
            }

            bool? isMerged = onTryMerge?.Invoke(tileObject);
            if (isMerged != null && isMerged.Value)
            {
                // TODO Try Merge
                
            }
            else
            {
                
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
            CheckTileObjectEventSubState();
            onPlacedTileObjectChanged?.Invoke(tileObject);
            Debug.Log("Set placedTileObject" + (placedTileObject is null ? "NULL" : "tile object") + ", Node Name : " + gameObject.name);
        }

        private void MoveObjectToTileCenter(TileObject tileObject)
        {
            Vector3 targetPos = GetCenterPoint();
            tileObject.CanDrag = false;
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

        #region Subscribe Unsubscribe Events

        private void UnsubscribeAllEvents()
        {
            UnsubscribeObjectDragEnd();
            UnsubscribeTileObjectEvents();
        }
        
        private bool isSubTileDragEndEvent;
        private void SubscribeObjectDragEnd()
        {
            if (isSubTileDragEndEvent) return;
            EventService.onTileObjectDragEnd += TileObjectDragEnd;
            isSubTileDragEndEvent = true;
        }

        private void UnsubscribeObjectDragEnd()
        {
            if (!isSubTileDragEndEvent) return;
            EventService.onTileObjectDragEnd -= TileObjectDragEnd;
            isSubTileDragEndEvent = false;
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
        
        private void SubscribeTileNodeDetectorEvents()
        {
            tileNodeObjectDetectionHandler.onTileObjectEntered += ObjectEnterTileArea;
            tileNodeObjectDetectionHandler.onTileObjectExited += ObjectExitTileArea;
        }

        #endregion
    }
}
