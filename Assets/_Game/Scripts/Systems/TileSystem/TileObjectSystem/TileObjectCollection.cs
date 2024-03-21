using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    [CreateAssetMenu(menuName = "lib / Tile Object Collection", fileName = "TileObjectCollection")]
    public class TileObjectCollection : ScriptableObject
    {
        [SerializeField] private List<TileObjectCollectionData> tileObjectCollectionDatas;
        
        private const string COLLECTION_PATH = "Configurations/TileObjectCollection";

        public static TileObjectCollection Create()
        {
            var tileObjectCollection = Resources.Load<TileObjectCollection>(COLLECTION_PATH);
            return tileObjectCollection;
        }

        public List<TileObjectCollectionData> GetAllTileCollectionDatas()
        {
            return tileObjectCollectionDatas;
        }
    }
}
