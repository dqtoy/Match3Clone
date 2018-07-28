using UnityEngine;

public class BoosterController : Singleton<BoosterController> 
{

    public enum BoosterType
    {
        Rocket, Bomb, DiscoBall
    }

    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] GameObject discoBallPrefab;

    public int CurrentlyActiveBoosters;

    /// <summary>
    /// Swaps the board object at that position, with a booster given type.
    /// </summary>
    /// <param name="boosterType">Booster type.</param>
    /// <param name="boardPos">Board position.</param>
    /// <param name="worldPos">World position.</param>
    public void CreateBoosterAtPosition(BoosterType boosterType, GridCoordinate boardPos, Vector3 worldPos)
    {
        BoardObject newBoardObj = null;
        switch(boosterType)
        {
            case BoosterType.Rocket:
                // TODO: Replace this with rocket prefab when ready.
                newBoardObj = Instantiate(rocketPrefab).GetComponent<BoardObject>();
                break;
            case BoosterType.Bomb:
                newBoardObj = Instantiate(bombPrefab).GetComponent<BoardObject>();
                break;
            case BoosterType.DiscoBall:
                // TODO: Replace this with disco ball prefab when ready.
                newBoardObj = Instantiate(bombPrefab).GetComponent<BoardObject>();
                break;
        }
        newBoardObj.GridPosition = boardPos;
        newBoardObj.transform.position = worldPos;

        BoardController.Instance.AssignToBoard(newBoardObj);
    }

    /// <summary>
    /// Notifies BoosterController that a booster object is being activated.
    /// </summary>
    public void NotifyBoosterActivated()
    {
        CurrentlyActiveBoosters++;
    }

    /// <summary>
    /// Notifies BoosterController that a booster object is being de-activated.
    /// </summary>
    public void NotifyBoosterDeactivated()
    {
        CurrentlyActiveBoosters--;
        if(CurrentlyActiveBoosters == 0)
        {
            BoardController.Instance.OnClickHandled();
        }
    }


}
