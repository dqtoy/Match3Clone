using UnityEngine;

public class ColorChanger : MonoBehaviour 
{

    public enum CubeColor
    {
        Blue, Green, Red, Yellow, COUNT
    }

    public CubeColor cubeColor;

    [SerializeField]
    Texture2D blueColorTexture;
    [SerializeField]
    Texture2D greenColorTexture;
    [SerializeField]
    Texture2D redColorTexture;
    [SerializeField]
    Texture2D yellowColorTexture;


    Renderer m_Renderer;

    void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
    }


    public void SetColor(CubeColor color)
    {
        Texture2D colorTexture = null;
        cubeColor = color;

        switch(color)
        {
            case CubeColor.Blue:
                colorTexture = blueColorTexture;
                break;
            case CubeColor.Green:
                colorTexture = greenColorTexture;
                break;
            case CubeColor.Red:
                colorTexture = redColorTexture;
                break;
            case CubeColor.Yellow:
                colorTexture = yellowColorTexture;
                break;
        }

        m_Renderer.material.SetTexture("_MainTex", colorTexture);
    }

    public void AssignRandomColor()
    {
        int randomIndex = Random.Range(0, (int) CubeColor.COUNT);
        CubeColor randomColor = (CubeColor)randomIndex;
        SetColor(randomColor);
    }
}
