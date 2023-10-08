using UnityEngine;

namespace Others
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField] private bool hide = false;
    
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(!hide);
        }
    }
}
