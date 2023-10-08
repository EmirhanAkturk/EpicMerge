using _Game.Scripts.Systems.TileObjectSystem;
using JoostenProductions;
using UnityEngine;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeController : OverridableMonoBehaviour
    {
        [Space]
        
        [SerializeField] private TileNodeObjectController tileNodeObjectController;

        #region Init Functions

        public void Init(TileNode tileNode, BaseTileObject baseTileObject)
        {
            tileNodeObjectController.Init(tileNode, baseTileObject);
        }

        #endregion
     
        #region Tests

        /*private TileNode TileNode => tileNodeObjectController.ThisTileNode;

        [Button(nameof(BfsTest))] public bool bfsButtonField;
        [Button(nameof(PrintNeighbors))] public bool printNeighborsButtonField;

        public void BfsTest()
        {
            TileNode.Bfs();
        }
        
        public void PrintNeighbors()
        {
            TileNode.PrintNeighbors();
        }*/
        #endregion
    }
}
