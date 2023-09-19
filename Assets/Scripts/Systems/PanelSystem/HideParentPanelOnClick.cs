using Systems.PanelSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideParentPanelOnClick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    private BasePanel parentPanel;

    private bool isEnabled = true;
    private void Awake()
    {
        parentPanel = GetComponentInParent<BasePanel>();
        if (parentPanel == null)
            Destroy(this.gameObject);
    }

    public void SetEnabled(bool enable)
    {
        isEnabled = enable;
    }
    public void OnPointerClick(PointerEventData eventData) => CheckToHide();
    public void OnPointerDown(PointerEventData eventData) => CheckToHide();

    private void CheckToHide()
    {
        if (parentPanel == null || !isEnabled)
            Destroy(this.gameObject);
       /* if(TutorialManager.Instance.IsTutorialPlaying)
            return;*/
        PanelManager.Instance.Hide(parentPanel);
    }

    private bool IsInsidePanel(Vector3 screenPoint)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), screenPoint);
    }
}