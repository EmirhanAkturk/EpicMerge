using System;
using _Game.Scripts.Systems.TileObjectSystem;
using GameDepends;
using JoostenProductions;
using NaughtyAttributes;
using Systems.PanelSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.CameraSystem
{
    public class CameraController : OverridableMonoBehaviour
    {
        // This script created for Game Mechanic test, can be refactored
        
        [Space]
        [SerializeField] private Transform cameraTR;
        [Space]
        [SerializeField] private float zoomSpeed = 1.0f;
        [SerializeField] private float minZoom = 3.0f;
        [SerializeField] private float maxZoom = 10.0f;

        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float moveRangeRadius = 50f;

        private const string STR_WHEEL_AXIS = "Mouse ScrollWheel";

        private Vector3 startPosition;
        private Vector3 lastMousePosition;
        private Vector2 initialTouch1;
        private Vector2 initialTouch2;
        private float initialDistance;
        private float initialZoom;
        private float adaptiveMoveSpeed;
        private float adaptiveZoomSpeed;

        private bool isCameraControllable = true;

        #region Game Part
        protected override void OnEnable()
        {
            base.OnEnable();

            EventService.onTileObjectDragStart += ObjectDragStart;
            EventService.onTileObjectDragEnd += ObjectDragEnd;
        }
    
        protected override void OnDisable()
        {
            base.OnDisable();

            EventService.onTileObjectDragStart -= ObjectDragStart;
            EventService.onTileObjectDragEnd -= ObjectDragEnd;
        }

        private void ObjectDragStart(BaseTileObject obj)
        {
            isCameraControllable = false;
        }
        
        private void ObjectDragEnd(BaseTileObject obj)
        {
            isCameraControllable = true;
        }
        #endregion

        private void Awake()
        {
            startPosition = cameraTR.position;
            ConvertToAdaptiveSpeed();
        }

        private void ConvertToAdaptiveSpeed()
        {
            float adaptiveMultiplier = ((float)1920 / 1080) * Screen.height / Screen.width; 
             
            adaptiveMoveSpeed = moveSpeed * adaptiveMultiplier;
            adaptiveZoomSpeed = zoomSpeed * adaptiveMultiplier;
        }

        public override void UpdateMe()
        {
            if (!isCameraControllable) return;

            CameraMove();
            CameraZoom();
        }

        private void CameraMove()
        {
            // Check Zoom in / out
            if(Input.touchCount > 1 || PanelManager.Instance.IsAnyPanelShowing()) return;
                
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 deltaMousePosition = lastMousePosition - Input.mousePosition;
                Vector3 deltaPos = new Vector3(deltaMousePosition.x, deltaMousePosition.y, 0);
                Vector3 moveVector = adaptiveMoveSpeed * Time.deltaTime * deltaPos;
                Vector3 newPosition = cameraTR.position + moveVector;
                
                
                if(CanCameraMove(newPosition))
                {
                    cameraTR.Translate(moveVector);
                }
                
                lastMousePosition = Input.mousePosition;
            }
        }

        private bool CanCameraMove(Vector3 targetPos)
        {
            float newDistance = Vector3.Distance(targetPos, startPosition);
            if (newDistance <= moveRangeRadius) return true;
            float currentDistance = Vector3.Distance(cameraTR.position, startPosition);
            return newDistance < currentDistance;
        }
        
        private void CameraZoom()
        {
#if UNITY_EDITOR
            CameraZoomWheel();
#else
            CameraZoomTouch();
#endif
        }

        private void CameraZoomWheel()
        {
            float scrollWheelInput = Input.GetAxis(STR_WHEEL_AXIS);

            if (scrollWheelInput != 0)
            {
                float newZoom = Camera.main.orthographicSize - scrollWheelInput * adaptiveZoomSpeed;
                newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

                Camera.main.orthographicSize = newZoom;
            }
        }

        private void CameraZoomTouch()
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    initialTouch1 = touch1.position;
                    initialTouch2 = touch2.position;
                    initialDistance = Vector2.Distance(initialTouch1, initialTouch2);
                    initialZoom = Camera.main.orthographicSize;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouch1 = touch1.position;
                    Vector2 currentTouch2 = touch2.position;

                    float currentDistance = Vector2.Distance(currentTouch1, currentTouch2);
                    float pinchDelta = currentDistance - initialDistance;

                    float newZoom = Mathf.Clamp(initialZoom - pinchDelta * adaptiveZoomSpeed, minZoom, maxZoom);
                    Camera.main.orthographicSize = newZoom;
                }
            }
        }
    }
}
