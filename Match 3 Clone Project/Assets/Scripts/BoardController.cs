using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{

    public static BoardController Instance;

    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    uint width;
    [SerializeField]
    uint height;



    Cube[,] grid;

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
        InitCubes();
        foreach(Cube cube in grid)
        {
            if(GetNeighbourCubes(cube).Count > 1)
            {
                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.Rocket);
            }
        }
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
                Cube cubeHit = hit.collider.GetComponent<Cube>();
                PopCube(cubeHit);
            }
        }
    }

    void InitCubes()
    {
        grid = new Cube[height, width];

        Vector3 rowPos;
        Vector3 cubePos;


        for(int row = 0; row < height; row++)
        {
            GameObject newRow = new GameObject("Row " + (row + 1));
            newRow.transform.SetParent(transform);
            rowPos = transform.position + Vector3.up * ((height - 1) / 2f - row);
            newRow.transform.position = rowPos;

            for(int col = 0; col < width; col++)
            {
                cubePos = rowPos + Vector3.left * ((width - 1) / 2f - col);
                GameObject cubeObj = Instantiate(cubePrefab, cubePos, Quaternion.identity);
                cubeObj.transform.SetParent(newRow.transform);

                Cube cube = cubeObj.GetComponent<Cube>();
                grid[row, col] = cube;
                cube.ColorChanger.AssignRandomColor();

                cube.ShapeDrawer.SetShape(ShapeDrawer.Shape.TearDrop);
            }
        }

    }

    void PopCube(Cube cube)
    {
        List<Cube> neighbours = GetNeighbourCubes(cube);
        foreach(Cube _cube in neighbours)
        {
            Destroy(_cube.gameObject);
        }
        Destroy(cube.gameObject);
    }

    List<Cube> GetNeighbourCubes(Cube cube)
    {
        List<Cube> neighbours = new List<Cube>();
        bool[,] visited = new bool[grid.GetLength(0), grid.GetLength(1)];

        GetNeighbourCubesHelper(cube, ref neighbours, ref visited);
        return neighbours;
    }

    void GetNeighbourCubesHelper(Cube cube, ref List<Cube> memoNeighbours, ref bool[,] memoVisited)
    {

        Vector2 coordinates = GetCubeCoordinates(cube);
        int yCoord = (int)coordinates.x;
        int xCoord = (int)coordinates.y;

        memoVisited[yCoord, xCoord] = true;

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if(Mathf.Abs(i + j) != 1) continue;

                if(IsInsideGrid(xCoord + i, yCoord + j))
                {
                    if(!memoVisited[yCoord + j, xCoord + i] && grid[yCoord + j, xCoord + i].ColorChanger.cubeColor
                        == cube.ColorChanger.cubeColor)
                    {
                        memoNeighbours.Add(grid[yCoord + j, xCoord + i]);
                        GetNeighbourCubesHelper(grid[yCoord + j, xCoord + i], ref memoNeighbours, ref memoVisited);
                    }
                }
            }
        }
    }

    bool IsInsideGrid(int x, int y)
    {
        return (x >= 0 && x < grid.GetLength(1) && y >= 0 && y < grid.GetLength(0));
    }

    Vector2 GetCubeCoordinates(Cube cube)
    {
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = 0; j < grid.GetLength(1); j++)
            {
                if(grid[i,j].Equals(cube))
                {
                    return new Vector2(i, j);
                }
            }
        }
        return Vector2.negativeInfinity;
    }
}
