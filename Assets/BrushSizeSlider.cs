using UnityEngine;
using UnityEngine.UI;

public class BrushSizeSlider : MonoBehaviour
{
    public TexturePainter texturePainter;
    public Slider brushSizeSlider;

    void Start()
    {
        brushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
    }

    void OnBrushSizeChanged(float value)
    {
        texturePainter.SetBrushSize((int)value);
    }
}
    