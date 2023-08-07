using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FillAmountChanger : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private Image targetImage = null;
    public float rate;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(targetImage.rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float normalizedY = (localPoint.x * rate - targetImage.rectTransform.rect.yMin) / (targetImage.rectTransform.rect.yMax - targetImage.rectTransform.rect.yMin);
            targetImage.fillAmount = Mathf.Clamp01(normalizedY);
        }
    }
}
