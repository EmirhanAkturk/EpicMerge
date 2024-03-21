using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem.TileObjects;
using UnityEngine;
using Zenject;
namespace GameDepends._Game.Scripts.GameDepend.Zenject.Factory
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
            // TODO Use Pooling
            var baseTileObject = diContainer.InstantiatePrefabForComponent<BaseTileObject>(obj);
            return baseTileObject;
        }
    }

    public class TileObjectFactoryInterface : PlaceholderFactory<Object, BaseTileObject>
    {
    
    }
}