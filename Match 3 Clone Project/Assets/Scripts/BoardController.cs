using UnityEngine;
using System.Collections.Generic;

public class BoardController : Singleton<BoardController>
{
    
    private static readonly int ROCKET_MATCH_COUNT = 5;
    private static readonly int BOMB_MATCH_COUNT = 7;


    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] GameObject rocketPrefab;

    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;


    BoardObject[,] grid;
    GameObject[] columns;


    void Start()
    {
        InitBoardWithCubes();
        SetShapesOfMatchingGroups();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool gotHit = Physics.Raycast(ray, out hitInfo, 50);

            if(gotHit)
            {
                BoardObject hitObject = hitInfo.collider.GetComponent<BoardObject>();
                if(hitObject is Cube)
                {
                    PopCube((Cube) hitObject);
                    SetShapesOfMatchingGroups();
                }
                else if(hitObject is Bomb)
                {
                    ExploadBomb((Bomb) hitObject);
                    SetShapesOfMatchingGroups();
                }
                else if(hitObject is Rocket)
                {
                    FireRocket((Rocket) hitObject);
                    SetShapesOfMatchingGroups();
                }
            }
        }
    }

    void InitBoardWithCubes()
    {
        grid = new BoardObject[gridWidth, gridHeight];
        columns = new GameObject[gridWidth];

        int[] cubesToSpawnOnColumn = new int[gridWidth];
        Vector3 colPos;

        for(int col = 0; col < gridWidth; col++)
        {
            columns[col] = new GameObject("Column" + (col + 1));
            columns[col].transform.SetParent(transform);
            colPos = transform.position + Vector3.left * ((gridWidth - 1) / 2f - col);
            columns[col].transform.position = colPos;

            cubesToSpawnOnColumn[col] = gridHeight;
        }
        SpawnNewCubesOnColumns(cubesToSpawnOnColumn);
    }

    void PopCube(Cube cube)
    {
        List<Cube> matchingCubes = GetMatchingCubes(cube);
        int matchedCubeCount = matchingCubes.Count;

        if(matchedCubeCount < 1) return;

        int[] removedCubes = new int[gridWidth];

        // Remove this cube
        if(matchedCubeCount + 1 >= BOMB_MATCH_COUNT)
        {
            SwapCubeWithBomb(cube);

        }
        else if(matchedCubeCount + 1 >= ROCKET_MATCH_COUNT)
        {
            SwapCubeWithRocket(cube);
        }
        else
        {
            removedCubes[cube.GridPosition.x]++;
            RemoveSlotFromGrid(cube);
        }

        foreach(Cube _cube in matchingCubes)
        {
            removedCubes[_cube.GridPosition.x]++;
            RemoveSlotFromGrid(_cube);
        }
        SpawnNewCubesOnColumns(removedCubes);

    }

    void SwapCubeWithBomb(Cube cube)
    {
        GameObject bombObj = Instantiate(bombPrefab,cube.transform.position, Quaternion.identity);
        Bomb bomb = bombObj.GetComponent<Bomb>();
        bomb.GridPosition = cube.GridPosition;
        bomb.transform.SetParent(cube.transform.parent);
        bomb.transform.SetSiblingIndex(cube.transform.GetSiblingIndex());

        grid[cube.GridPosition.x, cube.GridPosition.y] = bomb;
        Destroy(cube.gameObject);
    }

    void SwapCubeWithRocket(Cube cube)
    {
        GameObject rocketObj = Instantiate(rocketPrefab, cube.transform.position, Quaternion.identity);
        Rocket rocket = rocketObj.GetComponent<Rocket>();
        rocket.GridPosition = cube.GridPosition;
        rocket.transform.SetParent(cube.transform.parent);
        rocket.transform.SetSiblingIndex(cube.transform.GetSiblingIndex());

        grid[cube.GridPosition.x, cube.GridPosition.y] = rocket;
        Destroy(cube.gameObject);
    }

    void ExploadBomb(Bomb bomb)
    {
        int[] removedSlots = new int[gridWidth];
        List<BoardObject> boardObjectsToRemove = new List<BoardObject>();

        for(int xCoordDelta = -1; xCoordDelta < 2; xCoordDelta++)
        {
            for(int yCoordDelta = -1; yCoordDelta < 2; yCoordDelta++)
            {
                Vector2Int neigborPos = bomb.GridPosition.pos + new Vector2Int(xCoordDelta, yCoordDelta);
                if(IsInsideGrid(neigborPos))
                {
                    removedSlots[neigborPos.x]++;
                    boardObjectsToRemove.Add(grid[neigborPos.x, neigborPos.y]);
                }
            }
        }
        foreach(BoardObject obj in boardObjectsToRemove)
        {
            RemoveSlotFromGrid(obj);
        }

        SpawnNewCubesOnColumns(removedSlots);
    }

    void FireRocket(Rocket rocket)
    {
        RemoveSlotFromGrid(rocket);
        rocket.Fire();
        int[] cubesToSpawnOnColumns = new int[gridWidth];
        cubesToSpawnOnColumns[rocket.GridPosition.x]++;
        SpawnNewCubesOnColumns(cubesToSpawnOnColumns);
    }

    void RemoveSlotFromGrid(BoardObject objectToRemove)
    {
        int xPos = objectToRemove.GridPosition.x;
        int yPos = objectToRemove.GridPosition.y;
        int numObjectsAbove = gridHeight - (yPos + 1);

        for(int i = 0; i < numObjectsAbove; i++)
        {
            int objectAboveYPos = yPos + i + 1;
            if(objectAboveYPos >= gridHeight || grid[xPos, objectAboveYPos] == null) break;

            BoardObject objectAbove = grid[xPos, objectAboveYPos];
            objectAbove.GridPosition.y--;
            grid[xPos, objectAboveYPos - 1] = objectAbove;
            grid[xPos, objectAboveYPos] = null;   
        }
        Destroy(objectToRemove.gameObject);
    }

    void SetShapesOfMatchingGroups()
    {
        bool[,] visited = new bool[gridWidth, gridHeight];

        foreach(BoardObject boardObject in grid)
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

    List<Cube> GetMatchingCubes(Cube cube)
    {
        List<Cube> neighbours = new List<Cube>();
        bool[,] visited = new bool[gridWidth, gridHeight];

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
                // We only need to check 4-neighbors
                if(Mathf.Abs(xDelta + yDelta) != 1) continue;

                Vector2Int neighborPos = cube.GridPosition.pos + new Vector2Int(xDelta, yDelta);

                if(!IsInsideGrid(neighborPos.x, neighborPos.y)) continue;

                Cube cubeBeingVisited = grid[neighborPos.x, neighborPos.y] as Cube;
                if(!memoVisited[neighborPos.x, neighborPos.y]
                   && cubeBeingVisited != null
                   && cubeBeingVisited.ColorChanger.cubeColor
                    == cube.ColorChanger.cubeColor)
                {
                    memoNeighbours.Add(cubeBeingVisited);
                    GetMatchingNeighborsRecursive(cubeBeingVisited, ref memoNeighbours, ref memoVisited);
                }
            }
        }
    }

    void SpawnNewCubesOnColumns(int[] cubeAmounts)
    {
        for(int i = 0; i < cubeAmounts.Length; i++)
        {
            int cubeNumToInsantiate = cubeAmounts[i];
            Vector3 colPos = columns[i].transform.position;
            Vector3 newCubePos;
            int heightOffset = 2;

            for(int j = 0; j < cubeNumToInsantiate; j++)
            {
                newCubePos = colPos + Vector3.up * ((gridHeight - 1) / 2f + j + heightOffset);
                GameObject cubeObj = Instantiate(cubePrefab, newCubePos, Quaternion.identity);
                cubeObj.transform.SetParent(columns[i].transform);
                cubeObj.transform.SetAsLastSibling();

                Cube cube = cubeObj.GetComponent<Cube>();
                grid[i, gridHeight - cubeNumToInsantiate + j] = cube;
                cube.GridPosition.pos = new Vector2Int(i, gridHeight - cubeNumToInsantiate + j);

                cube.ColorChanger.AssignRandomColor();
                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.TearDrop);
            }
        }
    }

    void SpawnCubeOnColumn(int columnIndex)
    {
        Vector3 colPos = columns[columnIndex].transform.position;
        int heightOffset = 2;
        Vector3 newCubePos = colPos + Vector3.up * ((gridHeight - 1) / 2f + heightOffset);
        GameObject cubeObj = Instantiate(cubePrefab, newCubePos, Quaternion.identity);
        cubeObj.transform.SetParent(columns[columnIndex].transform);
        cubeObj.transform.SetAsLastSibling();

        Cube cube = cubeObj.GetComponent<Cube>();
        grid[columnIndex, gridHeight - 1] = cube;
        cube.GridPosition.pos = new Vector2Int(columnIndex, gridHeight - 1);

        cube.ColorChanger.AssignRandomColor();
        cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.TearDrop);
    }

    public void DestroySingleCube(Cube cube)
    {
        RemoveSlotFromGrid(cube);
        SpawnCubeOnColumn(cube.GridPosition.x);
    }

    bool IsInsideGrid(int x, int y)
    {
        return (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight);
    }

    bool IsInsideGrid(Vector2Int gridPos)
    {
        return IsInsideGrid(gridPos.x, gridPos.y);
    }
}
