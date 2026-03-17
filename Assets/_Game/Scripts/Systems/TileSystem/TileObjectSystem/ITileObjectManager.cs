using System.Collections.Generic;

namespace _Game.Scripts.Systems.TileSystem.TileObjectSystem
{
    public interface ITileObjectManager
    {
        TileObjectCollectionData GetObjectDataById(int objectId);
        List<TileObjectDataByLevel> GetLevelDatasById(int objectId);
        TileObjectDataByLevel? GetLevelData(int objectId, int level);
    }
}
