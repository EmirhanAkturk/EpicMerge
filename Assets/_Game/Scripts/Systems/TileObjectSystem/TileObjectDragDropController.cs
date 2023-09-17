using System;
using GameDepends;
using JoostenProductions;
using Systems.ConfigurationSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    [RequireComponent(typeof(Collider))]
    public sealed class TileObjectDragDropController : OverridableMonoBehaviour
    {
        public Action onObjectDragStart;
        public Action onObjectDragEnd;

        public bool CanDrag
        {
            get => canDrag;
            set
            {
                canDrag = value;
                TryResetDragPos();
            }
        }

        private bool canDrag;

        [SerializeField] private float inputMouseLimitPercentage = .1f;

        private Camera MainCam
        {
            get
            {
                if (mainCam == null || mainCam.Equals(null))
                {
                    mainCam = Camera.main;
                }
                return mainCam;
            }    
        }
        private Camera mainCam;
        
        // TODO Move controller can only be known by TileObjectController
        private IMoveController MoveController => moveController ??= GetComponent<IMoveController>();
        private IMoveController moveController;

        private Transform draggingObjectTr;
        private LayerMask groundLayerMask;

        private Vector3 targetDragPosition;
        
        #region Initialize Functions

        private void Awake()
        {
            groundLayerMask = ConfigurationService.Configurations.dragDropLayerMask;
            draggingObjectTr = transform;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CanDrag = true;
        }

        #endregion

        #region Mouse Functions

        private Vector3 hitPoint;

        private void TryResetDragPos()
        {
            if (!CanDrag)
            {
                // TODO Check mouse clicking
                targetDragPosition = draggingObjectTr.position;
                hitPoint = Input.mousePosition;
                // Debug.Log("hitPoint : " + hitPoint);
            }
        }

        public void OnMouseDown()
        {
            // TODO Add AnyPanelShowing
            hitPoint = Input.mousePosition;
            if (CanDrag) 
            {
                onObjectDragStart?.Invoke();
                SetDraggingObjectState(true);
            }
        }

        public void OnMouseDrag()
        {
            if (!CanDrag || !IsMouseDragEnoughForMovement()) return;

            UpdateTargetDragPosition();
            MoveController.Move(targetDragPosition);
        }

        public void OnMouseUp()
        {
            onObjectDragEnd?.Invoke();
            SetDraggingObjectState(false);
            targetDragPosition = transform.position;
        }

        #endregion

        #region Other Functions
        
        private bool IsMouseDragEnoughForMovement()
        {
            return Vector3.Distance(Input.mousePosition, hitPoint) >= Screen.height * inputMouseLimitPercentage;
        }
        
        private void UpdateTargetDragPosition()
        {
            Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, groundLayerMask))
                targetDragPosition = raycastHit.point;
        }
        
        private void SetDraggingObjectState(bool isDragging)
        {
            if (isDragging)
            {
                // gameObject.layer = 7;
            }
            else
            {
                // gameObject.layer = 0;
            }
        }

        #endregion
    }
}