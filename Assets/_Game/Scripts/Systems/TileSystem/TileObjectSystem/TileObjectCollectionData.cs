using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    [Serializable]
    public class TileObjectCollectionData // TODO Can Made Struct
    {
        public int objectId;
        public List<TileObjectDataByLevel> tileObjectDataByLevels;

        private Dictionary<int, TileObjectDataByLevel> TileObjectLevelDataMap
        {
            get
            {
                if (tileObjectLevelDataMap is null)
                {
                    InitLevelDataMap();
                }

                return tileObjectLevelDataMap;
            }
        }
        [NonSerialized] private Dictionary<int, TileObjectDataByLevel> tileObjectLevelDataMap;

        private void InitLevelDataMap()
        {
            tileObjectLevelDataMap = new Dictionary<int, TileObjectDataByLevel>();
            foreach (var dataByLevel in tileObjectDataByLevels)
            {
                if (tileObjectLevelDataMap.ContainsKey(dataByLevel.level))
                {
                    LogUtility.PrintColoredError($"Level : {dataByLevel.level} is already in dictionary!!");
                    continue;
                }

                tileObjectLevelDataMap[dataByLevel.level] = dataByLevel;
            }
        }

        public TileObjectDataByLevel? GetDataByLevel(int level)
        {
            return TileObjectLevelDataMap.TryGetValue(level, out var dataByLevel) ? dataByLevel : null;
        }
    }
}
