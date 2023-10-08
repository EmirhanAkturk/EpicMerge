using GameDepends;
using Systems.PanelSystem;

namespace _Game.Scripts.GameDepend
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
