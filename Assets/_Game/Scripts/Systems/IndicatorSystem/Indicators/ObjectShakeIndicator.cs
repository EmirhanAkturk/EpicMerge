using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.Scripts.Systems.IndicatorSystem
{
    public class ObjectShakeIndicator : BaseIndicator
    {
        [SerializeField] private Transform modelParent;
        
        [SerializeField] private Vector3 shakeStrength = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private float shakeDuration = 1.0f;
        [SerializeField] private float shakePeriod = 2.0f;

        
        private Tween shakeTween; 
        private Vector3? defaultLocalPos;

        protected override void Init()
        {
            base.Init();
            defaultLocalPos ??= modelParent.localPosition;
        }

        protected override void ShowIndicator()
        {
            PlayShakeAnimation(shakePeriod);
        }

        protected override void HideIndicator()
        {
            StopShakeAnim();
        }

        private void PlayShakeAnimation(float period)
        {
            StopShakeAnim();
            
            Debug.Log("PlayShakeAnimation");
            Transform targetTr = modelParent;
            
            shakeTween = targetTr.DOShakePosition(shakeDuration, shakeStrength)
                .SetLoops(-1) 
                .SetEase(Ease.InOutQuad);
        }
        
        private void StopShakeAnim()
        {
            shakeTween?.Kill();
            ResetPosition();
        }

        private void ResetPosition()
        {
            if(!defaultLocalPos.HasValue) return;
            modelParent.localPosition = defaultLocalPos.Value;
        }
    }
}
