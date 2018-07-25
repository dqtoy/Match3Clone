using UnityEngine;

public class BoardObject : MonoBehaviour 
{
    
    public GridCoordinate GridPosition;
    public bool IsMoveable = true;

    /*
     * These handlers must be set by derived classes, otherwise they will use default implementation. 
     */
    protected IOnClickHandler clickHandler;
    protected IOnHitHandler hitHandler;
    protected IOnNearPopHandler nearPopHandler;

    protected bool handledHitThisTurn;

    Vector3 savedWorldPosition;
    bool isPositionLocked;

    void LateUpdate()
    {
        if(isPositionLocked)
        {
            transform.position = savedWorldPosition;
        }
    }

    /// <summary>
    /// Handles user click, happening on this board object.
    /// </summary>
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

    /// <summary>
    /// Handles hit by other objects, such as boosters.
    /// </summary>
    public void HandleHit()
    {
        if(hitHandler != null)
        {
            hitHandler.HandleOnHit();
        }
    }

    /// <summary>
    /// Must be called by other cubes which popped next to this board object.
    /// </summary>
    public void HandleNearPop()
    {
        if(nearPopHandler != null)
        {
            nearPopHandler.HandleOnNearPop();
        }
    }

    /// <summary>
    /// Locks/Unlocks the position of this board object.
    /// </summary>
    /// <param name="isLocked">If set to <c>true</c> is locked.</param>
    public void SetPositionLock(bool isLocked)
    {
        if(isLocked)
        {
            savedWorldPosition = transform.position;
        }
        isPositionLocked = isLocked;
    }

    /// <summary>
    /// Wobble this board object to give the feedback of "not matched" situation.
    /// </summary>
    public void Wobble()
    {
        Debug.Log("Wobble");
        // TODO: Implement wobble animation, without moving the collider.
    }


}
