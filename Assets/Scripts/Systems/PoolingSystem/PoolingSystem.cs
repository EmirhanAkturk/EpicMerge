using System;
using System.Collections.Generic;
using _Game.Scripts.GameDepend.Zenject.Factories;
using GameDepends.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Zenject;

namespace Systems.PoolingSystem
{
    public class PoolingSystem: Singleton<PoolingSystem>
    {
        [Inject] private PoolObjectFactoryInterface poolObjectFactory;
        
        private  bool isInitialized = false;
        private  PoolCollection poolCollection;
        private readonly Dictionary<PoolType, PoolElement> poolDictionary = new Dictionary<PoolType, PoolElement>();
        private readonly Dictionary<PoolType, Queue<GameObject>> pool = new Dictionary<PoolType, Queue<GameObject>>();
        public List<PoolRep> poolObjects = new List<PoolRep>();

        private const string POOL_PATH = "Configurations/PoolCollection";
        private static GameObject _poolParent;
    
        public  void InstantiatePool()
        {
            poolCollection = Resources.Load<PoolCollection>(POOL_PATH);
            _poolParent = new GameObject("PoolParent");
            _poolParent.AddComponent<DontDestroyOnLoad>();

            foreach (var poolGroup in poolCollection.list)
            {
                foreach (var pool in poolGroup.PoolElements)
                {
                    pool.SetPath(poolGroup.groupPath);
                    int count = pool.count > 0 ? pool.count : poolCollection.defaultCount;
                    this.pool.Add(pool.type, new Queue<GameObject>());
                    poolDictionary.Add(pool.type,pool);
                    for (int i = 0; i < count; i++)
                    {
                        AddToPool(pool.type, pool.PoolObject);
                    }
                }
            }

            isInitialized = true;
        }

        public T Create<T>(PoolType pooltype, Transform parent = null)
        {
            var go = Create(pooltype, parent);
            return go.GetComponent<T>();
        }

        public GameObject Create(PoolType pooltype, Transform parent = null)
        {
            CheckInitialized();

            if (pool[pooltype].Count <= 0 && poolCollection.extendMethod == PoolExtendMethod.Extend)
            {
                AddToPool(pooltype, GetPoolObjectPrefab(pooltype));
            }

            var go = pool[pooltype].Dequeue();
            poolObjects.RemoveAll(gg => { return gg.go == go;});
            if (go == null)
            {
                return Create(pooltype, parent);
            }
            go.SetActive(true);

            if (parent != null && parent != go.transform.parent)
            {
                go.transform.SetParent(parent);
            }

            var poolable = go.GetComponent<IPoolable>();
            poolable?.Compose();

            if (poolCollection.extendMethod == PoolExtendMethod.Loop)
            {
                pool[pooltype].Enqueue(go);
                poolObjects.Add(new PoolRep() { go = go, pt = pooltype });
            }

            return go;
        }
        
        public void Destroy(PoolType name, GameObject poolObject, bool changeParent = true)
        {
            CheckInitialized();
            
            if (poolObject == null || !poolObject.activeInHierarchy) return;
            
            var poolable = poolObject.GetComponent<IPoolable>();
            poolable?.Despose();

            if (poolCollection.extendMethod != PoolExtendMethod.Loop)
            {
                pool[name].Enqueue(poolObject);
                poolObjects.Add(new PoolRep() {go = poolObject, pt = name });
            }
            if (changeParent)
            {
                if (poolObject.transform is RectTransform) poolObject.transform.SetParent(_poolParent.transform);
                else poolObject.transform.parent = _poolParent.transform;
            }

            poolObject.transform.position = Vector3.zero - Vector3.back * 10;

            poolObject.SetActive(false);
        }

        public void AddBatch(List<PoolType> objects, int count)
        {
            CheckInitialized();
            
            foreach (var item in objects)
            {
                for (int i = 0; i < count; i++)
                {
                    AddToPool(item, poolDictionary[item].PoolObject);
                }
            }
        }

        private  void AddToPool(PoolType name, GameObject poolObject)
        {
            if (poolObject == null)
            {
                LogUtility.PrintColoredError($"Pool type : {name} object is null!!");
                return;
            }

            
            var go = poolObjectFactory.Create(poolObject, _poolParent.transform);
            // go = Instantiate(poolObject, _poolParent.transform, true);
            go.SetActive(false);
            
            var poolable = go.GetComponent<IPoolable>();
            poolable?.Init();

            pool[name].Enqueue(go);
            poolObjects.Add(new PoolRep() { go = go, pt = name });
        }

        public GameObject GetPoolObjectPrefab(PoolType poolType)
        {
            CheckInitialized();
            return poolDictionary[poolType].PoolObject;
        }

        private void CheckInitialized()
        {
            if (isInitialized) return;
            InstantiatePool();
        }
    }

    [Serializable]
    public class PoolRep
    {
        public GameObject go;
        public PoolType pt;
    }
}