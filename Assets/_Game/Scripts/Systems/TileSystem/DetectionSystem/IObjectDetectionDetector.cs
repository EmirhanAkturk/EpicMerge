using System;
using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.DetectionSystem
{
    public interface IObjectDetector
    {
        bool IsDetectionActive { get; set; }
        public Action<GameObject> OnEnteredGameObject { get; set; }
        public Action<GameObject> OnExitedGameObject { get; set; }
    }
}
