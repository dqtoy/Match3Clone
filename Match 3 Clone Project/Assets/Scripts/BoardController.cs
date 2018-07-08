using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{

    public static BoardController Instance;

    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject bombPrefab;

    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;

    private static readonly int ROCKET_MATCH_COUNT = 5;


    BoardObject[,] grid;
    GameObject[] columns;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

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
            RaycastHit hit;
            bool gotHit = Physics.Raycast(ray, out hit, 100);

            if(gotHit)
            {
                BoardObject hitObject = hit.collider.GetComponent<BoardObject>();
                if(hitObject is Cube)
                {
                    PopCube((Cube) hitObject);
                }
                else if(hitObject is Bomb)
                {
                    ExploadBomb((Bomb) hitObject);
                }
            }
        }
    }

    void InitBoardWithCubes()
    {
        grid = new BoardObject[gridWidth, gridHeight];
        columns = new GameObject[gridWidth];

        Vector3 colPos;
        Vector3 cubePos;

        for(int col = 0; col < gridWidth; col++)
        {
            columns[col] = new GameObject("Column" + (col + 1));
            columns[col].transform.SetParent(transform);
            colPos = transform.position + Vector3.left * ((gridWidth - 1) / 2f - col);
            columns[col].transform.position = colPos;

            
            for(int row = 0; row < gridHeight; row++)
            {
                cubePos = colPos + Vector3.down * ((gridHeight - 1) / 2f - row);
                GameObject cubeObj = Instantiate(cubePrefab, cubePos, Quaternion.identity);
                cubeObj.transform.SetParent(columns[col].transform);
                cubeObj.transform.SetAsLastSibling();

                Cube cube = cubeObj.GetComponent<Cube>();
                grid[col, row] = cube;
                cube.GridPosition.pos = new Vector2(col, row);

                cube.ColorChanger.AssignRandomColor();
                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.TearDrop);
            }
        }

    }

    void PopCube(Cube cube)
    {
        List<Cube> matchingCubes = GetMatchingCubes(cube);
        int matchedCubeCount = matchingCubes.Count;

        if(matchedCubeCount < 1) return;

        int[] removedCubes = new int[gridWidth];

        // Remove this cube
        if(matchedCubeCount < ROCKET_MATCH_COUNT - 1)
        {
            removedCubes[cube.GridPosition.x]++;
            RemoveSlotFromGrid(cube);
        }
        // Turn it into a bomb
        else
        {
            SwapCubeWithBomb(cube);
        }

        foreach(Cube _cube in matchingCubes)
        {
            removedCubes[_cube.GridPosition.x]++;
            RemoveSlotFromGrid(_cube);
        }

        SpawnNewCubesOnColumns(removedCubes);
        SetShapesOfMatchingGroups();
    }

    void SwapCubeWithBomb(Cube cube)
    {
        Debug.Log("Swap with bomb");
        GameObject bombObj = Instantiate(bombPrefab
                                         ,cube.transform.position, Quaternion.identity, cube.transform.parent);
        Bomb bomb = bombObj.GetComponent<Bomb>();
        bomb.GridPosition = cube.GridPosition;
        bomb.transform.SetSiblingIndex(cube.transform.GetSiblingIndex());


        grid[cube.GridPosition.x, cube.GridPosition.y] = bomb;
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
                int xCoord = bomb.GridPosition.x + xCoordDelta;
                int yCoord = bomb.GridPosition.y + yCoordDelta;
                if(IsInsideGrid(xCoord, yCoord))
                {
                    removedSlots[xCoord]++;
                    boardObjectsToRemove.Add(grid[xCoord, yCoord]);
                }
            }
        }
        foreach(BoardObject obj in boardObjectsToRemove)
        {
            RemoveSlotFromGrid(obj);
        }
        SpawnNewCubesOnColumns(removedSlots);
        SetShapesOfMatchingGroups();
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

        int yCoord = cube.GridPosition.y;
        int xCoord = cube.GridPosition.x;

        memoVisited[xCoord, yCoord] = true;

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if(Mathf.Abs(i + j) != 1) continue;

                if(IsInsideGrid(xCoord + i, yCoord + j))
                {
                    Cube cubeBeingVisited = grid[xCoord + i, yCoord + j] as Cube;
                    if(!memoVisited[xCoord + i, yCoord + j]
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
    }

    void SpawnNewCubesOnColumns(int[] cubeAmounts)
    {
        for(int i = 0; i < cubeAmounts.Length; i++)
        {
            int cubeNumToInsantiate = cubeAmounts[i];
            Vector3 colPos = transform.position + Vector3.left * ((gridWidth - 1) / 2f - i);
            Vector3 newCubePos;

            for(int j = 0; j < cubeNumToInsantiate; j++)
            {
                newCubePos = colPos + Vector3.up * ((gridHeight - 1) / 2f + j);
                GameObject cubeObj = Instantiate(cubePrefab, newCubePos, Quaternion.identity);
                cubeObj.transform.SetParent(columns[i].transform);
                cubeObj.transform.SetAsLastSibling();

                Cube cube = cubeObj.GetComponent<Cube>();
                grid[i, gridHeight - cubeNumToInsantiate + j] = cube;
                cube.GridPosition.pos = new Vector2(i, gridHeight - cubeNumToInsantiate + j);

                cube.ColorChanger.AssignRandomColor();
                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.TearDrop);
            }
        }
    }

    bool IsInsideGrid(int x, int y)
    {
        return (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight);
    }
}
