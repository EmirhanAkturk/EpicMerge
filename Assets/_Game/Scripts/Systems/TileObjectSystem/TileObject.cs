using System;
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
        public bool CanDrag
        {
            get { return tileObjectDragDropController.CanDrag; }
            set { tileObjectDragDropController.CanDrag = value; }
        }

        public TileObjectValue TileObjectValue { get; private set; } 
        public TileNode TileNode { get;  set; } // for test 
        
        [SerializeField] private TileObjectDragDropController tileObjectDragDropController;
        [SerializeField] private TileObjectModelController tileObjectModelController;
        
        private IMoveController MoveController => moveController ??= GetComponent<IMoveController>();
        private IMoveController moveController;

        // TODO Use inject below part
        private IObjectDetectionHandler ObjectDetectionHandler => objectDetectionHandler ??= new TileObjectDetectionHandler();
        private IObjectDetectionHandler objectDetectionHandler;
        

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

        private void SetDetectionActiveState(bool isDetectionActive)
        {
            ObjectDetectionHandler.IsDetectionActive = isDetectionActive;
        }

        // private void Start()
        // {
        //     Init();
        // }
        

        public void Init(TileObjectValue tileObjectValue)
        {
            TileObjectValue = tileObjectValue;
            tileObjectModelController.InitVisual(TileObjectValue);
            SubscribeDragDropEvents();
            // Debug.Log(" ### Init return : " + gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            ObjectDetectionHandler.TileObjectEntered(this, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            ObjectDetectionHandler.TileObjectExited(this, other.gameObject);
        }

        private void ObjectDragStart()
        {
            SetDetectionActiveState(true);
            EventService.onTileObjectDragStart?.Invoke(this);
        }

        private void ObjectDragEnd()
        {
            SetDetectionActiveState(false);
            ObjectDetectionHandler.TileObjectPlaced(this);
            EventService.onTileObjectDragEnd?.Invoke(this);
            EventService.onAfterTileObjectDragEnd?.Invoke(this);
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

        #endregion
    }
}
