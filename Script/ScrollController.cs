using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollStep = 0.25f; // Her adýmda kaydýrýlacak mesafe

    public void ScrollUp()
    {
        float newPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollStep);
        scrollRect.verticalNormalizedPosition = newPosition;
    }

    public void ScrollDown()
    {
        float newPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - scrollStep);
        scrollRect.verticalNormalizedPosition = newPosition;
    }
}
