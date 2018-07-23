using UnityEngine;
using System.Collections.Generic;
using System;

public class BoardController : Singleton<BoardController>
{
    public static readonly int ROCKET_MATCH_COUNT = 5;
    public static readonly int BOMB_MATCH_COUNT = 7;
    public static readonly int DISCOBALL_MATCH_COUNT = 9;


    [SerializeField] int boardWidth;
    [SerializeField] int boardHeight;

    [HideInInspector]
    public GameObject[] Columns;

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
                    isInputEnabled = false;
                    SetPositionLockOfAllBoardObjects(true);

                    hitObject.HandleClick();
                }
            }
        }    
    }

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

    public void OnClickHandled()
    {
        UpdateGridPositionsOfFallingObjects();
        CubeSpawner.Instance.SpawnNeededCubes(ref board);

        SetPositionLockOfAllBoardObjects(false);

        // TODO: Wait for cubes to settle

        SetShapesOfMatchingGroups();

        isInputEnabled = true;
    }

    public void AssignToBoard(BoardObject boardObj)
    {
        board[boardObj.GridPosition.x, boardObj.GridPosition.y] = boardObj;
    }

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
            if(cube == null || visited[cube.GridPosition.x, cube.GridPosition.y]) continue;

            visited[cube.GridPosition.x, cube.GridPosition.y] = true;

            List<Cube> matchingCubes = GetMatchingCubes(cube);

            if(matchingCubes.Count + 1 >= ROCKET_MATCH_COUNT)
            {
                foreach(Cube matchingCube in matchingCubes)
                {
                    visited[matchingCube.GridPosition.x, matchingCube.GridPosition.y] = true;
                    matchingCube.ShapeDrawer.SetShape(ShapeDrawer.Shape.Rocket);
                }
                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.Rocket);
            }
            else
            {
                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.TearDrop);
            }
        }
    }


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

    public List<Cube> GetMatchingCubes(Cube cube)
    {
        List<Cube> neighbours = new List<Cube>();
        bool[,] visited = new bool[boardWidth, boardHeight];

        GetMatchingNeighborsRecursive(cube, ref neighbours, ref visited);
        return neighbours;
    }

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

    bool IsInsideBoard(int x, int y)
    {
        return (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight);
    }

    bool IsInsideBoard(Vector2Int gridPos)
    {
        return IsInsideBoard(gridPos.x, gridPos.y);
    }
}
