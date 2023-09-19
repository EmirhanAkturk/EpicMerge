using GameDepends.Enums;
using Systems.PanelSystem;

namespace _Game.Scripts.GameDepend.Panels
{
    public class GameplayPanel : BasePanel
    {
        protected override PopupType PopupType => PopupType.GamePlayPanel;

        public override void ShowPanel(PanelData data)
        {
            base.ShowPanel(data);
        }

        public override void HidePanel()
        {
            base.HidePanel();
        }
    }
}
