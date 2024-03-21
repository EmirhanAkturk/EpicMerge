using _Game.Scripts.Systems.DetectionSystem;
using GameDepends._Game.Scripts.GameDepend.Zenject.Factories;
using UnityEngine;
using Zenject;
namespace Systems.PoolingSystem.Zenject
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IObjectDetectionHandler>().To<TileObjectDetectionHandler>().AsTransient();

            Container.BindFactory<Object, Transform, GameObject, PoolObjectFactoryInterface>()
                .FromFactory<PoolObjectFactory>();
        }
    }
}