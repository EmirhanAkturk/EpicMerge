using System;
using System.Collections.Generic;
using Systems.ConfigurationSystem;
using Utils;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class TileObjectManager : Singleton<TileObjectManager>
    {
        // Test Part
        public int MergeableObjectTypeCount
        {
            get
            {
                return ConfigurationService.Configurations.mergeableObjectTypeCount;
            }
            set
            {
                ConfigurationService.Configurations.mergeableObjectTypeCount = value;
            }
        }
        
        private Dictionary<int, TileObjectCollectionData> TileObjectCollectionDataMap
        {
            get
            {
                if (tileObjectCollectionDataMap is null)
                {
                    Load();
                }
                return tileObjectCollectionDataMap;
            }
        }
        private Dictionary<int, TileObjectCollectionData> tileObjectCollectionDataMap;
        
        private TileObjectCollection tileObjectCollection;
        private bool isLoaded;
        
        private void Awake()
        {
            Load();
        }

        #region Load

        private void Load()
        {
            if (isLoaded) return;
            tileObjectCollection ??= TileObjectCollection.Create();
            
            LoadCollectionDatas();
            
            MergeableObjectTypeCount = ConfigurationService.Configurations.mergeableObjectTypeCount;
            // Load save data 
            isLoaded = true;
        }

        private void LoadCollectionDatas()
        {
            var tileCollectionDatas = tileObjectCollection.GetAllTileCollectionDatas();
            LoadDictionary(tileCollectionDatas);
        }

        private void LoadDictionary(List<TileObjectCollectionData> tileObjectCollectionDatas)
        {
            tileObjectCollectionDataMap ??= new Dictionary<int, TileObjectCollectionData>();
            tileObjectCollectionDataMap.Clear();

            foreach (var tileObjectData in tileObjectCollectionDatas)
            {
                if (tileObjectCollectionDataMap.ContainsKey(tileObjectData.objectId))
                {
                    LogUtility.PrintColoredError($"Object Id : {tileObjectData.objectId} is already in dictionary!!");
                    continue;
                }

                tileObjectCollectionDataMap[tileObjectData.objectId] = tileObjectData;
            }
        }
        
        #endregion

        #region Get Functions
        
        public TileObjectCollectionData GetObjectDataById(int objectId)
        {
            return TileObjectCollectionDataMap.TryGetValue(objectId, out var collectionData)
                ? collectionData
                : null;
        }

        public List<TileObjectDataByLevel> GetLevelDatasById(int objectId)
        {
            var collectionData = GetObjectDataById(objectId);
            return collectionData?.tileObjectDataByLevels;
        }
        
        public TileObjectDataByLevel? GetLevelData(int objectId, int level)
        {
            var collectionData = GetObjectDataById(objectId);
            return collectionData?.GetDataByLevel(level);
        }
        
        #endregion
    }
}
