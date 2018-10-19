using UnityEngine;

/// <summary>
/// Controls the color of the material that this script is attached to.
/// </summary>
public class ColorController : MonoBehaviour 
{

    public enum ToonColor
    {
        Blue, Green, Red, Yellow, COUNT
    }

    public ToonColor CurrentColor;

    [SerializeField]
    Texture2D blueColorTexture;
    [SerializeField]
    Texture2D greenColorTexture;
    [SerializeField]
    Texture2D redColorTexture;
    [SerializeField]
    Texture2D yellowColorTexture;


    /// <summary>
    /// Changes the color of the material that this instance has.
    /// </summary>
    /// <param name="color">Color.</param>
    public void SetColor(ToonColor color)
    {
        Texture2D colorTexture = null;
        CurrentColor = color;

        switch(color)
        {
            case ToonColor.Blue:
                colorTexture = blueColorTexture;
                break;
            case ToonColor.Green:
                colorTexture = greenColorTexture;
                break;
            case ToonColor.Red:
                colorTexture = redColorTexture;
                break;
            case ToonColor.Yellow:
                colorTexture = yellowColorTexture;
                break;
        }


        Renderer ourRenderer = GetComponent<MeshRenderer>();
        Material tempMaterial = new Material(ourRenderer.sharedMaterial);
        tempMaterial.SetTexture("_MainTex", colorTexture);
        ourRenderer.sharedMaterial = tempMaterial;
    }

    /// <summary>
    /// Randomly changes the color of the material that this instance has.
    /// </summary>
    public void AssignRandomColor()
    {
        int randomIndex = Random.Range(0, (int) ToonColor.COUNT);
        ToonColor randomColor = (ToonColor)randomIndex;
        SetColor(randomColor);
    }
    
}
