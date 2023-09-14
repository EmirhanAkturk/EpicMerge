using System;
using _Game.Scripts.Systems.TileObjectSystem;

namespace GameDepends
{
    public class EventService
    {
        public static Action<TileObject> onTileObjectMoveStart;
        public static Action<TileObject> onTileObjectMoveEnd;
    }
}
