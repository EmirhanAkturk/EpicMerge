using _Game.Scripts.Systems.TileSystem.DetectionSystem;
using _Game.Scripts.Systems.TileSystem.EventSystem;
using _Game.Scripts.Systems.TileSystem.TileMergeSystem;
using GameDepends._Game.Scripts.GameDepend.Zenject.Factories;
using Systems.PoolingSystem;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.GameDepend.Zenject
{
    /// <summary>
    /// ProjectContext'e eklenir — proje genelinde geçerli binding'ler.
    /// Sahneye özgü MonoBehaviour'lar (PanelManager, TileGraphGeneratorManager, TileObjectManager)
    /// için SceneInstaller'a bakın (SceneContext'e eklenmeli).
    /// </summary>
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // ── Event System ─────────────────────────────────────────────────
            var eventService = new EventService();
            Container.Bind<IEventService>().FromInstance(eventService).AsSingle();

            // ── Merge System ─────────────────────────────────────────────────
            Container.Bind<ITileObjectMergeHelper>().To<TileObjectMergeHelper>().AsSingle();

            // ── Detection ────────────────────────────────────────────────────
            Container.Bind<IObjectDetectionHandler>().To<TileObjectDetectionHandler>().AsTransient();

            // ── Pooling ──────────────────────────────────────────────────────
            Container.BindFactory<Object, Transform, GameObject, PoolObjectFactoryInterface>()
                .FromFactory<PoolObjectFactory>();

            Container.Bind<IPoolingSystem>()
                .To<PoolingSystem>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("PoolingSystem")
                .AsSingle()
                .NonLazy();
        }
    }
}
