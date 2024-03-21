using System.Collections.Generic;

namespace GameDepends
{
    public enum PopupType 
    {
        GamePlayPanel = 1,
        SettingPanel = 2,
    }

    public static class PopupTypeExtensions
    {
        private static readonly Dictionary<PopupType, string> PopupPrefabs = new Dictionary<PopupType, string>()
        {
            {PopupType.GamePlayPanel, "GamePlayPanel"},
            {PopupType.SettingPanel, "SettingsPanel"},
        };
    
        public static string GetPopupPrefabName(this PopupType popupType)
        {
            return PopupPrefabs[popupType];
        }
    }
}