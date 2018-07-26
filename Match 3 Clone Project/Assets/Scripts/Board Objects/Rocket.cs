using UnityEngine;

public class Rocket : BoardObject, IOnClickHandler, IOnHitHandler 
{

    [SerializeField] GameObject leftHalf;
    [SerializeField] GameObject rightHalf;
    [SerializeField] float rocketSpeed = 50f;

    bool fired;
    int numRocketHalvesLeftBoard;

    RocketHalf leftRocketHalf;
    RocketHalf rightRocketHalf;
    Collider slotCollider;


    void Awake()
    {
        base.clickHandler = this;
        base.hitHandler = this;
        leftRocketHalf = leftHalf.GetComponent<RocketHalf>();
        rightRocketHalf = rightHalf.GetComponent<RocketHalf>();
        slotCollider = GetComponent<Collider>();
    }


    public void HandleOnClick()
    {
        Fire();
    }

    public void HandleOnHit()
    {
        if(handledHitThisTurn) return;
        Fire();
    }

    void Fire()
    {
        slotCollider.enabled = false;
        handledHitThisTurn = true;

        leftRocketHalf.SetRocketController(this);
        rightRocketHalf.SetRocketController(this);
        leftRocketHalf.FireWithSpeed(Vector2.left * rocketSpeed);
        rightRocketHalf.FireWithSpeed(Vector2.right * rocketSpeed);
    }

    public void RocketHalfLeftBoard()
    {
        numRocketHalvesLeftBoard++;
        if(numRocketHalvesLeftBoard >= 2)
        {
            BoardController.Instance.NotifyDestroyedObject(this);
            BoardController.Instance.OnClickHandled();
            Destroy(gameObject);
        }
    }
}
