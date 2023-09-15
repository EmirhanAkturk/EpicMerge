using System;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public delegate void MoveEndCallback(bool isReachedTarget); // declare delegate type

    public interface IMoveController
    {
        void Move(Vector3 targetPos, MoveEndCallback onMoveEnd = null);
        void Stop();
        void SetSpeed(float speed);
        float GetSpeed();
    }
}
