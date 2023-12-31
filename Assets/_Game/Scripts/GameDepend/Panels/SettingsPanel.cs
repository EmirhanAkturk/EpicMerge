using System.Collections.Generic;
using _Game.Scripts.GameDepend.Panels.Tabs;
using _Game.Scripts.Systems.TileNodeSystem.GraphGenerator;
using GameDepends.Enums;
using Systems.PanelSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace _Game.Scripts.GameDepend.Panels
{
    public class SettingsPanelData : PanelData
    {
        public readonly List<TileGraphGenerator> graphGenerators;

        public SettingsPanelData(List<TileGraphGenerator> generators)
        {
            graphGenerators = generators;
        }
    }
    
    public class SettingsPanel : BasePanel
    {
        protected override PopupType PopupType => PopupType.SettingPanel;

        [FormerlySerializedAs("recreateGraphButtonParent")] [SerializeField] private RecreateGraphButtonsParent recreateGraphButtonsParent;

        public override void ShowPanel(PanelData data)
        {
            base.ShowPanel(data);

            var panelData = data as SettingsPanelData;
            LoadButtons(panelData.graphGenerators);
        }

        private void LoadButtons(IReadOnlyList<TileGraphGenerator> graphGenerators)
        {
            recreateGraphButtonsParent.LoadButtons(graphGenerators);
        }
    }
}
