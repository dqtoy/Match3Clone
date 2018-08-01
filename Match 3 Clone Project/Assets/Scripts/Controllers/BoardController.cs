using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Class that is responsible for events happening on game board.
/// </summary>
public class BoardController : Singleton<BoardController>
{

    [SerializeField] int boardWidth;
    [SerializeField] int boardHeight;

    public int BoardWidth { get { return boardWidth; } private set { boardWidth = value; } }
    public int BoardHeight { get { return boardHeight; } private set { boardHeight = value; } }

    [HideInInspector] public GameObject[] Columns;

    BoardObject[,] board;
    

    public override void Awake()
    {
        base.Awake();
        InitBoard();
    }

    void Start()
    {
        OnClickHandled();
    }

    /// <summary>
    /// Initializes necessary containers for board objects.
    /// </summary>
    void InitBoard()
    {
        board = new BoardObject[BoardWidth, BoardHeight];
        Columns = new GameObject[BoardWidth];

        Vector3 colPos;
        for(int col = 0; col < BoardWidth; col++)
        {
            Columns[col] = new GameObject("Column" + (col + 1));
            Columns[col].transform.SetParent(transform);
            colPos = transform.position + Vector3.left * ((BoardWidth - 1) / 2f - col);
            Columns[col].transform.position = colPos;
        }
    }

    /// <summary>
    /// Lock/Unlock all the board objects' position which are currently on the board.
    /// </summary>
    /// <param name="isLocked">If set to <c>true</c> locks the position.</param>
    public void SetPositionLockOfAllBoardObjects(bool isLocked)
    {
        foreach(BoardObject boardObj in board)
        {
            if(boardObj != null)
            {
                boardObj.SetPositionLock(isLocked);
            }
        }
    }

    /// <summary>
    /// Function that tells the BoardController that all destructions and events are done.
    /// </summary>
    public void OnClickHandled()
    {
        if(BoosterController.Instance.CurrentlyActiveBoosters != 0) return;
        UpdateGridPositionsOfFallingObjects();
        CubeSpawner.Instance.SpawnNeededCubes(ref board);

        SetPositionLockOfAllBoardObjects(false);

        StartCoroutine(WaitForObjectsToSettleThenContinue());
    }


    IEnumerator WaitForObjectsToSettleThenContinue()
    {
        bool allSettled = false;
        while(!allSettled)
        {
            allSettled = true;
            foreach(BoardObject boardObj in board)
            {
                if(!boardObj.IsSettled())
                {
                    allSettled = false;
                    yield return null;
                }
            }
        }

        // Changes happened on the board, shapes must be updated.
        SetShapesOfMatchingGroups();
        InputManager.Instance.SetInputEnabled(true);
    }


    /// <summary>
    /// Assigns <paramref name="boardObj"/> to board.
    /// </summary>
    /// <param name="boardObj">Board object to be assigned.</param>
    public void AssignToBoard(BoardObject boardObj)
    {
        board[boardObj.GridPosition.x, boardObj.GridPosition.y] = boardObj;
        boardObj.transform.SetParent(Columns[boardObj.GridPosition.x].transform);
        boardObj.transform.SetSiblingIndex(boardObj.GridPosition.y);
    }

    /// <summary>
    /// Upon destruction of any BoardObject, sets the corresponding slot in board to null.
    /// </summary>
    /// <param name="boardObject">Board object.</param>
    public void NotifyDestroyedObject(BoardObject boardObject)
    {
        board[boardObject.GridPosition.x, boardObject.GridPosition.y] = null;
    }

    void SetShapesOfMatchingGroups()
    {
        bool[,] visited = new bool[BoardWidth, BoardHeight];

        foreach(BoardObject boardObject in board)
        {
            Cube cube = boardObject as Cube;
            // If this object is not a cube, or visited before, we don't need to check it.
            if(cube == null || visited[cube.GridPosition.x, cube.GridPosition.y]) continue;

            visited[cube.GridPosition.x, cube.GridPosition.y] = true;

            List<Cube> matchingCubes = AlgoUtils.GetMatchingCubes(cube);

            // TODO: Change this part after adding new normal maps for bomb and disco ball shapes.
            if(matchingCubes.Count + 1 >= Constants.ROCKET_MATCH_COUNT)
            {
                foreach(Cube matchingCube in matchingCubes)
                {
                    visited[matchingCube.GridPosition.x, matchingCube.GridPosition.y] = true;
                    matchingCube.ShapeDrawer.SetShape(ShapeController.Shape.Rocket);
                }
                cube.ShapeDrawer.SetShape(ShapeController.Shape.Rocket);
            }
            else
            {
                cube.ShapeDrawer.SetShape(ShapeController.Shape.TearDrop);
            }
        }
    }

    /// <summary>
    /// Iterates all the BoardObjects, lets them fall if there is nothing below them.
    /// </summary>
    void UpdateGridPositionsOfFallingObjects()
    {
        for(int x = 0; x < BoardWidth; x++)
        {
            for(int y = 0; y < BoardHeight; y++)
            {
                BoardObject currentObj = board[x, y];
                if(currentObj == null || !currentObj.IsMoveable) continue;

                while(IsInsideBoard(currentObj.GridPosition.pos + Vector2Int.down)
                      && board[currentObj.GridPosition.x, currentObj.GridPosition.y - 1] == null)
                {
                    MoveBoardObjectDown(currentObj);
                }
            }
        }
    }

    void MoveBoardObjectDown(BoardObject boardObject)
    {
        int xPos = boardObject.GridPosition.x;
        int yPos = boardObject.GridPosition.y;

        if(IsInsideBoard(xPos, yPos - 1))
        {
            board[xPos, yPos - 1] = boardObject;
            board[xPos, yPos] = null;
            boardObject.GridPosition.pos += Vector2Int.down;
        }
    }


    /// <summary>
    /// Gets the surrounding board objects.
    /// </summary>
    /// <returns>The surrounding board objects.</returns>
    /// <param name="boardObj">Board object.</param>
    public List<BoardObject> GetSurroundingBoardObjects(BoardObject boardObj)
    {
        List<BoardObject> neighbors = new List<BoardObject>();
        for(int xDelta = -1; xDelta < 2; xDelta++)
        {
            for(int yDelta = -1; yDelta < 2; yDelta++)
            {
                Vector2Int neighborPos = boardObj.GridPosition.pos + new Vector2Int(xDelta, yDelta);
                if(IsInsideBoard(neighborPos) && board[neighborPos.x, neighborPos.y] != null)
                {
                    neighbors.Add(board[neighborPos.x, neighborPos.y]);
                }
            }
        }
        return neighbors;
    }

    public List<Cube> GetAllCubesWithColor(ColorController.ToonColor color)
    {
        List<Cube> cubes = new List<Cube>();
        foreach(BoardObject boardObj in board)
        {
            Cube currentCube = boardObj as Cube;
            if(currentCube != null && currentCube.ColorChanger.CurrentColor == color)
            {
                cubes.Add(currentCube);
            }
        }
        return cubes;

    }

    public BoardObject GetBoardObjectAt(int xPos, int yPos)
    {
        if(IsInsideBoard(xPos, yPos))
        {
            return board[xPos, yPos];
        }
        else
        {
            return null;
        }
    }

    public Bounds GetBoundingBox()
    {
        Bounds bounds = new Bounds();
        foreach(BoardObject boardObj in board)
        {
            Collider col = boardObj.GetComponent<Collider>();
            bounds.Encapsulate(col.bounds);
        }
        return bounds;
    }

    public bool IsInsideBoard(int x, int y)
    {
        return (x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight);
    }

    public bool IsInsideBoard(Vector2Int gridPos)
    {
        return IsInsideBoard(gridPos.x, gridPos.y);
    }
}
