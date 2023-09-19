using System.Collections.Generic;
using System.Linq;
using GameDepends.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;

namespace Systems.PanelSystem
{
    //singleton pattern for panel manager
    public class PanelManager : Singleton<PanelManager>
    {
        public PanelEvent onPanelShowed = new PanelEvent();
        public PanelEvent onPanelHidden = new PanelEvent();
    
        //Optimization
        [FormerlySerializedAs("AllExtraCanvasList")] public List<Canvas> allExtraCanvasList = new List<Canvas>();
        [FormerlySerializedAs("BasePanelsListForCloseAfter")] public List<BasePanel> basePanelsListForCloseAfter = new List<BasePanel>();

        public Transform PanelCanvas
        {
            get
            {
                if (panelsParent == null)
                {
                    Canvas mainCanvas = FindObjectOfType<MainCanvas>().GetComponent<Canvas>();
                    panelsParent = mainCanvas.transform;
                }
            
                return panelsParent;
            }
        }

        private bool isPanelDisabled;
        private const string PREFAB_PATH_PREFIX = "Panels/";
        private Transform panelsParent;
        private readonly Dictionary<PopupType, BasePanel> panels = new Dictionary<PopupType, BasePanel>();
        private readonly Dictionary<PopupType, BasePanel> loadedPrefabs = new Dictionary<PopupType, BasePanel>();

        private PopupType lastOpenedPanel = PopupType.GamePlayPanel;
    
        //Show whit panel type and panel data
        public void Show(PopupType type, PanelData data)
        {
            if (!isPanelDisabled)
            {
                var instance = GetPopup(type);

                panels.TryAdd(type, instance);

                instance.ShowPanel(data);
                onPanelShowed.Invoke(type);
            }
        }

        public bool IsThatPanelAlreadyLastSibling(PopupType type)
        {
            if (type == lastOpenedPanel)
            {
                LogUtility.PrintLog("This is last panel");
                return true;
            }
            else
            {
                lastOpenedPanel = type;
                LogUtility.PrintLog("Last panel has changed");
                return false;
            }
        }

        public void Start()
        {
            SceneManager.sceneLoaded += PreloadPopups;
        }

        private void PreloadPopups(Scene arg0, LoadSceneMode arg1)
        {
           
        }
    
        public BasePanel GetPopup(PopupType type)
        {
            if (loadedPrefabs.TryGetValue(type, out var wantedPopup))
                return wantedPopup;

            string panelPath = PREFAB_PATH_PREFIX + type.GetPopupPrefabName();
            BasePanel panelPrefab = Resources.Load<BasePanel>(panelPath);
            BasePanel panel = Instantiate(panelPrefab, PanelCanvas);

            if (type != PopupType.GamePlayPanel)
            {
                if (panel.GetComponent<Canvas>())
                {
                    panel.GetComponent<BasePanel>().SetParent(panel.transform);
                    panel.GetComponent<Canvas>().enabled = false;
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }
            }
        
            loadedPrefabs.Add(type, panel);
            return panel;
        }

        public void Hide(PopupType poolType)
        {
            if (panels.TryGetValue(poolType, out var panel))
            {
                panel.HidePanel();
                RemoveShowingPanel(poolType);
            }
        }
        
        public void Hide(BasePanel panel)
        {
            if (panels.ContainsValue(panel))
            {
                var pair = panels.First(pair => pair.Value == panel);
                panels[pair.Key].HidePanel();
                RemoveShowingPanel(pair.Key);
            }
        }

        public void RemoveShowingPanel(PopupType popupType)
        {
            panels.Remove(popupType);
        }

        public void HideAllPanel()
        {
            var panelsListCopy = panels.Values.ToList();
        
            foreach (var panel in panelsListCopy)
            {
                panel.HidePanel();
            }
        }

        public void DisablePopUps()
        {
            isPanelDisabled = true;
        }

        
        [Tooltip("Except Gameplay panel")]
        public bool IsAnyPanelShowing()
        {
            return IsAnyPanelShowed(PopupType.GamePlayPanel);
        }
        
        public bool IsPanelShowed(PopupType popupType, bool checkStillShowing = true)
        {
            if (!panels.ContainsKey(popupType)) return false;
            var panel = panels[popupType];
            return !checkStillShowing || panel.IsVisible;
        }
    
        private readonly List<PopupType> fullScreenPanels = new List<PopupType>
        {

        };

        public bool IsFullScreenPanel(PopupType popupType)
        {
            return fullScreenPanels.Contains(popupType);
        }
    
        public bool IsFullScreenPanelsShowing()
        {
            foreach (var fullScreenPanel in fullScreenPanels)
            {
                if (IsPanelShowed(fullScreenPanel))
                    return true;
            }

            return false;
        }

        public bool IsAnyPanelShowed(params PopupType[] exepts)
        {
            foreach (var panel in panels)
            {
                if (panel.Value.IsVisible && !exepts.Contains(panel.Key))
                {
                    return true;
                }
            }

            return false;
        }
    
        public void HideAllPanel(params PopupType[] exepts)
        {
            var allPanels = panels.ToList();
            foreach (var panel in allPanels)
            {
                if (panel.Value.IsVisible && !exepts.Contains(panel.Key))
                {
                    Hide(panel.Key);
                }
            }
        }
    }

    public class PanelEvent : UnityEvent<PopupType> { }
}