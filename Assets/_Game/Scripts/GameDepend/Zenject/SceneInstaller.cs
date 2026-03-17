using _Game.Scripts.Systems.TileSystem.TileNodeSystem.GraphGenerator;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
using Systems.PanelSystem;
using Zenject;

namespace _Game.Scripts.GameDepend.Zenject
{
    /// <summary>
    /// Sahneye özgü MonoBehaviour singleton'larını bağlar.
    /// GameScene'deki SceneContext GameObject'ine eklenmelidir.
    /// </summary>
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bu manager'lar eski Singleton<T> tarafından runtime'da dinamik yaratılıyordu.
            // FromNewComponentOnNewGameObject aynı davranışı Zenject üzerinden sağlar.
            Container.Bind<IPanelManager>()
                .To<PanelManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("PanelManager")
                .AsSingle()
                .NonLazy();

            Container.Bind<ITileGraphGeneratorManager>()
                .To<TileGraphGeneratorManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("TileGraphGeneratorManager")
                .AsSingle()
                .NonLazy();

            Container.Bind<ITileObjectManager>()
                .To<TileObjectManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("TileObjectManager")
                .AsSingle()
                .NonLazy();
        }
    }
}
