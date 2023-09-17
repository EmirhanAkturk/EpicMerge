using UnityEngine;

namespace _Game.Scripts.Systems.IndicatorSystem
{
    public class ObjectShowIndicator : BaseIndicator
    {
        [SerializeField] private GameObject modelParent;
    
        protected override void ShowIndicator()
        {
            modelParent.SetActive(true);
        }

        protected override void HideIndicator()
        {
            modelParent.SetActive(false);
        }
    }
}
