using _Game.Scripts.GameDepend.Zenject.Factories;
using _Game.Scripts.Systems.DetectionSystem;
using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IObjectDetectionHandler>().To<TileObjectDetectionHandler>().AsTransient();

        Container.BindFactory<Object, BaseTileObject, TileObjectFactoryInterface>()
            .FromFactory<TileObjectFactory>();
    }
}