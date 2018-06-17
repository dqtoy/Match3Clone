using UnityEngine;

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
                Debug.Log(cubePos);
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
        foreach(Cube _cube in grid)
        {
            if(!_cube.Equals(cube) && _cube.MyColor.Equals(cube.MyColor))
            {
                Destroy(_cube.gameObject);
            }
        }
        Destroy(cube.gameObject);
    }
}
