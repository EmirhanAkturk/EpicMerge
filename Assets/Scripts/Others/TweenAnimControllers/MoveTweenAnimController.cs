using System;
using DG.Tweening;
using UnityEngine;

namespace Others
{
    public class MoveTweenAnimController : MonoBehaviour
    {
        [Header("Move Anim")]
        [SerializeField] private float moveAnimDuration = .5f;
        [SerializeField] private Ease moveAnimEase = Ease.OutQuad;
        
        [Space]
        [SerializeField] private float shortMoveAnimDistance = 1f;
        [SerializeField] private float shotMoveAnimDuration = .1f;
        [SerializeField] private Ease shotMoveAnimEase = Ease.OutQuad;

        private Tween moveTween;

        public void MoveAnim(Transform targetTransform, Vector3 targetPos, Action onCompleteAction = null, Action onKillAction = null)
        {
            StopMove();
            AssignParameters(targetTransform, targetPos, out float animDuration, out Ease animEase);

            moveTween = targetTransform.DOMove(targetPos, animDuration)
                .SetEase(animEase)
                .OnKill( () => onKillAction?.Invoke())
                .OnComplete(() => onCompleteAction?.Invoke());
        }

        public void LocalMoveAnim(Transform targetTransform, Vector3 targetPos, Action onCompleteAction = null, Action onKillAction = null)
        {
            StopMove();
            AssignParameters(targetTransform, targetPos, out float animDuration, out Ease animEase);
            
            moveTween = targetTransform.DOLocalMove(targetPos, animDuration)
                .SetEase(animEase)
                .OnKill( () => onKillAction?.Invoke())
                .OnComplete(() => onCompleteAction?.Invoke());
        }

        private void StopMove()
        {
            moveTween?.Kill();
        }

        private void AssignParameters(Transform targetTransform, Vector3 targetPos, out float animDuration, out Ease animEase)
        {
            float distance = Vector3.Distance(targetTransform.position, targetPos);
            bool isShortMove = distance <= shortMoveAnimDistance;
            
            (animDuration, animEase) = isShortMove
                ? (shotMoveAnimDuration, shotMoveAnimEase)
                : (moveAnimDuration, moveAnimEase);

        }
    }
}
