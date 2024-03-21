using System;
using UnityEngine;
namespace _Game.Scripts.Systems.TileSystem.DetectionSystem
{
    public interface IObjectDetector
    {
        bool IsDetectionActive { get; set; }
        public Action<GameObject> OnEnteredGameObject { get; set; }
        public Action<GameObject> OnExitedGameObject { get; set; }
    }
}
