using _Game.Scripts.Systems.DetectionSystem;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.GameDepend
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