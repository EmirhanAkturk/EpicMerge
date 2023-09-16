using System;
using _Game.Scripts.Systems.TileSystem;
using GameDepends;
using JoostenProductions;
using UnityEngine;
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

        [SerializeField] private TileObjectDragDropController tileObjectDragDropController;
        
        private IMoveController MoveController => moveController ??= GetComponent<IMoveController>();
        private IMoveController moveController;

        
        // TODO Use inject below part
        private IObjectDetectionHandler ObjectDetectionHandler => objectDetectionHandler ??= new TileObjectDetectionHandler();
        private IObjectDetectionHandler objectDetectionHandler;

        public void Move(Vector3 targetPos, MoveEndCallback onMoveEnd = null)
        {
            MoveController?.Move(targetPos, onMoveEnd);
        }
        
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            tileObjectDragDropController.onObjectDragStart += ObjectDragStart;
            tileObjectDragDropController.onObjectDragEnd += ObjectDragEnd;
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
            EventService.onTileObjectDragStart?.Invoke(this);
        }

        private void ObjectDragEnd()
        {
            EventService.onTileObjectDragEnd?.Invoke(this);
            EventService.onAfterTileObjectDragEnd?.Invoke(this);
        }
    }
}
