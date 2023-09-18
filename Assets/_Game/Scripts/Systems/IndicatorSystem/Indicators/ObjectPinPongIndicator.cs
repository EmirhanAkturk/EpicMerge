using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;

namespace _Game.Scripts.Systems.IndicatorSystem
{
    public class ObjectPinPongIndicator : BaseIndicator
    {
        [SerializeField] private Transform targetTransform;

        [Space]
        [SerializeField] private float swingDuration = 0.8f;
        [SerializeField] private float swingDistance = 0.4f;

        private Sequence sequence;
        private Vector3? defaultLocalPos;

        protected override void Init()
        {
            base.Init();
            defaultLocalPos ??= targetTransform.localPosition;
        }

        protected override void ShowIndicator()
        {
            PlayAnimation();
        }

        protected override void HideIndicator()
        {
            StopAnim();
        }

        private void PlayAnimation()
        {
            StopAnim();

            sequence = DOTween.Sequence();

            sequence.Append(targetTransform.DOLocalMoveX(swingDistance, swingDuration)); // 1
            sequence.Append(targetTransform.DOLocalMoveX(0f, swingDuration / 4)); // 0
            sequence.Append(targetTransform.DOLocalMoveX(-swingDistance, swingDuration)); // -1
            sequence.Append(targetTransform.DOLocalMoveX(0f, swingDuration / 4)); // 0

            sequence.SetLoops(-1);
        }

        private void StopAnim()
        {
            sequence?.Kill();
            ResetPosition();
        }

        private void ResetPosition()
        {
            if (!defaultLocalPos.HasValue) return;
            targetTransform.localPosition = defaultLocalPos.Value;
        }
    }
}
