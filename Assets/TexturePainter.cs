using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter : MonoBehaviour
{
    public Camera mainCamera;
    public Material cubeMaterial;
    public float drawInterval = 0.1f;
    public int interpolationPixelCount = 1;
    public Color drawColor = Color.red;
    public float rotationSpeed = 5f;
    public int brushSize = 1;

    private Texture2D texture;
    private Queue<Vector2Int> drawPoints = new Queue<Vector2Int>();
    private bool isDrawingMode = false;
    private bool isRotating = false;
    private Vector3 lastMousePosition;
    private bool textureModified = false;
    private string debugInfo = "";

    void Start()
    {
        texture = new Texture2D(512, 512);
        cubeMaterial.mainTexture = texture;
        StartCoroutine(DrawToCanvas());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDrawingMode)
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        if (isRotating && !isDrawingMode)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
            transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDrawingMode)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                Vector2 uv = hit.textureCoord;
                Vector2Int drawPos = new Vector2Int((int)(uv.x * texture.width), (int)(uv.y * texture.height));
                AddDrawPositions(drawPos);
                textureModified = true;
            }
        }
    }

    private void SetPixelsBetweenDrawPoints()
    {
        while (drawPoints.Count > 1)
        {
            Vector2Int startPos = drawPoints.Dequeue();
            Vector2Int endPos = drawPoints.Peek();
            InterpolateDrawPositions(startPos, endPos);
        }
    }

    IEnumerator DrawToCanvas()
    {
        while (true)
        {
            SetPixelsBetweenDrawPoints();
            if (textureModified)
            {
                texture.Apply();
                textureModified = false;
            }
            yield return new WaitForSeconds(drawInterval);
        }
    }

    void InterpolateDrawPositions(Vector2Int startPos, Vector2Int endPos)
    {
        int dx = endPos.x - startPos.x;
        int dy = endPos.y - startPos.y;
        float xinc = ((float)dx / Mathf.Abs(dx)) * interpolationPixelCount;
        float yinc = ((float)dy / Mathf.Abs(dy)) * interpolationPixelCount;
        float x = startPos.x;
        float y = startPos.y;

        int steps = (int)(Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) / interpolationPixelCount);
        for (int i = 0; i < steps; i++)
        {
            canvasDrawOrEraseAt(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
            x += xinc;
            y += yinc;
        }
        canvasDrawOrEraseAt(endPos.x, endPos.y);
    }

    void AddDrawPositions(Vector2Int newDrawPos)
    {
        drawPoints.Enqueue(newDrawPos);
    }

    void canvasDrawOrEraseAt(int x, int y)
    {
        if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
        {
            for (int i = -brushSize; i <= brushSize; i++)
            {
                for (int j = -brushSize; j <= brushSize; j++)
                {
                    if (x + i >= 0 && x + i < texture.width && y + j >= 0 && y + j < texture.height)
                    {
                        texture.SetPixel(x + i, y + j, drawColor);
                    }
                }
            }
            texture.Apply();
        }
    }

    public void SaveTextureToFile(string filePath)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePath, bytes);
    }

    public void LoadTextureFromFile(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            texture.LoadImage(bytes);
            texture.Apply();
        }
    }

    public void ClearTexture()
    {
        Color[] clearPixels = new Color[texture.width * texture.height];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = Color.white;
        }
        texture.SetPixels(clearPixels);
        texture.Apply();
    }

    public void ToggleDrawingMode()
    {
        isDrawingMode = !isDrawingMode;
        if (isDrawingMode)
        {
            isRotating = false;
        }
    }

    public void SetBrushSize(int newSize)
    {
        brushSize = newSize;
    }

    public void SetBrushColor(Color newColor)
    {
        drawColor = newColor;
    }
}
