using UnityEngine;
namespace _Game.Scripts.Systems.TileSystem.TileObjectSystem
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
