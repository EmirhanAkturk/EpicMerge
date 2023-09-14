using UnityEngine;

namespace Utils.Extensions
{
    public static class TransformExtensions
    {
        public static void SetParent(this Transform transform, Transform parent, bool worldPositionStays = true, bool resetLocalPosition = true)
        {
            transform.SetParent(parent, worldPositionStays);
            if (resetLocalPosition) transform.localPosition = Vector3.zero;
        }

        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                GameObject.Destroy(child.gameObject);
        }

        public static void DestroyChildren(this Transform transform, int startIdx)
        {
            DestroyChildren(transform, startIdx, transform.childCount);
        }
        public static void DestroyChildren(this Transform transform, int startIdx, int endIdx)
        {
            if (transform.childCount == 0 || startIdx > transform.childCount - 1) return;

            var children = transform.GetChildren();
            endIdx = Mathf.Min(endIdx, transform.childCount);
            for (int i = startIdx; i < endIdx; i++)
                GameObject.Destroy(children[i].gameObject);
        }

        public static Transform[] GetChildren(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];
            int i = 0;
            foreach (Transform child in transform)
                children[i++] = child;
            return children;
        }
    
        public static RectTransform[] GetChildren(this RectTransform transform)
        {
            RectTransform[] children = new RectTransform[transform.childCount];
            int i = 0;
            foreach (RectTransform child in transform)
                children[i++] = child;
            return children;
        }

        public static void CenterToChild(this Transform parent, Transform child)
        {
            Vector3 posDiff = child.position - parent.position;
            parent.position += posDiff;
            foreach (Transform childTr in parent)
                childTr.position -= posDiff;
        }
    }
}
