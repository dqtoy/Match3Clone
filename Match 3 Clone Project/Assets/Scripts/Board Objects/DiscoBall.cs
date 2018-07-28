using UnityEngine;

public class DiscoBall : BoardObject, IOnClickHandler, IOnHitHandler 
{

    [HideInInspector]
    public ColorController ColorChanger { get; private set; }


    void Awake()
    {
        ColorChanger = GetComponent<ColorController>();
        base.clickHandler = this;
        base.hitHandler = this;
        transform.Rotate(0, 180, 0);
    }


    public void HandleOnClick()
    {
        PopSameColoredCubes();
    }
    
    public void HandleOnHit()
    {
        if(handledHitThisTurn) return;
        PopSameColoredCubes();
    }


    void PopSameColoredCubes()
    {
        BoosterController.Instance.NotifyBoosterActivated();

        handledHitThisTurn = true;

        foreach(Cube cube in BoardController.Instance.GetAllCubesWithColor(ColorChanger.CurrentColor))
        {
            cube.HandleHit();
        }

        BoardController.Instance.NotifyDestroyedObject(this);
        BoosterController.Instance.NotifyBoosterDeactivated();
        Destroy(gameObject);
    }

}
