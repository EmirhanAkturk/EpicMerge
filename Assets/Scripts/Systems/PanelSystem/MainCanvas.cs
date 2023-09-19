using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.PanelSystem
{
    public class MainCanvas : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
