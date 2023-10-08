using _Game.Scripts.Systems.TileNodeSystem;
using JoostenProductions;
using Systems.MoveSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class BaseTileObject : OverridableMonoBehaviour
    {
        public TileObjectValue TileObjectValue { get; private set; }
        public TileNode CurrentTileNode { get;  set; } // for test 

        [SerializeField] private BaseTileObjectSpriteController baseTileObjectSpriteController;
        
        protected IMoveController MoveController => moveController ??= GetComponent<IMoveController>();
        private IMoveController moveController;

        protected override void OnDisable()
        {
            base.OnDisable();
            CurrentTileNode = null;
        }

        public virtual void Init(TileObjectValue tileObjectValue)
        {
            TileObjectValue = tileObjectValue;
            InitVisual(tileObjectValue);
        }

        private void InitVisual(TileObjectValue tileObjectValue)
        {
            if(tileObjectValue.IsEmptyTileObjectValue()) return;
            
            var objectData = TileObjectManager.Instance.GetObjectDataById(tileObjectValue.objectId);
            var dataByLevel = objectData.GetDataByLevel(tileObjectValue.objectLevel);
            
            Sprite sprite = dataByLevel?.sprite;
            baseTileObjectSpriteController.InitVisual(sprite);
        }

        public virtual bool CanObjectCentered()
        {
            return true;
        }
        
        protected void Move(Vector3 targetPos, MoveEndCallback onMoveEnd = null)
        {
            MoveController?.Move(targetPos, onMoveEnd);
        }        
        
        public virtual void MoveToTargetNode(Vector3 targetPos)
        {
            Move(targetPos);
        }
    }
}
