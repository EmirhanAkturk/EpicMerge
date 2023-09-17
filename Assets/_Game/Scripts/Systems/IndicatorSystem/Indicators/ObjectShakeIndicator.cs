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
            
            Transform targetTr = modelParent;
            
            // DOTween ile shake animasyonunu başlatma
            shakeTween = targetTr.DOShakePosition(shakeDuration, shakeStrength)
                // .SetId("ShakeAnimation") // Eşsiz bir ID atayın
                .SetLoops(-1) // Sonsuz döngüde çalıştırmak için -1 kullanın
                .SetEase(Ease.InOutQuad) // İsterseniz başka bir eğri kullanabilirsiniz
                .Pause() // Animasyonu başlatmadan duraklatın
                .OnPlay(() =>
                {
                    // Animasyon başladığında yapılacak işlemler
                    Debug.Log("Shake animation started");
                })
                .OnKill(() =>
                {
                    // Animasyon sona erdiğinde veya durdurulduğunda yapılacak işlemler
                    Debug.Log("Shake animation finished");
                });

            // DOTween'de zamanlayıcı kullanarak period süresi boyunca animasyonu çalıştırma
            DOTween.Sequence()
                .AppendInterval(period) // Belirtilen süre boyunca bekleme
                .OnComplete(() =>
                {
                    // Period süresi boyunca bekleme sona erdiğinde animasyonu başlatma
                    // transform.DOPlayById("ShakeAnimation");
                });
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
