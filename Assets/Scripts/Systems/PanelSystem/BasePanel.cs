using System.Collections.Generic;
using DG.Tweening;
using GameDepends;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Systems.PanelSystem
{
    public enum PanelAnimType
    {
        None,
        Shring,
        Fade,
        ShringAndFade,
        ShringAndBouncy,
    }

    public class PanelData
    {

    }

    public abstract class BasePanel : MonoBehaviour
    {
        public bool IsVisible { get; protected set; }

        [SerializeField] protected PanelAnimType animationType = PanelAnimType.ShringAndBouncy;
        [SerializeField] protected Ease easeType = Ease.OutElastic;
        [SerializeField] protected float animationDuration = 0.1f;
        [SerializeField] protected float zoomOutSize = 0.7f;
        [SerializeField] protected bool hideOnInputExit = true;
        [CanBeNull][SerializeField] protected AnimationCurve curve;

        [Space]
        [SerializeField] protected GameObject panel;
        [SerializeField] protected Image background;
        [SerializeField] protected Button exitButton;
        [SerializeField] protected float fadeAmount = 0.2f;

        [Space]
        [SerializeField] private Transform nestedCanvasParent;
        [SerializeField] private bool dontDisableOnHide;
        
        protected virtual bool SendHiddenEvent { get; } = false;
        protected abstract PopupType PopupType { get; }

        private float panelOpenTime;
        private float panelCloseTime;

        private Canvas nestedCanvas;
        
        protected HideParentPanelOnClick hideParentPanelOnClick;

        public void SetParent(Transform canvas)
        {
            nestedCanvasParent = canvas;
            nestedCanvas = nestedCanvasParent.GetComponent<Canvas>();
        }

        public virtual void Preload()
        {

        }

        protected virtual void Awake()
        {
            InitBackground();
            InitExitButton();
        }

        //single responsibility
        public virtual void ShowPanel(PanelData data)
        {
            if (animationType != PanelAnimType.None)
            {
                SetScaleSubPanel();
                EnableCanvas();
                PlayPanelOpenAnimation(animationType, animationDuration * (easeType == Ease.OutElastic ? 1.5f : 1f), easeType);
            }
            else
            {
                EnableCanvas();
            }
            panelOpenTime = Time.time;

            IsVisible = true;
        }

        public virtual void HidePanel()
        {
            if (animationType != PanelAnimType.None)
            {
                PlayPanelCloseAnimation(animationType, animationDuration);
            }
            else if(animationType == PanelAnimType.ShringAndFade)
            {
                PlayPanelCloseAnimation(animationType, animationDuration);
            }
            else
            {
                //transform.gameObject.SetActive(false);
                DisableCanvas();
            }

            panelCloseTime = Time.time;
            //AnalyticsManager.Instance.PanelClosedEvent(PopupType, panelCloseTime - panelOpenTime, GetPanelEventParams());

            if(SendHiddenEvent) PanelManager.Instance.onPanelHidden.Invoke(PopupType);
            IsVisible = false;
        }

        //single responsibility
        public virtual void ExitButtonHidePanel()
        {
            ExitButton();
            HidePanel();
        }
        
        protected void SetAsLastSibling()
        {
            transform.SetAsLastSibling();
        }

        private void SetScaleSubPanel()
        {
            if (panel == null) return;
            panel.transform.localScale = new Vector3(zoomOutSize, zoomOutSize, zoomOutSize);
        }
        
        protected virtual void ExitButton()
        {
            // Play Click Sound 
            // Play Haptic
        }

        #region Init Functions

        private void InitBackground()
        {
            if (background != null && hideOnInputExit)
                hideParentPanelOnClick = background.gameObject.AddComponent<HideParentPanelOnClick>();
        }

        private void InitExitButton()
        {
            if (exitButton == null) return;
            exitButton.onClick.RemoveListener(HidePanel);
            exitButton.onClick.RemoveListener(ExitButton);
            exitButton.onClick.AddListener(ExitButtonHidePanel);
        }

        #endregion

        #region Panel Anims
        
        private void PlayPanelOpenAnimation(PanelAnimType animType, float duration, Ease ease)
        {
            KillPlayingTweens();
            if (animType == PanelAnimType.Shring)
            {
                panel?.transform.DOScale(1f, duration).SetUpdate(true);
                background?.DOFade(fadeAmount, duration).From(0).SetUpdate(true).OnComplete(EnableCanvas);
            }
            else if (animType == PanelAnimType.Fade)
            {
                background?.DOFade(fadeAmount, duration).SetUpdate(true);
            }
            else if (animType == PanelAnimType.ShringAndBouncy)
            {
                panel?.transform.DOScale(1f, duration).SetUpdate(true).SetEase(curve);
                background?.DOFade(fadeAmount, duration).From(0).SetUpdate(true).OnComplete(EnableCanvas);
            }
            else
            {
                EnableCanvas();
            }
        }

        private void PlayPanelCloseAnimation(PanelAnimType animType, float duration)
        {
            KillPlayingTweens();

            if (animType == PanelAnimType.Shring)
            {
                panel?.transform.DOScale(zoomOutSize, duration).SetUpdate(true);
                background?.DOFade(0, duration).SetUpdate(true).OnComplete(DisableCanvas);
            }
            else if (animType == PanelAnimType.Fade)
            {
                background?.DOFade(0, duration).SetUpdate(true).OnComplete(DisableCanvas);
            }
            else if (animType == PanelAnimType.ShringAndBouncy)
            {
                panel?.transform.DOScale(zoomOutSize, duration).SetUpdate(true);
                background?.DOFade(0, duration).SetUpdate(true).OnComplete(DisableCanvas);
            }
            else
            {
                DisableCanvas();
            }
        }

        private void KillPlayingTweens()
        {
            if (panel != null)
                DOTween.Kill(panel.transform);
            DOTween.Kill(background);
        }

        #endregion
        
        protected virtual void EnableCanvas()
        {
            if (dontDisableOnHide)
            {
                transform.GetComponent<RectTransform>().localScale = Vector3.one;
                if (!nestedCanvas.enabled)
                {
                    nestedCanvas.enabled = true;
                }
                return;
            }
        
            if (nestedCanvas)
            {
                nestedCanvas.enabled = true;
            }
            else
            {
                transform.gameObject.SetActive(true);
                LogUtility.PrintLog("NestedCanvasNotFound");
            }
        }

        protected virtual void DisableCanvas()
        {
            if (dontDisableOnHide)
            {
                transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 0f, 1f);
                return;
            }
        
            if (nestedCanvas)
            {
                nestedCanvas.enabled = false;
            }
            else
            {
                transform.gameObject.SetActive(false);
                LogUtility.PrintLog("NestedCanvasNotFound");
            }
        }
    
    }
}