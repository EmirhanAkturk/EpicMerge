using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.Scripts.Systems.TileObjectSystem
{
    public class BaseTileObjectSpriteController : MonoBehaviour
    {
        [Space]
        [SerializeField] private SpriteRenderer spriteRenderer;
   
        public void InitVisual(Sprite sprite)
        {
            SetSprite(sprite);
        }
        
        #region Set Functions

        private void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        #endregion
    }
}
