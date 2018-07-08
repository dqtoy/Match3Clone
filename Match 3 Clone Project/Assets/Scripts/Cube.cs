using UnityEngine;

[RequireComponent(typeof(ColorChanger), typeof(ShapeDrawer))]
public class Cube : BoardObject 
{
    [HideInInspector]
    public ColorChanger ColorChanger { get; private set; }

    [HideInInspector]
    public ShapeDrawer ShapeDrawer { get; private set; }


    void Awake()
    {
        ColorChanger = GetComponent<ColorChanger>();
        ShapeDrawer = GetComponent<ShapeDrawer>();
        transform.Rotate(0, 180, 0);
    }
}

