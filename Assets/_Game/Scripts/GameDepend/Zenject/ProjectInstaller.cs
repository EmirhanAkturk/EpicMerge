using _Game.Scripts.Systems.TileSystem.DetectionSystem;
using _Game.Scripts.Systems.TileSystem.EventSystem;
using _Game.Scripts.Systems.TileSystem.TileMergeSystem;
using GameDepends._Game.Scripts.GameDepend.Zenject.Factories;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.GameDepend.Zenject
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // ── Event System ─────────────────────────────────────────────────
            var eventService = new EventService();
            Container.Bind<IEventService>().FromInstance(eventService).AsSingle();
            TileObjectMergeHelper.Initialize(eventService);

            // ── Detection ────────────────────────────────────────────────────
            Container.Bind<IObjectDetectionHandler>().To<TileObjectDetectionHandler>().AsTransient();

            // ── Pooling ──────────────────────────────────────────────────────
            Container.BindFactory<Object, Transform, GameObject, PoolObjectFactoryInterface>()
                .FromFactory<PoolObjectFactory>();
        }
    }
}
