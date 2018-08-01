using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    [HideInInspector]
    public bool IsGrounded;
    float distanceToBottom;
    Rigidbody ourRigidbody;

    void Start()
    {
        Collider ourCollider = GetComponent<Collider>();
        ourRigidbody = GetComponent<Rigidbody>();
        distanceToBottom = ourCollider.bounds.extents.y;
    }

    void Update()
    {
        if(!IsGrounded)
        {
            bool isVelocityZero = ourRigidbody.velocity.sqrMagnitude <= float.MinValue;
            if(!isVelocityZero)
            {
                IsGrounded = Physics.Raycast(ourRigidbody.position, Vector3.down, distanceToBottom + 0.1f);
            }
        }
    }

}