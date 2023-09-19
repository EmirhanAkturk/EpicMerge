using _Game.Scripts.GameDepend.Panels;
using _Game.Scripts.Systems.TileNodeSystem.GraphGenerator;
using GameDepends.Enums;
using Systems.PanelSystem;

namespace _Game.Scripts.GameDepend.Buttons
{
    public class ShowSettingsPanelButton : BasePanelButton
    {
        protected override void OpenPanel()
        {
            var graphGenerators = TileGraphGeneratorManager.Instance.GetAllGraphGenerators();
            PanelManager.Instance.Show(PopupType.SettingPanel, new SettingsPanelData(graphGenerators));
        }
    }
}
