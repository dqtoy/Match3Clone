using UnityEngine;

/// <summary>
/// Controls the shape of a cube by changing normal map of the material.
/// </summary>
public class ShapeController : MonoBehaviour 
{

    public enum Shape
    {
        TearDrop, Rocket
    }

    [SerializeField]
    Shape defaultShape;
    [SerializeField]
    Texture2D teardropNormalMap;
    [SerializeField]
    Texture2D rocketNormalMap;


    MeshRenderer m_renderer;


    void Awake()
    {
        GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("_NORMALMAP");
        SetShapeToDefault();
    }

    public void SetShape(Shape shape)
    {
        Texture2D normalMapToUse = null;
        switch(shape)
        {
            case Shape.TearDrop:
                normalMapToUse = teardropNormalMap;
                break;
            case Shape.Rocket:
                normalMapToUse = rocketNormalMap;
                break;
        }

        Renderer ourRenderer = GetComponent<MeshRenderer>();
        Material tempMaterial = new Material(ourRenderer.sharedMaterial);
        tempMaterial.SetTexture("_BumpMap", normalMapToUse);
        ourRenderer.sharedMaterial = tempMaterial;
    }

    public void SetShapeToDefault()
    {
        SetShape(defaultShape);
    }
}
