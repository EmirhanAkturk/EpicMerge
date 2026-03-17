using _Game.Scripts.Systems.TileSystem.TileNodeSystem.GraphGenerator;
using GameDepends;
using Systems.PanelSystem;
using UnityEngine;
using Utils;
using Zenject;

namespace GameDepends._Game.Scripts.GameDepend
{
    public class GameStarter : MonoBehaviour
    {
        [Inject] private IPanelManager PanelManager { get; }
        [Inject] private ITileGraphGeneratorManager GraphGeneratorManager { get; }

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            ShowGameplayPanel();
            CreateGraphsWithDelay(.1f);
        }

        private void ShowGameplayPanel()
        {
            PanelManager.Show(PopupType.GamePlayPanel, new PanelData());
        }

        private void CreateGraphsWithDelay(float delay)
        {
            CoroutineDispatcher.ExecuteWithDelay(delay, GraphGeneratorManager.RecreateAllGraphs);
        }
    }
}
