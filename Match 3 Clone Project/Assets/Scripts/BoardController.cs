using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Class that is responsible for events happening on game board.
/// </summary>
public class BoardController : Singleton<BoardController>
{
    public static readonly int MIN_MATCH_COUNT = 2;
    public static readonly int ROCKET_MATCH_COUNT = 5;
    public static readonly int BOMB_MATCH_COUNT = 7;
    public static readonly int DISCOBALL_MATCH_COUNT = 9;

    public enum BoosterType
    {
        Rocket, Bomb, DiscoBall
    }

    [SerializeField] int boardWidth;
    [SerializeField] int boardHeight;

    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] GameObject discoBallPrefab;


    [HideInInspector] public GameObject[] Columns;


    BoardObject[,] board;
    bool isInputEnabled;


    public override void Awake()
    {
        base.Awake();
        InitBoard();
    }

    void Start()
    {
        OnClickHandled();
    }

    void Update()
    {
        if(isInputEnabled && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool gotHit = Physics.Raycast(ray, out hitInfo, 50);

            if(gotHit)
            {
                BoardObject hitObject = hitInfo.collider.GetComponent<BoardObject>();
                if(hitObject != null)
                {
                    // Input has to be disabled until all the objects are settled on board
                    isInputEnabled = false;

                    SetPositionLockOfAllBoardObjects(true);
                    hitObject.HandleClick();
                }
            }
        }    
    }

    /// <summary>
    /// Initializes necessary containers for board objects.
    /// </summary>
    void InitBoard()
    {
        board = new BoardObject[boardWidth, boardHeight];
        Columns = new GameObject[boardWidth];

        Vector3 colPos;
        for(int col = 0; col < boardWidth; col++)
        {
            Columns[col] = new GameObject("Column" + (col + 1));
            Columns[col].transform.SetParent(transform);
            colPos = transform.position + Vector3.left * ((boardWidth - 1) / 2f - col);
            Columns[col].transform.position = colPos;
        }
    }

    /// <summary>
    /// Lock/Unlock all the board objects' position which are currently on the board.
    /// </summary>
    /// <param name="isLocked">If set to <c>true</c> locks the position.</param>
    void SetPositionLockOfAllBoardObjects(bool isLocked)
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
        UpdateGridPositionsOfFallingObjects();
        CubeSpawner.Instance.SpawnNeededCubes(ref board);

        SetPositionLockOfAllBoardObjects(false);

        // TODO: Wait for cubes to settle

        // Changes happened on the board, shapes must be updated.
        SetShapesOfMatchingGroups();
        isInputEnabled = true;
    }

    /// <summary>
    /// Assigns <paramref name="boardObj"/> to board.
    /// </summary>
    /// <param name="boardObj">Board object to be assigned.</param>
    public void AssignToBoard(BoardObject boardObj)
    {
        board[boardObj.GridPosition.x, boardObj.GridPosition.y] = boardObj;
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
        bool[,] visited = new bool[boardWidth, boardHeight];

        foreach(BoardObject boardObject in board)
        {
            Cube cube = boardObject as Cube;
            // If this object is not a cube, or visited before, we don't need to check it.
            if(cube == null || visited[cube.GridPosition.x, cube.GridPosition.y]) continue;

            visited[cube.GridPosition.x, cube.GridPosition.y] = true;

            List<Cube> matchingCubes = GetMatchingCubes(cube);

            // TODO: Change this part after adding new normal maps for bomb and disco ball shapes.
            if(matchingCubes.Count + 1 >= ROCKET_MATCH_COUNT)
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
        for(int x = 0; x < boardWidth; x++)
        {
            for(int y = 0; y < boardHeight; y++)
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
    /// Wrapper function for match detection algorithm.
    /// </summary>
    /// <returns> matching cubes with the given one.</returns>
    /// <param name="cube">Current cube.</param>
    public List<Cube> GetMatchingCubes(Cube cube)
    {
        List<Cube> neighbours = new List<Cube>();
        bool[,] visited = new bool[boardWidth, boardHeight];

        GetMatchingNeighborsRecursive(cube, ref neighbours, ref visited);
        return neighbours;
    }

    /// <summary>
    /// Actual Flood and Fill Algorithm implementation.
    /// </summary>
    void GetMatchingNeighborsRecursive(Cube cube, ref List<Cube> memoNeighbours, ref bool[,] memoVisited)
    {
        memoVisited[cube.GridPosition.x, cube.GridPosition.y] = true;

        for(int xDelta = -1; xDelta < 2; xDelta++)
        {
            for(int yDelta = -1; yDelta < 2; yDelta++)
            {
                // We only need to check 4-neighbors, not the corners nor the center.
                if(Mathf.Abs(xDelta + yDelta) != 1) continue;

                Vector2Int neighborPos = cube.GridPosition.pos + new Vector2Int(xDelta, yDelta);

                if(!IsInsideBoard(neighborPos.x, neighborPos.y)) continue;

                Cube cubeBeingVisited = board[neighborPos.x, neighborPos.y] as Cube;
                if(!memoVisited[neighborPos.x, neighborPos.y]
                   && cubeBeingVisited != null
                   && cubeBeingVisited.ColorChanger.cubeColor == cube.ColorChanger.cubeColor)
                {
                    memoNeighbours.Add(cubeBeingVisited);
                    GetMatchingNeighborsRecursive(cubeBeingVisited, ref memoNeighbours, ref memoVisited);
                }
            }
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
                if(IsInsideBoard(neighborPos) &&  board[neighborPos.x, neighborPos.y] != null)
                {
                    neighbors.Add(board[neighborPos.x, neighborPos.y]);
                }
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Swaps the board object at that position, with a booster given type.
    /// </summary>
    /// <param name="boosterType">Booster type.</param>
    /// <param name="boardPos">Board position.</param>
    /// <param name="worldPos">World position.</param>

    public void CreateBoosterAtPosition(BoosterType boosterType, GridCoordinate boardPos, Vector3 worldPos)
    {
        BoardObject oldBoardObj = board[boardPos.x, boardPos.y];
        BoardObject newBoardObj = null;
        switch(boosterType)
        {
            case BoosterType.Rocket:
                // TODO: Replace this with rocket prefab when ready.
                newBoardObj = Instantiate(bombPrefab).GetComponent<BoardObject>();
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
        newBoardObj.transform.SetParent(Columns[boardPos.x].transform);
        newBoardObj.transform.SetSiblingIndex(boardPos.y);

        AssignToBoard(newBoardObj);
    }

    bool IsInsideBoard(int x, int y)
    {
        return (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight);
    }

    bool IsInsideBoard(Vector2Int gridPos)
    {
        return IsInsideBoard(gridPos.x, gridPos.y);
    }
}
