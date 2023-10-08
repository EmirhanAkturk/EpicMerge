using _Game.Scripts.Systems.DetectionSystem;
using _Game.Scripts.Systems.DragDropSystem;
using _Game.Scripts.Systems.IndicatorSystem;
using _Game.Scripts.Systems.MergeSystem;
using _Game.Scripts.Systems.TileNodeSystem;
using GameDepends;
using Systems.MoveSystem;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class MergeableTileObject : BaseTileObject, IMergeableTileObject
    {
        private bool CanDrag
        {
            get { return objectDragDropController.CanDrag; }
            set { objectDragDropController.CanDrag = value; }
        }

        private bool isDraggingObject;

        
        [SerializeField] private ObjectDragDropController objectDragDropController;
  
        [Space]
        [Header("Indicator Controllers")]
        [SerializeField] private BaseIndicatorController mergeableTileObjectIndicatorController;
        [SerializeField] private BaseIndicatorController dragObjectIndicatorController;

        private IObjectDetector ObjectDetector => objectDetector ??= GetComponentInChildren<IObjectDetector>();
        private IObjectDetector objectDetector;

        [Inject] private IObjectDetectionHandler ObjectDetectionHandler { get; }

        protected override void OnDisable()
        {
            base.OnDisable();
            SetDetectionActiveState(false);
        }

        public override void Init(TileObjectValue tileObjectValue)
        {
            base.Init(tileObjectValue);
            SubscribeDragDropEvents();
            SubscribeObjectDetectionEvents();
            SetDetectionActiveState(true);
        }

        public void MoveWithoutDetection(Vector3 targetPos, MoveEndCallback onMoveEnd = null)
        {
            SetDetectionActiveState(false);
            onMoveEnd += (_) => SetDetectionActiveState(true);
            MoveController?.Move(targetPos, onMoveEnd);
        }

        public override void MoveToTargetNode(Vector3 targetPos)
        {
            CanDrag = false;
            Move(targetPos, ReachedToNode);
        }

        public void UpdateMergeableIndicatorState(bool isMergeable)
        {
            if(isDraggingObject) return;
            mergeableTileObjectIndicatorController.UpdateIndicatorState(isMergeable);    
        }
        
        public override bool CanObjectCentered()
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
            dragObjectIndicatorController.UpdateIndicatorState(state);
        }

        #region Subscribe & Unsubscribe Events

        private bool isSubscribedDragDropEvents;
        private void SubscribeDragDropEvents()
        {
            if(isSubscribedDragDropEvents) return;
            objectDragDropController.onObjectDragStart += ObjectDragStart;
            objectDragDropController.onObjectDragEnd += ObjectDragEnd;
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
