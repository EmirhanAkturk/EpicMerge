using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Extensions
{
    public static class DoTweenExtensions
    {
        public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup cg, float endAlpha, float duration)
        {
            return DOTween.To(() => cg.alpha, (a) => cg.alpha = a, endAlpha, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DOFade(this Text txt, float endAlpha, float duration)
        {
            return DOTween.To(() => { return txt.color.a; }, (a) => { txt.color = GetColor(txt.color, a); }, endAlpha, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DOFloat(this Material material, int floatPropID, float from, float to, float duration)
        {
            float t = 0;
            return DOTween.To(() => t, (newT) =>
            {
                t = newT;
                material.SetFloat(floatPropID, Mathf.Lerp(from, to, t));
            }, 1f, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DOMoveTransform(this Transform transform, Transform toTransform, float duration)
        {
            float t = 0;
            Vector3 startPosition = transform.position;
            return DOTween.To(() => t, (newT) =>
            {
                t = newT;
                transform.position = Vector3.Lerp(startPosition, toTransform.position, t);
            }, 1f, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DORotateTransform(this Transform transform, Transform toTransform, float duration)
        {
            float t = 0;
            Quaternion startRotation = transform.rotation;
            return DOTween.To(() => t, (newT) =>
            {
                t = newT;
                transform.rotation = Quaternion.Lerp(startRotation, toTransform.rotation, t);
            }, 1f, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DOMoveDynamic(this Transform transform, Func<Vector3> positionGetter, float duration)
        {
            float t = 0;
            Vector3 startPosition = transform.position;
            return DOTween.To(() => t, (newT) =>
            {
                t = newT;
                transform.position = Vector3.Lerp(startPosition, positionGetter(), t);
            }, 1f, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DOFill(this Image img, float duration)
        {
            float t = 0;
            float startFill = img.fillAmount;
            return DOTween.To(() => t, (newT) =>
            {
                t = newT;
                img.fillAmount = Mathf.Lerp(startFill, 1f, t);
            }, 1f, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DOTextureOffset(this Material material, string name, Vector2 toOffset, float duration)
        {
            float t = 0;
            Vector2 startOffset = material.GetTextureOffset(name);
            return DOTween.To(() => t, (newT) =>
            {
                t = newT;
                Vector2 value = Vector2.Lerp(startOffset, toOffset, t);
                material.SetTextureOffset(name, value);
            }, 1f, duration);
        }
        
        private static Color GetColor(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}