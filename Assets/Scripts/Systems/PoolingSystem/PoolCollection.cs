using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.PoolingSystem
{
    [CreateAssetMenu(fileName = "PoolCollection", menuName = "lib/GameDependent/PoolCollection")]
    public class PoolCollection : ScriptableObject
    {
        public List<PoolGroup> list = new List<PoolGroup>();
        public int defaultCount;
        public PoolExtendMethod extendMethod;
    }

    [Serializable]
    public class PoolElement
    {
        public PoolType type;

        public GameObject PoolObject
        {
            get
            {
                if (poolObject == null)
                {
                    poolObject = Resources.Load<GameObject>(absolutePath);
                }

                return poolObject;
            }
        }

        private GameObject poolObject;
        public string path;
        public int count;
        private string absolutePath;

        public void SetPath(string groupPath)
        {
            absolutePath = groupPath + "/" + path;
        }
    }

    public interface IPoolable
    {
        void Init();
        void Compose();
        void Despose();
    }

    public enum PoolExtendMethod
    {
        Extend,
        Loop,
        Block
    }
}