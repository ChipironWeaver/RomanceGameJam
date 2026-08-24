using UnityEngine;
using UnityEngine.EventSystems;

public class UIChangeCursorOnOver : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public CursorType onEnterCursor = CursorType.Hover;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        CustomCursorBehavior.Instance.SetCursor(onEnterCursor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CustomCursorBehavior.Instance.SetCursor(CustomCursorBehavior.Instance.defaultCursor);
    }

}
