using _Game.Scripts.Systems.TileSystem.TileNodeSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.GraphGenerator;
using Systems.PanelSystem;
using UnityEngine;
using Utils;
namespace GameDepends._Game.Scripts.GameDepend
{
    public class GameStarter : MonoBehaviour
    {
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
            PanelManager.Instance.Show(PopupType.GamePlayPanel, new PanelData());
        }

        private void CreateGraphsWithDelay(float delay)
        {
            CoroutineDispatcher.ExecuteWithDelay(delay, TileGraphGeneratorManager.Instance.RecreateAllGraphs);
        }
    }
}
