using System;
using _Game.Scripts.Systems.TileObjectSystem;
using GameDepends;
using JoostenProductions;
using Others.TweenAnimControllers;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeObjectController : OverridableMonoBehaviour
    {
        [Space]

        [SerializeField] private TileNodeObjectDetector tileNodeObjectDetector;
        [SerializeField] private MoveTweenAnimController moveTweenAnimController;

        private TileObject placedTileObject;
        private TileObject movingTileObjectOnThisTile;
        private Vector3? centerPoint;
        
        public void Init()
        {
            ResetVariables();
            SubscribeTileNodeDetectorEvents();
            tileNodeObjectDetector.Init();
        }

        private void ResetVariables()
        {
            movingTileObjectOnThisTile = null;
            placedTileObject = null;
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
            MoveObjectToCenter(tileObject);
        }

        private void ObjectEnterTileArea(TileObject tileObject)
        {
            //TODO Refactor below part!!
            // EventService.onTileObjectEnteredToNode?.Invoke(tileObject, this);
            
            // TODO Due to the CanDrag control, the object may not be centered in the slot when the drag ends
            if(!tileObject.CanDrag) return;
            
            movingTileObjectOnThisTile = tileObject;
            SubscribeObjectDragEnd();
            MoveObjectToCenter(tileObject);
        }

        private void ObjectExitTileArea(TileObject tileObject)
        {
            movingTileObjectOnThisTile = null;
            UnsubscribeObjectDragEnd();
        }

        private void TryPlaceObjectInTile(TileObject tileObject)
        {
            Debug.Log("TryPlaceObjectIn : " + gameObject.name);
            if(tileObject != movingTileObjectOnThisTile) return;

            if (placedTileObject == tileObject)
            {
                // Already on this tile, just move to the center
                MoveObjectToCenter(tileObject);
            }
            else
            {
                // Place object on the this slot
                PlaceObjectOnTile(tileObject);
            }
        }

        private void PlaceObjectOnTile(TileObject tileObject)
        {
            MoveObjectToCenter(tileObject);
            movingTileObjectOnThisTile = null;
            SetPlacedObject(tileObject);
            EventService.onTileObjectPlacedToTile?.Invoke(this, tileObject);
        }

        private void SetPlacedObject(TileObject tileObject)
        {
            placedTileObject = tileObject;
            CheckTileObjectEventSubState();
            Debug.Log("Set placedTileObject" + (placedTileObject is null ? "NULL" : "tile object") + ", Node Name : " + gameObject.name);
        }

        private void MoveObjectToCenter(TileObject tileObject)
        {
            Vector3 targetPos = GetCenterPoint();
            tileObject.CanDrag = false;
            
            void MoveEnd() => TileObjectMoveEnd(tileObject);
            moveTweenAnimController.MoveAnim(tileObject.transform, targetPos, MoveEnd, MoveEnd);
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

        private bool isSubTileDragEndEvent;
        private void SubscribeObjectDragEnd()
        {
            if (isSubTileDragEndEvent) return;
            EventService.onTileObjectDragEnd += TryPlaceObjectInTile;
            isSubTileDragEndEvent = true;
        }

        private void UnsubscribeObjectDragEnd()
        {
            if (!isSubTileDragEndEvent) return;
            EventService.onTileObjectDragEnd -= TryPlaceObjectInTile;
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
            tileNodeObjectDetector.onTileObjectEntered += ObjectEnterTileArea;
            tileNodeObjectDetector.onTileObjectExited += ObjectExitTileArea;
        }

        #endregion
    }
}
