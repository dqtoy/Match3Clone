using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class that represents regular cubes in the game.
/// </summary>
[RequireComponent(typeof(ColorController), typeof(ShapeController))]
public class Cube : BoardObject, IOnClickHandler, IOnHitHandler 
{

    [HideInInspector] 
    public ColorController ColorChanger { get; private set; }

    [HideInInspector] 
    public ShapeController ShapeDrawer { get; private set; }

   
    protected override void Awake()
    {
        base.Awake();
        ColorChanger = GetComponent<ColorController>();
        ShapeDrawer = GetComponent<ShapeController>();
        base.clickHandler = this;
        base.hitHandler = this;
        transform.Rotate(0, 180, 0);
    }


    public void HandleOnClick()
    {
        List<Cube> matchingCubes = AlgoUtils.GetMatchingCubes(this);

        /* Note that matching cubes are not including this cube itself.
         * Therefore we need to add 1 one the count,
         * in order to get total number of cubes in this combination.
         */
        int numTotalCubesInCombo = matchingCubes.Count + 1;

        if(numTotalCubesInCombo < Constants.MIN_MATCH_COUNT)
        {
            Wobble();
        }
        else
        {
            foreach(Cube matchingCube in matchingCubes)
            {
                matchingCube.HandleOnHit();
            }
            DestroySelf();

            if(numTotalCubesInCombo >= Constants.DISCOBALL_MATCH_COUNT)
            {
                BoosterController.Instance.CreateDiscoBallAtPosition(GridPosition, transform.position
                                                                     , ColorChanger.CurrentColor);
            }
            else if(numTotalCubesInCombo >= Constants.BOMB_MATCH_COUNT)
            {
                BoosterController.Instance.CreateBombAtPosition(GridPosition, transform.position);
            }
            else if(numTotalCubesInCombo >= Constants.ROCKET_MATCH_COUNT)
            {
                BoosterController.Instance.CreateRocketAtPosition(GridPosition, transform.position);
            }
        }

        BoardController.Instance.OnClickHandled();
    }

    public void HandleOnHit()
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

