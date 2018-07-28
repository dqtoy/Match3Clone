using UnityEngine;

public class RocketHalf : MonoBehaviour 
{
    
    Rocket parentRocketController;

    float lifetime = 0.5f;
    Vector2 speed;
    bool hasFired;
    float timeElapsedSinceFire;


    void Update()
    {
        if(hasFired)
        {
            timeElapsedSinceFire += Time.deltaTime;
            transform.Translate(speed * Time.deltaTime);
        }
        if(timeElapsedSinceFire > lifetime)
        {
            parentRocketController.RocketHalfLeftBoard();
            Destroy(gameObject);
        }
    }


    public void FireWithSpeed(Vector2 speed)
    {
        this.speed = speed;
        hasFired = true;
    }

    public void SetRocketController(Rocket rocketController)
    {
        parentRocketController = rocketController;
    }

    void OnTriggerEnter(Collider col)
    {
        if(!hasFired) return;
        BoardObject boardObject = col.GetComponent<BoardObject>();
        if(boardObject != null)
        {
            boardObject.HandleHit();
        }
    }

}
