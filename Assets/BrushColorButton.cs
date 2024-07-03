using UnityEngine;
using UnityEngine.UI;

public class BrushColorButton : MonoBehaviour
{
    public TexturePainter texturePainter;
    public Button brushColorButton;
    public Image brushColorImage;

    void Start()
    {
        brushColorButton.onClick.AddListener(OnBrushColorButtonClicked);
    }

    void OnBrushColorButtonClicked()
    {
        
        Color newColor = new Color(Random.value, Random.value, Random.value);
        brushColorImage.color = newColor;
        texturePainter.SetBrushColor(newColor);
    }
}
