using _Game.Scripts.Systems.TileNodeSystem;
using GameDepends;
using Systems.PanelSystem;

namespace _Game.Scripts.GameDepend
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
