using _Game.Scripts.Systems.DetectionSystem;
using _Game.Scripts.Systems.IndicationSystem;
using _Game.Scripts.Systems.IndicatorSystem;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Utils;
using GameDepends;
using JoostenProductions;
using NaughtyAttributes;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeObjectController : OverridableMonoBehaviour
    {
        public TileNode ThisTileNode { get; private set; }

        [Space]
        
        [SerializeField] private TileNodeObjectDetectionHandler tileNodeObjectDetectionHandler;
        // [SerializeField] private MergeableTileIndicator mergeableTileIndicator;
        [SerializeField] private bool hasMergeableTileIndicator;
        [ShowIf("hasMergeableTileIndicator")]
        [SerializeField] private BaseIndicatorController mergeableTileIndicatorController;
        
        private BaseTileObject placedBaseTileObject;
        private BaseTileObject movingBaseTileObjectOnThisBaseTile;
        private Vector3? centerPoint;

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeAllEvents();
        }

        public void Init(TileNode tileNode, BaseTileObject initObject)
        {
            ThisTileNode = tileNode;
            // tileNode.Init(GetTileObjectValue(initObject));
            ResetVariables();
            SubscribeInitEvents();
            InitPlacedObject(initObject);
        }

        private void InitPlacedObject(BaseTileObject initObject)
        {
            if(initObject == null) return;
            movingBaseTileObjectOnThisBaseTile = initObject;
            TryPlaceObjectInTile(initObject);
        }
        
        private void ResetVariables()
        {
            movingBaseTileObjectOnThisBaseTile = null;
            placedBaseTileObject = null;
            
            UnsubscribeAllEvents();
        }

        private void ObjectPlacedToTile(TileNode newTileNode, BaseTileObject baseTileObject)
        {
            if(placedBaseTileObject == null || placedBaseTileObject != baseTileObject) return;
            if (newTileNode != ThisTileNode)
            {
                // The object previously placed in this tile is now placed in another tile
                SetPlacedObject(null);
            }
        }

        private void AfterObjectDragEnd(BaseTileObject baseTileObject)
        {
            if(baseTileObject != placedBaseTileObject) return;
            // Debug.Log("ObjectDragEnd : " + gameObject.name);
            MoveObjectToTileCenter(baseTileObject);
        }

        private void ObjectEnterTileArea(BaseTileObject baseTileObject)
        {
            //TODO Refactor below part!!
            // EventService.onTileObjectEnteredToNode?.Invoke(tileObject, this);
            
            // TODO Due to the CanDrag control, the object may not be centered in the slot when the drag ends
            if (!baseTileObject.CanObjectCentered())
            {
                Debug.Log("### tileObject.CanDrag return : " + gameObject.name);
                return;
            }
            
            Debug.Log("### tileObject.CanDrag not return : " + gameObject.name);

            
            movingBaseTileObjectOnThisBaseTile = baseTileObject;
            MoveObjectToTileCenter(baseTileObject);

            if (placedBaseTileObject != null && placedBaseTileObject != baseTileObject)
            {
                bool canMerge = CanMerge(baseTileObject, true);
                Debug.Log("canMerge : " + canMerge);
            }
            else
            {
                EventService.onMergeCanceled?.Invoke();
            }
        }

        private void ObjectExitTileArea(BaseTileObject baseTileObject)
        {
            movingBaseTileObjectOnThisBaseTile = null;
        }

        private void TileObjectPlaced(BaseTileObject baseTileObject)
        {
            if (placedBaseTileObject is null)
            {
                // Debug.Log("###TryPlaceObjectInTile");
                TryPlaceObjectInTile(baseTileObject);
                return;
            }

            bool isMerged = false;
            if (CanTryMerge(baseTileObject))
            {
                // Debug.Log("###TryPlaceObjectInTile");
                isMerged = TryMerge(baseTileObject);
            }
            if (!isMerged)
            {
                var targetTileNode = ThisTileNode.GetEmptyNeighbor() ?? baseTileObject.TileNode;
                targetTileNode.onTileObjectChanged?.Invoke(placedBaseTileObject);
                
                PlaceObjectOnTile(baseTileObject);
                Debug.Log("###isMerged False" + gameObject.name);
            }
            else
            {
                Debug.Log("###isMerged true : " + gameObject.name);
            }
        }
        
        private void TryPlaceObjectInTile(BaseTileObject baseTileObject)
        {
            Debug.Log("TryPlaceObjectIn : " + gameObject.name);
            if(baseTileObject != movingBaseTileObjectOnThisBaseTile) return;

            if (placedBaseTileObject == baseTileObject)
            {
                // Already on this tile, just move to the center
                MoveObjectToTileCenter(baseTileObject);
            }
            else
            {
                // Place object on the this slot
                PlaceObjectOnTile(baseTileObject);
            }
        }

        private void PlaceObjectOnTile(BaseTileObject baseTileObject)
        {
            MoveObjectToTileCenter(baseTileObject);
            movingBaseTileObjectOnThisBaseTile = null;
            SetPlacedObject(baseTileObject);
            EventService.onTileObjectPlacedToTile?.Invoke(ThisTileNode, baseTileObject);
        }

        private void SetPlacedObject(BaseTileObject baseTileObject)
        {
            placedBaseTileObject = baseTileObject;
            if (placedBaseTileObject != null) baseTileObject.TileNode = ThisTileNode; 
            CheckTileObjectEventSubState();
            // onPlacedTileObjectChanged?.Invoke(tileObject);
            PlacedTileObjectChanged(baseTileObject);
            Debug.Log("Set placedTileObject" + (placedBaseTileObject is null ? "NULL" : "tile object") + ", Node Name : " + gameObject.name);
        }

        private void MoveObjectToTileCenter(BaseTileObject baseTileObject)
        {
            Vector3 targetPos = GetCenterPoint();
            baseTileObject.MoveToTargetNode(targetPos);
        }

        private void PlacedTileObjectChanged(BaseTileObject baseTileObject)
        {
            UpdateTileNodeValue(GetTileObjectValue(baseTileObject));
        }

        private void CheckTileObjectEventSubState()
        {
            if (placedBaseTileObject != null)
                SubscribeTileObjectEvents();
            else
                UnsubscribeTileObjectEvents();
        }

        private Vector3 GetCenterPoint()
        {
            centerPoint ??= transform.position;
            return centerPoint.Value;
        }
        
        private void UpdateMergeableIndicator(bool isMergeable)
        {
            if (hasMergeableTileIndicator)
            {
                mergeableTileIndicatorController.UpdateIndicatorState(isMergeable);
            }
            if (placedBaseTileObject != null)
            {
                if (placedBaseTileObject is IMergeableTileObject mergeableTileObject)
                {
                    mergeableTileObject.UpdateMergeableIndicatorState(isMergeable);
                }
                else
                {
                    LogUtility.PrintColoredError("Placed Tile Object is not a TileObject!!");    
                }
            }
        }

        #region Merge Functions

        private bool CanTryMerge(BaseTileObject baseTileObject)
        {
            return placedBaseTileObject != baseTileObject && placedBaseTileObject.TileObjectValue.Equals(baseTileObject.TileObjectValue);
        }
        
        private bool CanMerge(BaseTileObject baseTileObject, bool indicateMergeableObjects)
        {
            return baseTileObject != null && TileObjectMergeHelper.CanMerge(baseTileObject.TileNode, ThisTileNode, baseTileObject.TileObjectValue, indicateMergeableObjects);
        }

        private bool TryMerge(BaseTileObject baseTileObject)
        {
            return TileObjectMergeHelper.TryMerge(baseTileObject.TileNode,ThisTileNode, baseTileObject.TileObjectValue);
        }

        private void UpdateMergedTileObjectValue(TileObjectValue tileObjectValue)
        {
            movingBaseTileObjectOnThisBaseTile = null;

            if (tileObjectValue.IsEmptyTileObjectValue())
            {
                Destroy(placedBaseTileObject.gameObject);
                placedBaseTileObject = null;
            }
            else if (placedBaseTileObject != null)
            {
                placedBaseTileObject.Init(tileObjectValue);
            }
            
            // UnsubscribeAllObjectEvents();
            // InitPlacedObject(placedTileObject);
        }

        private void TileObjectMerged(TileObjectValue newValue)
        {
            UpdateTileNodeValue(newValue);
            UpdateMergedTileObjectValue(newValue);
        }

        #endregion
        
        #region Node Functions
        
        private void UpdateTileNodeValue(TileObjectValue tileObjectValue)
        {
            ThisTileNode.SetValue(tileObjectValue);
        }
        
        private TileObjectValue GetTileObjectValue(BaseTileObject baseTileObject)
        {
            return baseTileObject != null ? baseTileObject.TileObjectValue : TileObjectValue.GetEmptyTileObjectValue();
        }
        
        #endregion
        
        #region Subscribe Unsubscribe Events

        private void UnsubscribeAllEvents()
        {
            UnsubscribeAllObjectEvents();
            UnSubscribeInitEvents();
        }        
        
        private void UnsubscribeAllObjectEvents()
        {
            UnsubscribeTileObjectEvents();
        }
        
        private bool isSubTileObjectEvents;
        private void SubscribeTileObjectEvents()
        {
            if(isSubTileObjectEvents) return;
            EventService.onAfterTileObjectDragEnd += AfterObjectDragEnd;
            EventService.onTileObjectPlacedToTile += ObjectPlacedToTile;
            isSubTileObjectEvents = true;
        }

        private void UnsubscribeTileObjectEvents()
        {
            // TODO unsubscribe can only be done in OnDisable 
            if(!isSubTileObjectEvents) return;
            EventService.onAfterTileObjectDragEnd -= AfterObjectDragEnd;
            EventService.onTileObjectPlacedToTile -= ObjectPlacedToTile;
            isSubTileObjectEvents = false;
        }

        private bool isSubscribedInitEvents;
        private void SubscribeInitEvents()
        {
            if(isSubscribedInitEvents) return;
            SubscribeTileNodeEvents();
            SubscribeTileNodeDetectorEvents();
            isSubscribedInitEvents = true;
        }

        private void UnSubscribeInitEvents()
        {
            if(!isSubscribedInitEvents) return;
            UnsubscribeTileNodeEvents();
            UnsubscribeTileNodeDetectorEvents();
            isSubscribedInitEvents = false;
        }

        private void SubscribeTileNodeEvents()
        {
            ThisTileNode.onTileObjectMerged += TileObjectMerged;
            ThisTileNode.onTileObjectChanged += PlaceObjectOnTile;
            ThisTileNode.onUpdateMergeableIndicator += UpdateMergeableIndicator;
        }

        private void UnsubscribeTileNodeEvents()
        {
            ThisTileNode.onTileObjectMerged -= TileObjectMerged;
            ThisTileNode.onTileObjectChanged -= PlaceObjectOnTile;
            ThisTileNode.onUpdateMergeableIndicator -= UpdateMergeableIndicator;
        }

        private void SubscribeTileNodeDetectorEvents()
        {
            tileNodeObjectDetectionHandler.onTileObjectEntered += ObjectEnterTileArea;
            tileNodeObjectDetectionHandler.onTileObjectExited += ObjectExitTileArea;
            tileNodeObjectDetectionHandler.onTileObjectPlaced += TileObjectPlaced;
        }        
        
        private void UnsubscribeTileNodeDetectorEvents()
        {
            tileNodeObjectDetectionHandler.onTileObjectEntered -= ObjectEnterTileArea;
            tileNodeObjectDetectionHandler.onTileObjectExited -= ObjectExitTileArea;
            tileNodeObjectDetectionHandler.onTileObjectPlaced -= TileObjectPlaced;
        }
        #endregion
    }
}
