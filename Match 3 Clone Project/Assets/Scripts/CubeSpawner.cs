using UnityEngine;

/// <summary>
/// Singleton Spawner class which is responsible for spawning new cubes.
/// </summary>
public class CubeSpawner : Singleton<CubeSpawner>
{
    static readonly float HEIGHT_OFFSET = 1.75f;

    [SerializeField] GameObject cubePrefab;

    /// <summary>
    /// Check if there are cubes needed to be spawn, spawn them if there are.
    /// </summary>
    /// <param name="board">Board.</param>
    public void SpawnNeededCubes(ref BoardObject[,] board)
    {
        // Determine cubes that have to be spawned.
        int boardWidth = board.GetLength(0);        
        int boardHeight = board.GetLength(1);

        int[] cubeAmountsForColums = new int[boardWidth];

        for(int x = 0; x < boardWidth; x++)
        {
            // Check the column from top to bottom.
            for(int y = boardHeight - 1; y >= 0; y--)
            {
                if(board[x,y] == null)
                {
                    cubeAmountsForColums[x]++;
                }
                else
                {
                    break;
                }
            }
        }

        // Spawn cubes on columns
        for(int x = 0; x < cubeAmountsForColums.Length; x++)
        {
            int numCubesToInstantiate = cubeAmountsForColums[x];

            Transform currentColumn = BoardController.Instance.Columns[x].transform;
            Vector3 colPos = currentColumn.position;
            Vector3 newCubeWorldPos;
            Vector2Int newCubeGridPos;

            for(int y = 0; y < numCubesToInstantiate; y++)
            {
                newCubeWorldPos = colPos + Vector3.up * ((boardHeight - 1) / 2f + y + HEIGHT_OFFSET);
                newCubeGridPos = new Vector2Int(x, boardHeight - numCubesToInstantiate + y);
                SpawnRandomCube(newCubeWorldPos, newCubeGridPos, currentColumn);
            }
        }
    }

    /// <summary>
    /// Spawns a cube and randomly color it.
    /// </summary>
    /// <param name="worldPos">World position.</param>
    /// <param name="gridPos">Grid position.</param>
    /// <param name="parent">Parent transform.</param>
    void SpawnRandomCube(Vector3 worldPos, Vector2Int gridPos, Transform parent)
    {
        GameObject cubeObj = Instantiate(cubePrefab, worldPos, Quaternion.identity);
        cubeObj.transform.SetParent(parent);
        cubeObj.transform.SetAsLastSibling();

        Cube cube = cubeObj.GetComponent<Cube>();
        cube.GridPosition.pos = gridPos;
        cube.SetPositionLock(true);
        BoardController.Instance.AssignToBoard(cube);

        cube.ColorChanger.AssignRandomColor();
        cube.ShapeDrawer.SetShape(ShapeController.Shape.TearDrop);
    }
}
