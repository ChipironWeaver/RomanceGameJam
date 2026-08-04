using TMPro;
using UnityEngine;

public class TextRenderer : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;

    public void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    
    public void Render(string text)
    {
        _textMeshPro.text = text; //Improved with typewriter style
    }
}
