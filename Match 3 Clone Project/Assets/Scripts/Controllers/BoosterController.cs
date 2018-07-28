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

    [HideInInspector]
    public int CurrentlyActiveBoosters;


    /// <summary>
    /// Creates a rocket booster at given position.
    /// </summary>
    /// <param name="boardPos">Board position.</param>
    /// <param name="worldPos">World position.</param>
    public void CreateRocketAtPosition(GridCoordinate boardPos, Vector3 worldPos)
    {
        BoardObject newBoardObj = Instantiate(rocketPrefab).GetComponent<BoardObject>();
        newBoardObj.GridPosition = boardPos;
        newBoardObj.transform.position = worldPos;
        BoardController.Instance.AssignToBoard(newBoardObj);
    }

    /// <summary>
    /// Creates bomb booster at given position.
    /// </summary>
    /// <param name="boardPos">Board position.</param>
    /// <param name="worldPos">World position.</param>
    public void CreateBombAtPosition(GridCoordinate boardPos, Vector3 worldPos)
    {
        BoardObject newBoardObj = Instantiate(bombPrefab).GetComponent<BoardObject>();
        newBoardObj.GridPosition = boardPos;
        newBoardObj.transform.position = worldPos;
        BoardController.Instance.AssignToBoard(newBoardObj);
    }

    /// <summary>
    /// Creates a disco ball booster at position.
    /// </summary>
    /// <param name="boardPos">Board position.</param>
    /// <param name="worldPos">World position.</param>
    /// <param name="color">Color.</param>
    public void CreateDiscoBallAtPosition(GridCoordinate boardPos, Vector3 worldPos, ColorController.ToonColor color)
    {
        BoardObject newBoardObj = Instantiate(discoBallPrefab).GetComponent<BoardObject>();
        newBoardObj.GetComponent<ColorController>().SetColor(color);
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
