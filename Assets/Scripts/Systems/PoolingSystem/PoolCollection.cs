using System;
using System.Collections.Generic;
using System.Text;
using GameDepends.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.PoolingSystem
{
    [CreateAssetMenu(fileName = "PoolCollection", menuName = "lib/PoolCollection")]
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
        private string pathPrefix = "PoolObjects/";

        public void SetPath(string groupPath)
        {
            absolutePath = GetFullPath(groupPath);;
        }

        private string GetFullPath(string groupPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append(pathPrefix);
            stringBuilder.Append(groupPath);
            stringBuilder.Append("/");
            stringBuilder.Append(path);

            return stringBuilder.ToString();
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