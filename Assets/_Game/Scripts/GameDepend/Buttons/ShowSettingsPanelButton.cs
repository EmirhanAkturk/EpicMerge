using _Game.Scripts.GameDepend;
using _Game.Scripts.Systems.TileNodeSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem;
using GameDepends._Game.Scripts.GameDepend.Panels;
using Systems.PanelSystem;
namespace GameDepends._Game.Scripts.GameDepend
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
