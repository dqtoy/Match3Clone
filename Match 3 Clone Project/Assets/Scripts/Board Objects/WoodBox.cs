using UnityEngine;

public class WoodBox : BoardObject, IOnHitHandler, IOnNearPopHandler 
{


    protected override void Awake()
    {
        base.Awake();
        base.hitHandler = this;
        base.nearPopHandler = this;
    }

    public void HandleOnHit()
    {
        if(handledHitThisTurn) return;

        DestroySelf();
    }

    public void HandleOnNearPop()
    {
        if(handledHitThisTurn) return;

        DestroySelf();
    }

    public void DestroySelf()
    {
        handledHitThisTurn = true;
        BoardController.Instance.NotifyDestroyedObject(this);
        Destroy(gameObject);
    }
}
