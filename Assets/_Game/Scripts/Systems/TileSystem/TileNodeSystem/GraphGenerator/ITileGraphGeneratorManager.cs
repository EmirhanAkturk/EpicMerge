using System.Collections.Generic;

namespace _Game.Scripts.Systems.TileSystem.TileNodeSystem.GraphGenerator
{
    public interface ITileGraphGeneratorManager
    {
        void AddGenerator(TileGraphGenerator generator);
        bool RemoveGenerator(TileGraphGenerator generator);
        void RecreateGraph(int graphGeneratorId);
        void RecreateAllGraphs();
        List<TileGraphGenerator> GetAllGraphGenerators();
        IEnumerable<int> GetAllGeneratorIds();
    }
}
