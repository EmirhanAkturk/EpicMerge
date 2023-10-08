using UnityEngine;
using Zenject;

namespace _Game.Scripts.GameDepend
{
    public class PoolObjectFactory : IFactory<Object, Transform, GameObject>
    {
        private readonly DiContainer diContainer;
        
        public PoolObjectFactory(DiContainer container)
        {
            diContainer = container;
        }
        
        public GameObject Create(Object targetObject, Transform parentTr)
        {
            return diContainer.InstantiatePrefab(targetObject, parentTr);

        }
    }

    public class PoolObjectFactoryInterface : PlaceholderFactory<Object, Transform, GameObject>
    {
    
    }
}
