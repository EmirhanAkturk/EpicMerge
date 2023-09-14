using System;
using _Game.Scripts.Systems.TileObjectSystem;
using GameDepends;
using JoostenProductions;
using Others.TweenAnimControllers;
using UnityEngine;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeObjectController : OverridableMonoBehaviour
    {
        [Space]

        [SerializeField] private TileNodeObjectDetector tileNodeObjectDetector;
        [SerializeField] private MoveTweenAnimController moveTweenAnimController;

        private TileObject movingTileObjectInThisTile;
        private TileObject placedTileObject;
        private Vector3? centerPoint;
        
        public void Init()
        {
            ResetVariables();
            tileNodeObjectDetector.Init(ObjectEnterTileArea, ObjectExitTileArea);
        }

        private void ResetVariables()
        {
            movingTileObjectInThisTile = null;
            placedTileObject = null;
        }

        private bool isListening;
        private void ObjectEnterTileArea(TileObject tileObject)
        {
            //TODO Refactor below part!!
            // EventService.onTileObjectEnteredToNode?.Invoke(tileObject, this);
            
            movingTileObjectInThisTile = tileObject;

            if (!isListening)
            {
                EventService.onTileObjectDragEnd += PlaceObjectInSlot;
                // EventService.onTileObjectEnteredToNode += EnteredToNode;
                isListening = true;
            }
           
            MoveObjectToCenter(tileObject);
        }

        // private void EnteredToNode(TileObject tileObject, TileNodeObjectController tileNodeObjectController)
        // {
        //     if (movingTileObjectInThisTile == null || 
        //         movingTileObjectInThisTile != tileObject)
        //     {
        //         return;
        //     }
        //
        //     if (tileNodeObjectController != this)
        //     {
        //         if (isListening)
        //         {
        //             EventService.onTileObjectDragEnd -= PlaceObjectInSlot;
        //             // EventService.onTileObjectEnteredToNode -= EnteredToNode;
        //             isListening = false;
        //         }
        //         ObjectExitTileArea(tileObject);
        //     }
        // }

        private void ObjectExitTileArea(TileObject tileObject)
        {
            movingTileObjectInThisTile = null;

            if (isListening)
            {
                EventService.onTileObjectDragEnd -= PlaceObjectInSlot;
                isListening = false;
            }
        }

        private void TryPlaceObjectInSlot(TileObject tileObject)
        {
            if(tileObject != movingTileObjectInThisTile) return;
            PlaceObjectInSlot(tileObject);
        }

        private void PlaceObjectInSlot(TileObject tileObject)
        {
            MoveObjectToCenter(tileObject);
            movingTileObjectInThisTile = null;
            placedTileObject = tileObject;
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
    }
}
