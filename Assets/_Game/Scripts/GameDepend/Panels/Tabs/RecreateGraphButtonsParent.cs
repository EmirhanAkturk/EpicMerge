using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem;
using UnityEngine;

namespace _Game.Scripts.GameDepend
{
    public class RecreateGraphButtonsParent : MonoBehaviour
    {
        [SerializeField] private List<RecreateGraphButton> recreateGraphButtons;

        public void LoadButtons(IReadOnlyList<TileGraphGenerator> graphGenerators)
        {
            for (int i = 0; i < recreateGraphButtons.Count; i++)
            {
                var currentButton = recreateGraphButtons[i];
                bool hasGenerator = graphGenerators.Count > i;
                if (hasGenerator)
                {
                    var generator = graphGenerators[i];
                    currentButton.Init(generator.GraphGraphGeneratorId, generator.GraphGeneratorName, RecreateGraph);
                }
                else
                {
                    currentButton.Hide();
                }
            }
        }
        
        private void RecreateGraph(int generatorId)
        {
            TileGraphGeneratorManager.Instance.RecreateGraph(generatorId);
        }
    }
}
