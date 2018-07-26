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
        m_renderer = GetComponent<MeshRenderer>();
        m_renderer.material.EnableKeyword("_NORMALMAP");
        SetShape(defaultShape);
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
        m_renderer.material.SetTexture("_BumpMap", normalMapToUse);
    }

    public void SetShapeToDefault()
    {
        SetShape(defaultShape);
    }
}
