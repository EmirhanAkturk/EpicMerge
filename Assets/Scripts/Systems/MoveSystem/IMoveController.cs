using UnityEngine;

namespace Systems.MoveSystem
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
