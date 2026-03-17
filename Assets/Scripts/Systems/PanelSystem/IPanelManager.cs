using GameDepends;

namespace Systems.PanelSystem
{
    public interface IPanelManager
    {
        PanelEvent onPanelShowed { get; }
        PanelEvent onPanelHidden { get; }

        void Show(PopupType type, PanelData data);
        void Hide(PopupType type);
        void Hide(BasePanel panel);
        bool IsAnyPanelShowing();
        bool IsPanelShowed(PopupType popupType, bool checkStillShowing = true);
        bool IsAnyPanelShowed(params PopupType[] exepts);
    }
}
