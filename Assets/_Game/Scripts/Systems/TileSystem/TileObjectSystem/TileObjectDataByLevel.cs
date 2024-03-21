using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    [Serializable]
    public struct TileObjectDataByLevel
    {
        public int level;
        public Sprite sprite;
    }
}
