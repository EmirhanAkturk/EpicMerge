using System.Collections.Generic;
using _Game.Scripts.Systems.IndicationSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.IndicatorSystem
{
    public abstract class BaseObjectIndicator : MonoBehaviour, IIndicator
    {
        public bool IsShowingIndicator { get; private set; }

        [SerializeField] private List<GameObject> indicatorObjects;

        private void Start()
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

        protected virtual void ShowIndicator()
        {
            SetIndicatorObjectsState(IsShowingIndicator);
        }

        protected virtual void HideIndicator()
        {
            SetIndicatorObjectsState(IsShowingIndicator);
        }

        private void SetIndicatorObjectsState(bool isShowingIndicator)
        {
            foreach (var mergeIndicatorObject in indicatorObjects)
            {
                mergeIndicatorObject.SetActive(isShowingIndicator);
            }
        }
    }
}
