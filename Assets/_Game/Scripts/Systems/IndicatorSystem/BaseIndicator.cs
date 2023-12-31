using System.Collections.Generic;
using _Game.Scripts.Systems.IndicationSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.IndicatorSystem
{
    public abstract class BaseIndicator : MonoBehaviour, IIndicator
    {
        public bool IsShowingIndicator { get; private set; }

        private void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            HideIndicator();
        }

        public void UpdateIndicatorState(bool isMergeable)
        {
            if(isMergeable) 
                TryShowIndicator();
            else
                TryHideIndicator();
        }

        private void TryShowIndicator()
        {
            if(IsShowingIndicator) return;
            IsShowingIndicator = true;
            ShowIndicator();
        }

        private void TryHideIndicator()
        {
            if(!IsShowingIndicator) return;
            IsShowingIndicator = false;
            HideIndicator();
        }

        protected abstract void ShowIndicator();
        protected abstract void HideIndicator();
    }
}
