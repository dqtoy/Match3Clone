using UnityEngine;

public class RocketHalf : MonoBehaviour 
{

    void OnTriggerEnter(Collider other)
    {
        BoardObject boardObject = other.GetComponent<BoardObject>();

        if(boardObject == null) return;

        if(boardObject is Cube)
        {
            BoardController.Instance.DestroySingleCube((Cube)boardObject);
        }
    }
}
