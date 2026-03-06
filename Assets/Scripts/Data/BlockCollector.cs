using System.Collections.Generic;
using UnityEngine;

namespace ChallengePlanA.Data
{
    /// <summary>
    /// BFS flood fill to find connected blocks of same color.
    /// I went with BFS over recursive DFS to avoid stack issues
    /// on bigger grids, and its easier to follow.
    /// </summary>
    public static class BlockCollector
    {
        private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        public static List<Vector2Int> GetConnectedBlocks(GridModel grid, int startX, int startY)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            int targetType = grid.GetBlock(startX, startY);
            if (targetType < 0) return result;

            bool[,] visited = new bool[grid.Columns, grid.Rows];
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            queue.Enqueue(new Vector2Int(startX, startY));
            visited[startX, startY] = true;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                result.Add(current);

                foreach (var dir in Directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;

                    if (!grid.IsInBounds(nx, ny)) continue;
                    if (visited[nx, ny]) continue;
                    if (grid.GetBlock(nx, ny) != targetType) continue;

                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }

            return result;
        }
    }
}
