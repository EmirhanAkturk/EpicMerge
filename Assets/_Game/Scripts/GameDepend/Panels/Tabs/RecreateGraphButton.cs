using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.GameDepend.Panels.Tabs
{
    public class RecreateGraphButton : MonoBehaviour
    {
        [SerializeField] private GameObject visualPartParent;
        [Space]
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button button;

        private Action<int> onClickAction;
        
        private int graphGeneratorId;

        private void OnEnable()
        {
            button.onClick.AddListener(RecreateGraph);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(RecreateGraph);
        }

        public void Init(int graphId, string graphName, Action<int> buttonClickAction)
        {
            graphGeneratorId = graphId;
            buttonText.text = graphName;
            onClickAction = buttonClickAction;

            Show();
        }

        public void Show()
        {
            SetVisualState(true);
        }

        public void Hide()
        {
            SetVisualState(false);
        }

        private void SetVisualState(bool state)
        {
            visualPartParent.SetActive(state);
        }

        private void RecreateGraph()
        {
            onClickAction?.Invoke(graphGeneratorId);
        }
    }
}
