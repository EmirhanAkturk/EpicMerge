using _Game.Scripts.Systems.TileObjectSystem;
using GameDepends.Enums;
using Systems.PoolingSystem;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.GameDepend.Zenject.Factories
{
    public class TileObjectFactory : IFactory<Object, BaseTileObject>
    {
        private readonly DiContainer diContainer;
        
        public TileObjectFactory(DiContainer container)
        {
            diContainer = container;
        }
        
        public BaseTileObject Create(Object obj)
        {
            var baseTileObject = diContainer.InstantiatePrefabForComponent<BaseTileObject>(obj);
            return baseTileObject;
        }
    }

    public class TileObjectFactoryInterface : PlaceholderFactory<Object, BaseTileObject>
    {
    
    }
}