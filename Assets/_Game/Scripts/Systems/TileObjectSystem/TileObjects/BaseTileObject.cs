using System.Net.Http.Headers;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using JoostenProductions;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class BaseTileObject : OverridableMonoBehaviour
    {
        public TileObjectValue TileObjectValue { get; private set; }
        public TileNode TileNode { get;  set; } // for test 

        [FormerlySerializedAs("tileObjectModelController")] [SerializeField] private BaseTileObjectModelController baseTileObjectModelController;
        
        protected IMoveController MoveController => moveController ??= GetComponent<IMoveController>();
        private IMoveController moveController;
        
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
            
            Material material = objectData.objectMaterial;
            Mesh mesh = dataByLevel?.model;
            baseTileObjectModelController.InitVisual(mesh, material);
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
