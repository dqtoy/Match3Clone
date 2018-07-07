using UnityEngine;

[RequireComponent(typeof(ColorChanger), typeof(ShapeDrawer))]
public class Cube : MonoBehaviour 
{
    [HideInInspector]
    public ColorChanger ColorChanger { get; private set; }

    [HideInInspector]
    public ShapeDrawer ShapeDrawer { get; private set; }

    public GridCoordinate GridPosition;

    void Awake()
    {
        ColorChanger = GetComponent<ColorChanger>();
        ShapeDrawer = GetComponent<ShapeDrawer>();
        transform.Rotate(0, 180, 0);
    }
}

