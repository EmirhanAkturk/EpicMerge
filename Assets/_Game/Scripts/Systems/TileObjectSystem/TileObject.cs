using System;
using _Game.Scripts.Systems.DetectionSystem;
using _Game.Scripts.Systems.IndicationSystem;
using _Game.Scripts.Systems.IndicatorSystem;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using GameDepends;
using JoostenProductions;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class TileObject : OverridableMonoBehaviour
    {
        //TODO This script was created temporarily to test the Tile Node System and will be refactored
        private bool CanDrag
        {
            get { return tileObjectDragDropController.CanDrag; }
            set { tileObjectDragDropController.CanDrag = value; }
        }

        private bool isDraggingObject;

        public TileObjectValue TileObjectValue { get; private set; } 
        public TileNode TileNode { get;  set; } // for test 
        
        [SerializeField] private TileObjectDragDropController tileObjectDragDropController;
        [SerializeField] private TileObjectModelController tileObjectModelController;
        [SerializeField] private TileObjectMergeableIndicator mergeableTileObjectIndicator;
        [SerializeField] private DragObjectIndicator dragObjectIndicator;

        private IMoveController MoveController => moveController ??= GetComponent<IMoveController>();
        private IMoveController moveController;

        private IObjectDetector ObjectDetector => objectDetector ??= GetComponentInChildren<IObjectDetector>();
        private IObjectDetector objectDetector;
        
        
        // TODO Use inject for below part
        private IObjectDetectionHandler ObjectDetectionHandler => objectDetectionHandler ??= new TileObjectDetectionHandler();
        private IObjectDetectionHandler objectDetectionHandler;
        
        public void Init(TileObjectValue tileObjectValue)
        {
            TileObjectValue = tileObjectValue;
            tileObjectModelController.InitVisual(TileObjectValue);
            SubscribeDragDropEvents();
            SubscribeObjectDetectionEvents();
            // Debug.Log(" ### Init return : " + gameObject.name);
        }

        public void Move(Vector3 targetPos, MoveEndCallback onMoveEnd = null)
        {
            MoveController?.Move(targetPos, onMoveEnd);
        }        
        
        public void MoveWithoutDetection(Vector3 targetPos, MoveEndCallback onMoveEnd = null)
        {
            SetDetectionActiveState(false);
            onMoveEnd += (_) => SetDetectionActiveState(true);
            MoveController?.Move(targetPos, onMoveEnd);
        }

        public void MoveToTargetNode(Vector3 targetPos)
        {
            CanDrag = false;
            Move(targetPos, ReachedToNode);
        }

        public void UpdateMergeableIndicatorState(bool isMergeable)
        {
            if(isDraggingObject) return;
            mergeableTileObjectIndicator.UpdateIndicatorState(isMergeable);    
        }
        
        public bool CanObjectCentered()
        {
            return CanDrag;
        }
        
        private void ReachedToNode(bool _)
        {
            CanDrag = true;
        } 
        
        private void SetDetectionActiveState(bool isDetectionActive)
        {
            ObjectDetector.IsDetectionActive = isDetectionActive;
        }

        private void EnteredGameObject(GameObject enteredGo)
        {
            ObjectDetectionHandler.TileObjectEntered(this, enteredGo);
        }

        private void ExitedGameObject(GameObject exitedGo)
        {
            ObjectDetectionHandler.TileObjectExited(this, exitedGo);
        }

        private void ObjectDragStart()
        {
            SetDetectionActiveState(true);
            UpdateDraggingState(true);
            EventService.onTileObjectDragStart?.Invoke(this);
        }

        private void ObjectDragEnd()
        {
            SetDetectionActiveState(false);
            UpdateDraggingState(false);
            ObjectDetectionHandler.TileObjectPlaced(this);
            EventService.onTileObjectDragEnd?.Invoke(this);
            EventService.onAfterTileObjectDragEnd?.Invoke(this);
        }

        private void UpdateDraggingState(bool state)
        {
            isDraggingObject = state;
            dragObjectIndicator.UpdateIndicatorState(state);
        }

        #region Subscribe & Unsubscribe Events

        private bool isSubscribedDragDropEvents;
        private void SubscribeDragDropEvents()
        {
            if(isSubscribedDragDropEvents) return;
            tileObjectDragDropController.onObjectDragStart += ObjectDragStart;
            tileObjectDragDropController.onObjectDragEnd += ObjectDragEnd;
            isSubscribedDragDropEvents = true;
        }

        private bool isSubscribedObjectDetectionEvents;
        private void SubscribeObjectDetectionEvents()
        {
            if(isSubscribedObjectDetectionEvents) return;
            ObjectDetector.OnEnteredGameObject += EnteredGameObject;
            ObjectDetector.OnExitedGameObject += ExitedGameObject;
            isSubscribedObjectDetectionEvents = true;
        }

        #endregion
    }
}
