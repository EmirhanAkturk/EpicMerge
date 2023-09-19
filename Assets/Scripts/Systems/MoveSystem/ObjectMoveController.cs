using JoostenProductions;
using NaughtyAttributes;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem.MoveSystem
{
    public class ObjectMoveController : OverridableMonoBehaviour, IMoveController
    {
        [Space]
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private float moveSpeed = 1f;

        [Space] 
        [SerializeField] private bool hasLongDistanceSpeed;
        [ShowIf("hasLongDistanceSpeed")] [SerializeField] private float longMoveDistance = 10f;
        [ShowIf("hasLongDistanceSpeed")] [SerializeField] private float longMoveMoveSpeedMultiplier = 2f;
        
        private Transform targetTransform;
        private MoveEndCallback moveEndCallback;
        private Vector3 currentTargetPos;
        private bool isMoving;

        private void Awake()
        {
            targetTransform = transform;
        }
        
        public override void UpdateMe()
        {
            if (!isMoving) return;
            
            UpdatePosition();
            CheckTargetReached();
        }

        public void Move(Vector3 targetPos, MoveEndCallback onMoveEnd)
        {
            Stop();
            currentTargetPos = targetPos;
            moveEndCallback = onMoveEnd;

            if (IsTargetReached()) // destination to close
            {
                Stop();
            }
            else
            {
                isMoving = true;
            }
        }

        public void Stop()
        {
            moveEndCallback?.Invoke(IsTargetReached());
            moveEndCallback = null;
            isMoving = false;
        }

        public void SetSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public float GetSpeed()
        {
            float speed = moveSpeed;
 
            if (!hasLongDistanceSpeed) return speed;
            
            float distance = GetDistance();
            if (distance > longMoveDistance)
                speed *= longMoveMoveSpeedMultiplier;
            
            // Debug.Log("Distance : " + distance + ", speed : " + speed);
            return speed;
        }

        public bool IsFastMoving()
        {
            var speed = GetSpeed();
            return speed > moveSpeed;
        }

        private void UpdatePosition()
        {
            targetTransform.position = Vector3.MoveTowards(targetTransform.position, currentTargetPos, GetSpeed() * Time.deltaTime);
        }

        private void CheckTargetReached()
        {
            if (IsTargetReached())
            {
                Stop();
            }
        }

        private bool IsTargetReached()
        {
            return GetDistance() < stoppingDistance;
        }

        private float GetDistance()
        {
            return Vector3.Distance(targetTransform.position, currentTargetPos);
        }
    }
}
