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

        private void ObjectEnterTileArea(TileObject tileObject)
        {
            movingTileObjectInThisTile = tileObject;
            EventService.onTileObjectMoveEnd += PlaceObjectInSlot;
           
            MoveObjectToCenter(tileObject);
        }

        private void ObjectExitTileArea(TileObject tileObject)
        {
            movingTileObjectInThisTile = null;
            EventService.onTileObjectMoveEnd -= PlaceObjectInSlot;
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
