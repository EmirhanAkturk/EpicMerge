using System.Collections.Generic;
using System.Linq;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileGraphGeneratorManager : Singleton<TileGraphGeneratorManager>
    {
        private readonly Dictionary<int, TileGraphGenerator> tileGraphGenerators = new Dictionary<int, TileGraphGenerator>();

        #region Add & Remove

        public void AddGenerator(TileGraphGenerator tileGraphGenerator)
        {
            if (tileGraphGenerators.ContainsKey(tileGraphGenerator.GraphGraphGeneratorId))
            {
                LogUtility.PrintColoredError("Graph Generator Id :" + tileGraphGenerator.GraphGraphGeneratorId + "already in dictionary!!");
                return;
            }
            
            tileGraphGenerators.Add(tileGraphGenerator.GraphGraphGeneratorId, tileGraphGenerator);
        }
        
        public bool RemoveGenerator(TileGraphGenerator tileGraphGenerator)
        {
            bool isRemoved = tileGraphGenerators.Remove(tileGraphGenerator.GraphGraphGeneratorId); 
            return isRemoved;
        }
        
        #endregion

        #region RecreateGraphs

        public void RecreateGraph(int graphGeneratorId)
        {
            var graphGenerator = GetGraphGeneratorById(graphGeneratorId);
            if (graphGenerator is null)
            {
                LogUtility.PrintColoredError(" Graph Generator Is NULL!! Wanted graph generator id :" +  graphGeneratorId);
                return;
            }
            
            graphGenerator.RecreateGraph();
        }

        public void RecreateAllGraphs()
        {
            var allGraphGenerators = GetAllGraphGenerators();
            foreach (var graphGenerator in allGraphGenerators)
            {
                graphGenerator.RecreateGraph();
            }
        }

        #endregion
       
        #region Get Funcitons

        public List<TileGraphGenerator> GetAllGraphGenerators()
        {
            return tileGraphGenerators.Values.ToList();
        }

        public IEnumerable<int> GetAllGeneratorIds()
        {
            var allGraphGenerators = GetAllGraphGenerators(); 
            return allGraphGenerators.Select(tileGraphGenerator => tileGraphGenerator.GraphGraphGeneratorId);
        }

        private TileGraphGenerator GetGraphGeneratorById(int graphGeneratorId)
        {
            return tileGraphGenerators.TryGetValue(graphGeneratorId, out var tileGraphGenerator)
                ? tileGraphGenerator
                : null;
        }

        #endregion

    }
}
