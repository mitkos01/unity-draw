using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public TexturePainter texturePainter; 

    public void OnSaveButtonClicked()
    {
        string savePath = Application.persistentDataPath + "/savedTexture.png";
        texturePainter.SaveTextureToFile(savePath);
    }

    public void OnLoadButtonClicked()
    {
        string loadPath = Application.persistentDataPath + "/savedTexture.png";
        texturePainter.LoadTextureFromFile(loadPath);
    }

    public void OnClearButtonClicked()
    {
        texturePainter.ClearTexture();
    }
}
