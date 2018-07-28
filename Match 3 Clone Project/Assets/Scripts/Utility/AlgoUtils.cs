using UnityEngine;
using System.Collections.Generic;

public static class AlgoUtils
{

    /// <summary>
    /// Wrapper function for match detection algorithm.
    /// </summary>
    /// <returns> matching cubes with the given one.</returns>
    /// <param name="cube">Current cube.</param>
    public static List<Cube> GetMatchingCubes(Cube cube)
    {
        List<Cube> neighbours = new List<Cube>();
        bool[,] visited = new bool[BoardController.Instance.BoardWidth, BoardController.Instance.BoardHeight];

        GetMatchingNeighborsRecursive(cube, ref neighbours, ref visited);
        return neighbours;
    }

    /// <summary>
    /// Actual Flood and Fill Algorithm implementation.
    /// </summary>
    static void GetMatchingNeighborsRecursive(Cube cube, ref List<Cube> memoNeighbours, ref bool[,] memoVisited)
    {
        memoVisited[cube.GridPosition.x, cube.GridPosition.y] = true;

        for(int xDelta = -1; xDelta < 2; xDelta++)
        {
            for(int yDelta = -1; yDelta < 2; yDelta++)
            {
                // We only need to check 4-neighbors, not the corners nor the center.
                if(Mathf.Abs(xDelta + yDelta) != 1) continue;

                Vector2Int neighborPos = cube.GridPosition.pos + new Vector2Int(xDelta, yDelta);

                if(!BoardController.Instance.IsInsideBoard(neighborPos.x, neighborPos.y)) continue;

                Cube cubeBeingVisited = BoardController.Instance.GetBoardObjectAt(neighborPos.x, neighborPos.y) as Cube;
                if(!memoVisited[neighborPos.x, neighborPos.y]
                   && cubeBeingVisited != null
                   && cubeBeingVisited.ColorChanger.CurrentColor == cube.ColorChanger.CurrentColor)
                {
                    memoNeighbours.Add(cubeBeingVisited);
                    GetMatchingNeighborsRecursive(cubeBeingVisited, ref memoNeighbours, ref memoVisited);
                }
            }
        }
    }

}
