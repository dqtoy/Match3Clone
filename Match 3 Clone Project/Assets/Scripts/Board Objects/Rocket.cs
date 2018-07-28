using UnityEngine;

public class Rocket : BoardObject, IOnClickHandler, IOnHitHandler 
{

    [SerializeField] GameObject leftHalf;
    [SerializeField] GameObject rightHalf;
    [SerializeField] float rocketSpeed = 50f;

    int numRocketHalvesLeftBoard;

    RocketHalf leftRocketHalf;
    RocketHalf rightRocketHalf;
    BoxCollider slotCollider;


    void Awake()
    {
        base.clickHandler = this;
        base.hitHandler = this;
        leftRocketHalf = leftHalf.GetComponent<RocketHalf>();
        rightRocketHalf = rightHalf.GetComponent<RocketHalf>();
        slotCollider = GetComponent<BoxCollider>();

        bool isHorizontal = Random.Range(0f, 1f) > 0.5f;
        if(isHorizontal)
        {
            float rotateAngleInDegrees = -90;
            Vector3 newColliderSize = Quaternion.AngleAxis(rotateAngleInDegrees, Vector3.forward) * slotCollider.size;
            // Box collider shouldn't have any negative size values.
            newColliderSize = new Vector3(Mathf.Abs(newColliderSize.x)
                                          , Mathf.Abs(newColliderSize.y), Mathf.Abs(newColliderSize.z));
            transform.Rotate(0, 0, rotateAngleInDegrees);
            slotCollider.size = newColliderSize;
        }
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
        BoosterController.Instance.NotifyBoosterActivated();

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
            BoosterController.Instance.NotifyBoosterDeactivated();
            Destroy(gameObject);
        }
    }
}
