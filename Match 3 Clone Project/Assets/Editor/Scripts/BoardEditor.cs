using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardController))]
public class BoardEditor : Editor 
{
    public enum ItemType { CubeBlue, CubeRed, CubeGreen, CubeYellow, WoodenBox}

    BoardController boardController;

    Vector2Int itemPos;
    ItemType itemType;

    void OnEnable()
    {
        boardController = (BoardController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Create Board"))
        {
            if(boardController.BoardHeight > 0 && boardController.BoardWidth > 0)
            {
                boardController.InitBoard();
            }
            else
            {
                Debug.LogWarning("Cannot create board with zero or lower dimensions!");
            }
        }

        if(GUILayout.Button("Fill Board"))
        {
            if(boardController.transform.childCount > 0)
            {
                ClearAllCubes();
                FillBoard();
            }
            else{
                Debug.LogWarning("You must create the board before filling it!");
            }
        }

        if(GUILayout.Button("Remove Board"))
        {
            boardController.RemoveBoard();
        }

        itemType = (ItemType)EditorGUILayout.EnumPopup("Primitive to create:", itemType);
        itemPos = EditorGUILayout.Vector2IntField("Position to Set:", itemPos);

        if(GUILayout.Button("Set Item At Position"))
        {
            if(boardController.IsInsideBoard(itemPos))
            {
                Debug.Log("Set " + itemType.ToString() + " at pos: " + itemPos);
            }
            else
            {
                Debug.LogWarning(itemPos + " is not inside the board!");
            }
        }
    }

    /// <summary>
    /// Fills the board with cubes.
    /// </summary>
    void FillBoard()
    {

        int boardWidth = boardController.BoardWidth;
        int boardHeight = boardController.BoardHeight;

        Debug.Log("Columns: " + BoardController.Instance.Columns);

        // Spawn cubes on columns
        for(int x = 0; x < boardWidth; x++)
        {
            Transform currentColumn = BoardController.Instance.Columns[x].transform;
            Vector3 colPos = currentColumn.position;
            Vector3 newCubeWorldPos;
            Vector2Int newCubeGridPos;

            for(int y = 0; y < boardHeight; y++)
            {
                newCubeWorldPos = colPos + Vector3.up * ((boardHeight - 1) / 2f + y);
                newCubeGridPos = new Vector2Int(x, y);
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
    private void SpawnRandomCube(Vector3 worldPos, Vector2Int gridPos, Transform parent)
    {
        Cube cube = SpawnCube(ColorController.ToonColor.Blue, worldPos, gridPos, parent);
        cube.GetComponent<ColorController>().AssignRandomColor();
    }

    private Cube SpawnCube(ColorController.ToonColor color, Vector3 worldPos, Vector2Int gridPos, Transform parent) {
        GameObject cubeObj = PrefabUtility.InstantiatePrefab(CubeSpawner.Instance.cubePrefab) as GameObject;
        cubeObj.transform.position = worldPos;
        cubeObj.transform.SetParent(parent);
        cubeObj.transform.SetAsLastSibling();

        Cube cube = cubeObj.GetComponent<Cube>();
        cube.GridPosition.pos = gridPos;
        cube.SetPositionLock(true);

        BoardController.Instance.AssignToBoard(cube);
        cube.GetComponent<ColorController>().SetColor(color);
        cube.GetComponent<ShapeController>().SetShapeToDefault();
        return cube;
    }

    void ClearAllCubes()
    {
        foreach(GameObject column in boardController.Columns)
        {
            foreach(BoardObject boardObject in column.GetComponentsInChildren<BoardObject>())
            {
                if(boardObject is Cube)
                {
                    DestroyImmediate(boardObject.gameObject);
                }
            }
        }
    }
}
