using System;
using UnityEngine;

namespace _Game.Scripts.Systems.DetectionSystem
{
    [RequireComponent(typeof(Collider))]
    public class GameObjectDetector : MonoBehaviour, IObjectDetector
    {
        public bool IsDetectionActive { get; set; }
        public Action<GameObject> OnEnteredGameObject { get; set; }
        public Action<GameObject> OnExitedGameObject { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if(!IsDetectionActive) return;
            OnEnteredGameObject?.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!IsDetectionActive) return;
            OnExitedGameObject?.Invoke(other.gameObject);
        }
    }
}
