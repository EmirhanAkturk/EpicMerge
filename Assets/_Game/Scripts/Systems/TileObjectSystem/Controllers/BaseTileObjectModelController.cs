using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class BaseTileObjectModelController : MonoBehaviour
    {
        [Space]
        [HelpBox("Bottom Part For Test", HelpBoxMessageType.Info)]        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
   
        public void InitVisual(Mesh mesh, Material material)
        {
            SetMaterial(material);
            SetMesh(mesh);
        }
        
        #region Set Functions

        private void SetMaterial(Material material)
        {
            if(material is null) return;
            meshRenderer.material = material;
        }
        
        private void SetMesh(Mesh mesh)
        {
            meshFilter.mesh = mesh;
        }
        #endregion
    }
}
