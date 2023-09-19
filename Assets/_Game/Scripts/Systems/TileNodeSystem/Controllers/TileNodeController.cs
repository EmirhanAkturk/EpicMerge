using System;
using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using Attribute;
using JoostenProductions;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeController : OverridableMonoBehaviour
    {
        public TileNode TileNode => tileNodeObjectController.ThisTileNode;
        
        [Button(nameof(BfsTest))] public bool bfsButtonField;
        [Button(nameof(PrintNeighbors))] public bool printNeighborsButtonField;
        
        [Space]
        
        [SerializeField] private TileNodeObjectController tileNodeObjectController;
        // [SerializeField] private TileNode tileNode;

        #region Init Functions

        public void Init(TileNode tileNode, BaseTileObject baseTileObject)
        {
            tileNodeObjectController.Init(tileNode, baseTileObject);
        }

        #endregion

     
        #region Tests

        public void BfsTest()
        {
            TileNode.Bfs();
        }
        
        public void PrintNeighbors()
        {
            TileNode.PrintNeighbors();
        }
        #endregion
    }
}
