using System;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Others
{
    public class ScaleTweenController : MonoBehaviour
    {
        [Header("Show and Hide Anim")]
        [SerializeField] private float showAnimDuration = .25f;
        [SerializeField] private Ease showAnimEase = Ease.OutQuad;
        [SerializeField] private float hideAnimDuration = .25f;
        [SerializeField] private Ease hideAnimEase = Ease.InQuad;

        [Header("Punch Scale Anim")]
        [SerializeField] private float extraScaleSize = .25f;
        [SerializeField] private float scaleUpAnimDuration = 0.08f;
        [SerializeField] private float scaleDownAnimDuration = 0.04f;


        private Vector3? defaultScale;
        private Vector3? extraScale;
        private Tween scaleTween;

        public void AssignDefaultScale(Transform targetTransform)
        {
            defaultScale = targetTransform.localScale;
            extraScale = (defaultScale) + (Vector3.one * extraScaleSize);
        }

        public void ScaleTween(Transform targetTransform, bool isShow, Action endAction = null)
        {
            var disabledColliders = GameUtility.DisableChildrenColliders(targetTransform.gameObject);
            scaleTween?.Kill();

            defaultScale ??= targetTransform.localScale;

            (Vector3 startScale, Vector3 targetScale) = isShow ? (Vector3.zero, defaultScale.Value) : (defaultScale.Value, Vector3.zero);

            (float animDuration, Ease animEase) = isShow
                ? (showAnimDuration, showAnimEase)
                : (hideAnimDuration, hideAnimEase);

            targetTransform.localScale = startScale;

            scaleTween = targetTransform.DOScale(targetScale, animDuration)
                .SetEase(animEase)
                .OnKill(() =>
                {
                    targetTransform.localScale = targetScale;
                    GameUtility.EnableColliders(disabledColliders);
                })
                .OnComplete(() =>
                {
                    GameUtility.EnableColliders(disabledColliders);
                    endAction?.Invoke();
                });
        }

        public void ScalePunchShowTween(Transform targetTransform, Action endAction = null)
        {
            var disabledColliders = GameUtility.DisableChildrenColliders(targetTransform.gameObject);
            scaleTween?.Kill();

            defaultScale ??= targetTransform.localScale;
            extraScale ??= (defaultScale) + (Vector3.one * extraScaleSize);

            (Vector3 startScale, Vector3 targetScale, Vector3 targetExtraScale) = (Vector3.zero, defaultScale.Value, extraScale.Value);
            (float animDuration, Ease animEase) = (showAnimDuration, showAnimEase);

            targetTransform.localScale = startScale;

            targetTransform.DOScale(targetExtraScale, animDuration)
                .SetEase(animEase)
                .OnKill(() =>
                {
                    targetTransform.localScale = targetScale;
                    GameUtility.EnableColliders(disabledColliders);
                })
                .OnComplete(() =>
                {
                    targetTransform.DOScale(targetScale, animDuration / 3)
                        .SetEase(animEase)
                        .OnKill(() =>
                        {
                            targetTransform.localScale = targetScale;
                            GameUtility.EnableColliders(disabledColliders);
                        })
                        .OnComplete(() =>
                        {
                            GameUtility.EnableColliders(disabledColliders);
                            endAction?.Invoke();
                        });      
                });
        }

        public void PunchTween(Transform targetTransform)
        {
            var disabledColliders = GameUtility.DisableChildrenColliders(targetTransform.gameObject);

            defaultScale ??= targetTransform.localScale;
            extraScale ??= (defaultScale) + (Vector3.one * extraScaleSize);

            (Vector3 startScale, Vector3 targetScale, Vector3 targetExtraScale) = (defaultScale.Value, defaultScale.Value, extraScale.Value);

            Ease animEase = showAnimEase;

            targetTransform.DOScale(targetExtraScale, scaleUpAnimDuration)
                .SetEase(animEase)
                .OnKill(() =>
                {
                    targetTransform.localScale = targetScale;
                    GameUtility.EnableColliders(disabledColliders);
                })
                .OnComplete(() =>
                {
                    targetTransform.DOScale(targetScale, scaleDownAnimDuration)
                        .SetEase(animEase)
                        .OnKill(() =>
                        {
                            targetTransform.localScale = targetScale;
                            GameUtility.EnableColliders(disabledColliders);
                        })
                        .OnComplete(() =>
                        {
                            GameUtility.EnableColliders(disabledColliders);
                        });
                });
        }
    }
}
