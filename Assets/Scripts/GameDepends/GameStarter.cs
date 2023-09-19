using System.Collections;
using _Game.Scripts.Systems.TileNodeSystem.GraphGenerator;
using GameDepends.Enums;
using Systems.PanelSystem;
using UnityEngine;
using Utils;

namespace GameDepends
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
