using UnityEngine;

public static class Utils {
    /// <summary>
    /// Set pivot without changing the position of the element
    /// </summary>
    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 deltaPosition = pivot - rectTransform.pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation
    
        rectTransform.localPosition += deltaPosition;           // reverse the position change
        rectTransform.pivot = pivot;                            // change the pivot
        Debug.Log(deltaPosition);
    }
}
