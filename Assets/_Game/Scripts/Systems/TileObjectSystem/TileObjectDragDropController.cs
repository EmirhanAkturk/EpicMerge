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
        
        public bool CanDrag { get; set; }

        [SerializeField] private float moveSpeed = 3f;
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

        public override void UpdateMe()
        {
            if (CanDrag)
            {
                draggingObjectTr.position = Vector3.MoveTowards(draggingObjectTr.transform.position, targetDragPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                targetDragPosition = draggingObjectTr.position;
                hitPoint = Input.mousePosition;
                Debug.Log("hitPoint : " + hitPoint);
            }
            
            // Debug.Log("targetDragPosition : " + targetDragPosition + ", CanDrag : " + CanDrag);
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
            if (Vector3.Distance(Input.mousePosition, hitPoint) < Screen.height * inputMouseLimitPercentage) return;

            if (CanDrag)
                UpdateDraggingObjectPosition();
        }

        public void OnMouseUp()
        {
            onObjectDragEnd?.Invoke();
            SetDraggingObjectState(false);
            targetDragPosition = transform.position;
        }

        #endregion

        #region Other Functions
        
        private void UpdateDraggingObjectPosition()
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