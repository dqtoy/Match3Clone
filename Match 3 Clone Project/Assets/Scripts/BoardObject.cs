using UnityEngine;

public class BoardObject : MonoBehaviour 
{
    
    public GridCoordinate GridPosition;
    public bool IsMoveable = true;

    protected IOnClickHandler clickHandler;
    protected IOnHitHandler hitHandler;
    protected IOnNearPopHandler nearPopHandler;

    Vector3 savedPosition;
    bool isPositionLocked;


    void LateUpdate()
    {
        if(isPositionLocked)
        {
            transform.position = savedPosition;
        }
    }


    public void HandleClick()
    {
        if(clickHandler != null)
        {
            clickHandler.HandleOnClick();
        }
        else
        {
            Wobble();
        }
    }

    public void HandleHit()
    {
        if(hitHandler != null)
        {
            hitHandler.HandleOnHit();
        }
    }

    public void HandleNearPop()
    {
        if(nearPopHandler != null)
        {
            nearPopHandler.HandleOnNearPop();
        }
    }


    public void SetPositionLock(bool isLocked)
    {
        if(isLocked)
        {
            savedPosition = transform.position;
        }
        isPositionLocked = isLocked;
    }

    public void Wobble()
    {
        Debug.Log("Wobble");
        // TODO: Implement wobble animation
    }


}
