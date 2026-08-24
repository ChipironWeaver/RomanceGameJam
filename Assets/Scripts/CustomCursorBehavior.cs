using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursorBehavior : MonoBehaviour
{
    public CursorType defaultCursor = CursorType.Normal;
    
    [SerializeField] private Texture2D _normal;
    [SerializeField] private Texture2D _normalPressed;
    [SerializeField] private Texture2D _hover;
    [SerializeField] private Texture2D _hoverPressed;

    private CursorType _cursorType = CursorType.Normal;
    private bool _pressed = false;

    public void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            _pressed = true;
            SetPress(true);
            return;
        }
        _pressed = false;
        SetPress(false);
    }
    
    
    public void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Normal or CursorType.NormalPressed:
                Cursor.SetCursor(_pressed ? _normalPressed : _normal, Vector2.zero, CursorMode.ForceSoftware);
                _cursorType = _pressed ? CursorType.NormalPressed : CursorType.Normal;
                break;

            case CursorType.Hover or CursorType.HoverPressed:
                Cursor.SetCursor(_pressed ?  _hoverPressed : _hover, Vector2.zero, CursorMode.ForceSoftware);
                _cursorType = _pressed ? CursorType.HoverPressed : CursorType.Hover;
                break;
        }
    }

    public void SetPress(bool pressed)
    {
        switch (_cursorType)
        {
            case CursorType.Normal or CursorType.NormalPressed:
                Cursor.SetCursor(pressed ? _normalPressed : _normal, Vector2.zero, CursorMode.ForceSoftware);
                _cursorType = pressed ? CursorType.NormalPressed : CursorType.Normal;
                break;

            case CursorType.Hover or CursorType.HoverPressed:
                Cursor.SetCursor(pressed ?  _hoverPressed : _hover, Vector2.zero, CursorMode.ForceSoftware);
                _cursorType = pressed ? CursorType.HoverPressed : CursorType.Hover;
                break;
        }
    }
    
    void OnEnable()
    {
        Singleton();
        SetCursor(defaultCursor);
    }
    
    public static CustomCursorBehavior Instance{ get; private set; }
    
    void Singleton()
    {
        if (Instance !=null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
public enum CursorType
{
    Normal,
    NormalPressed,
    Hover,
    HoverPressed
}

