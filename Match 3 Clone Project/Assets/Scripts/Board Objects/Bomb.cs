using UnityEngine;

public class Bomb : BoardObject, IOnClickHandler, IOnHitHandler 
{


    protected override void Awake() 
    {
        base.Awake();
        base.clickHandler = this;
        base.hitHandler = this;
    }

    public void HandleOnClick()
    {
        Expload();
    }

    public void HandleOnHit()
    {
        if(handledHitThisTurn) return;
        Expload();
    }

    void Expload()
    {
        BoosterController.Instance.NotifyBoosterActivated();

        handledHitThisTurn = true;
        foreach(BoardObject boardObj in BoardController.Instance.GetSurroundingBoardObjects(this))
        {
            boardObj.HandleHit();
        }

        BoardController.Instance.NotifyDestroyedObject(this);
        BoosterController.Instance.NotifyBoosterDeactivated();
        Destroy(gameObject);
    }
	

}
