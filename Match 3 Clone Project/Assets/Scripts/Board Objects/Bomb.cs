using UnityEngine;

public class Bomb : BoardObject, IOnClickHandler, IOnHitHandler 
{


    void Awake() 
    {
        base.clickHandler = this;
        base.hitHandler = this;
    }

    public void HandleOnClick()
    {
        Expload();
        BoardController.Instance.OnClickHandled();
    }

    public void HandleOnHit()
    {
        if(handledHitThisTurn) return;
        Expload();
    }

    void Expload()
    {
        handledHitThisTurn = true;
        foreach(BoardObject boardObj in BoardController.Instance.GetSurroundingBoardObjects(this))
        {
            boardObj.HandleHit();
        }

        BoardController.Instance.NotifyDestroyedObject(this);
        Destroy(gameObject);
    }
	

}
