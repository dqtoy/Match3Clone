using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ColorChanger), typeof(ShapeDrawer))]
public class Cube : BoardObject, IOnClickHandler, IOnHitHandler 
{

    [HideInInspector]
    public ColorChanger ColorChanger { get; private set; }

    [HideInInspector]
    public ShapeDrawer ShapeDrawer { get; private set; }

   
    void Awake()
    {
        ColorChanger = GetComponent<ColorChanger>();
        ShapeDrawer = GetComponent<ShapeDrawer>();
        clickHandler = this;
        transform.Rotate(0, 180, 0);
    }


    public void HandleOnClick()
    {
        List<Cube> matchingCubes = BoardController.Instance.GetMatchingCubes(this);

        if(matchingCubes.Count < 1)
        {
            Wobble();
        }
        else
        {
            if(matchingCubes.Count + 1 >= BoardController.DISCOBALL_MATCH_COUNT)
            {
                Debug.Log("Turn into Disco Ball");
            }
            else if(matchingCubes.Count + 1 >= BoardController.BOMB_MATCH_COUNT)
            {
                Debug.Log("Turn into Bomb");
            }
            else if(matchingCubes.Count + 1 >= BoardController.ROCKET_MATCH_COUNT)
            {
                Debug.Log("Turn into Rocket");
            }

            foreach(Cube matchingCube in matchingCubes)
            {
                matchingCube.HandleOnHit();
            }
            DestroySelf();
        }

        BoardController.Instance.OnClickHandled();
    }

    public void HandleOnHit()
    {
        DestroySelf();
    }

    public void DestroySelf()
    {
        BoardController.Instance.NotifyDestroyedObject(this);
        Destroy(gameObject);
    }

}

