using UnityEngine;

namespace Others
{
    public class LookAtCamera : MonoBehaviour
    {
        public bool IsActive = true;
   
        private Camera CurrentCamera => currentCamera != null ? currentCamera : (currentCamera = Camera.main);
        private Camera currentCamera;

        private void Awake()
        {
            IsActive = true;
        }

        private void LateUpdate()
        {
            if (!IsActive || CurrentCamera == null) return;
            if(transform.rotation != CurrentCamera.transform.rotation)
            {
                transform.rotation = CurrentCamera.transform.rotation;
            }
        }
    }
}
