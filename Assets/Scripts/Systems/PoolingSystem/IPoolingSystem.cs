using System.Collections.Generic;
using GameDepends;
using UnityEngine;

namespace Systems.PoolingSystem
{
    public interface IPoolingSystem
    {
        void InstantiatePool();
        T Create<T>(PoolType pooltype, Transform parent = null);
        GameObject Create(PoolType pooltype, Transform parent = null);
        void Destroy(PoolType name, GameObject poolObject, bool changeParent = true);
        void AddBatch(List<PoolType> objects, int count);
        GameObject GetPoolObjectPrefab(PoolType poolType);
    }
}
