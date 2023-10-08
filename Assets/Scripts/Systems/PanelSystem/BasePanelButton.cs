using System.Collections.Generic;
using GameDepends;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Systems.PanelSystem
{
    public class BasePanelButton : MonoBehaviour
    {
        [SerializeField] protected PopupType popupType;
        [SerializeField] protected bool hasButtonObjectDifferent;
        [SerializeField] protected bool checkPanelShowing = false;
        [SerializeField] protected bool hideWhenPanelOpen = false;

        [SerializeField] protected GameObject visualPartParent;
        [ShowIf("hasButtonObjectDifferent")] [SerializeField] private Button button;
        [ShowIf("hideWhenPanelOpen")] [SerializeField] protected List<PopupType> hideWhenThesePanelOpen;

        protected Button Button => button ??= GetComponent<Button>();

        private List<PopupType> targetShowingPanels;

        protected virtual void Start()
        {
            if (hideWhenPanelOpen) targetShowingPanels = new List<PopupType>();
        }

        protected virtual void OnEnable()
        {
            Button.onClick.AddListener(TryOpenPanel);
            if (hideWhenPanelOpen)
            {
                PanelManager.Instance.onPanelShowed.AddListener(PanelShowed);
                PanelManager.Instance.onPanelHidden.AddListener(PanelHidden);
            }
        }

        protected virtual void OnDisable()
        {
            Button.onClick.RemoveListener(TryOpenPanel);
            if (hideWhenPanelOpen)
            {
                PanelManager.Instance.onPanelShowed.RemoveListener(PanelShowed);
                PanelManager.Instance.onPanelHidden.RemoveListener(PanelHidden);
            }
        }

        protected virtual void TryOpenPanel()
        {
            if(checkPanelShowing && PanelManager.Instance.IsPanelShowed(popupType)) return;
            OpenPanel();
        }

        protected virtual void OpenPanel()
        {
            PanelManager.Instance.Show(popupType, new PanelData());
        }

        protected void SetVisualPartState(bool state)
        {
            visualPartParent.SetActive(state);
        }

        private void PanelShowed(PopupType type)
        {
            if(type != popupType && !hideWhenThesePanelOpen.Contains(type)) return;

            if (!targetShowingPanels.Contains(type))
            {
                LogUtility.PrintLog("Add targetShowingPanels : " + type);
                targetShowingPanels.Add(type);
            }
        
            SetVisualPartState(false);
        }

        private void PanelHidden(PopupType type)
        {
            if(type != popupType && !hideWhenThesePanelOpen.Contains(type)) return;
        
            targetShowingPanels.Remove(type);
        
            if (targetShowingPanels.Count == 0)
            {
                TryShowButton();
            }
        }

        protected virtual void TryShowButton()
        {
            SetVisualPartState(true);
        }
    }
}
