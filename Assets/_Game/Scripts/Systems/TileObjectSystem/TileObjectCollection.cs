using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    [CreateAssetMenu(menuName = "lib / Tile Object Collection", fileName = "TileObjectCollection")]
    public class TileObjectCollection : ScriptableObject
    {
        [SerializeField] private List<TileObjectCollectionData> tileObjectCollectionDatas;
        
        [NonSerialized] private Dictionary<int, TileObjectCollectionData> tileObjectCollectionDataMap = new Dictionary<int, TileObjectCollectionData>();
        [NonSerialized] private bool isLoaded;

        public void LoadCollectionDatas()
        {
            
        }
    }
}
