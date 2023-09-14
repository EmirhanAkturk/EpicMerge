using System;
using GameDepends;
using JoostenProductions;
using UnityEngine;

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


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            tileObjectDragDropController.onObjectDragStart += ObjectDragStart;
            tileObjectDragDropController.onObjectDragEnd += ObjectDragEnd;
        }

        private void ObjectDragStart()
        {
            EventService.onTileObjectDragStart?.Invoke(this);
        }

        private void ObjectDragEnd()
        {
            EventService.onTileObjectDragEnd?.Invoke(this);
        }
    }
}
