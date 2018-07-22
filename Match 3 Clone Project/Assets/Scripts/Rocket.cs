using UnityEngine;

public class Rocket : BoardObject 
{
    [SerializeField] GameObject leftHalf;
    [SerializeField] GameObject rightHalf;
    [SerializeField] float rocketSpeed = 10f;
    [SerializeField] float rocketLifeTime = 2f;


    public void Fire()
    {
        Rigidbody leftRB = leftHalf.AddComponent<Rigidbody>();
        leftRB.velocity = Vector3.left * rocketSpeed;
        leftRB.useGravity = false;

        Rigidbody rightRB = rightHalf.AddComponent<Rigidbody>();
        rightRB.velocity = Vector3.right * rocketSpeed;
        rightRB.useGravity = false;

        leftHalf.transform.parent = null;
        rightHalf.transform.parent = null;

        Destroy(leftHalf.gameObject, rocketLifeTime);
        Destroy(rightHalf.gameObject, rocketLifeTime);
        Destroy(this.gameObject);
    }
}
