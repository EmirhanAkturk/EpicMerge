using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using JoostenProductions;
using UnityEngine;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeController : OverridableMonoBehaviour
    {
        public TileNode TileNode => tileNodeObjectController.ThisTileNode;
        
        [Space]
        
        [SerializeField] private TileNodeObjectController tileNodeObjectController;
        // [SerializeField] private TileNode tileNode;

        #region Init Functions

        public void Init(TileNode tileNode, BaseTileObject baseTileObject)
        {
            tileNodeObjectController.Init(tileNode, baseTileObject);
        }

        #endregion

     
        #region Subscribe & Unsubscribe Events
        

        #endregion
    }
}
