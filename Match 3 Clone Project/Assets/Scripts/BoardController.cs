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
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material greenMaterial;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material yellowMaterial;


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

                grid[row, col] = cubeObj.GetComponent<Cube>();

                int random = Random.Range(0, 4);
                Material mat = null;
                Cube.CubeColor cubeColor = Cube.CubeColor.Blue;

                switch(random)
                {
                    case 0:
                        mat = blueMaterial;
                        cubeColor = Cube.CubeColor.Blue;
                        break;
                    case 1:
                        mat = redMaterial;
                        cubeColor = Cube.CubeColor.Red;
                        break;
                    case 2:
                        mat = greenMaterial;
                        cubeColor = Cube.CubeColor.Green;
                        break;
                    case 3:
                        mat = yellowMaterial;
                        cubeColor = Cube.CubeColor.Yellow;
                        break;
                }
                grid[row, col].MyColor = cubeColor;
                cubeObj.GetComponent<MeshRenderer>().material = mat;
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

        // Check right
        if(xCoord + 1 < grid.GetLength(1))
        {
            if(!memoVisited[yCoord, xCoord + 1] && grid[yCoord, xCoord + 1].MyColor == cube.MyColor)
            {
                memoNeighbours.Add(grid[yCoord, xCoord + 1]);
                GetNeighbourCubesHelper(grid[yCoord, xCoord + 1], ref memoNeighbours, ref memoVisited);
            }
        }
           

        // Check left
        if(xCoord - 1 >= 0)
        {
            if(!memoVisited[yCoord, xCoord - 1] && grid[yCoord, xCoord - 1].MyColor == cube.MyColor)
            {
                memoNeighbours.Add(grid[yCoord, xCoord - 1]);
                GetNeighbourCubesHelper(grid[yCoord, xCoord - 1], ref memoNeighbours, ref memoVisited);
            }
        }

        // Check top
        if(yCoord - 1 >= 0)
        {
            if(!memoVisited[yCoord - 1, xCoord] && grid[yCoord - 1, xCoord].MyColor == cube.MyColor)
            {
                memoNeighbours.Add(grid[yCoord - 1, xCoord]);
                GetNeighbourCubesHelper(grid[yCoord - 1, xCoord], ref memoNeighbours, ref memoVisited);
            }
        }

        // Check bottom
        if(yCoord + 1 < grid.GetLength(0))
        {
            if(!memoVisited[yCoord + 1, xCoord] && grid[yCoord + 1, xCoord].MyColor == cube.MyColor)
            {
                memoNeighbours.Add(grid[yCoord + 1, xCoord]);
                GetNeighbourCubesHelper(grid[yCoord + 1, xCoord], ref memoNeighbours, ref memoVisited);
            }
        }
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
