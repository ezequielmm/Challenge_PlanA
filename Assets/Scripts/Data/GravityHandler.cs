using System.Collections.Generic;
using UnityEngine;

namespace ChallengePlanA.Data
{
    /// <summary>
    /// Handles column collapse and filling empty cells after blocks are removed.
    /// Works column by column: shifts blocks down, then fills empty top cells.
    /// </summary>
    public static class GravityHandler
    {
        /// <summary>
        /// Process gravity on the whole grid. Blocks fall down to fill gaps,
        /// then empty cells at top get new random blocks.
        /// Returns list of cells that got new blocks (for the view to update).
        /// </summary>
        public static List<Vector2Int> ApplyGravity(GridModel grid)
        {
            List<Vector2Int> newCells = new List<Vector2Int>();

            for (int x = 0; x < grid.Columns; x++)
            {
                CollapseColumn(grid, x);
                FillColumn(grid, x, newCells);
            }

            return newCells;
        }

        // shift all non-empty blocks down in a column
        private static void CollapseColumn(GridModel grid, int x)
        {
            int writeY = 0;

            for (int readY = 0; readY < grid.Rows; readY++)
            {
                if (!grid.IsEmpty(x, readY))
                {
                    if (writeY != readY)
                    {
                        grid.SetBlock(x, writeY, grid.GetBlock(x, readY));
                        grid.ClearBlock(x, readY);
                    }
                    writeY++;
                }
            }
        }

        // fill remaining empty cells at the top with random blocks
        private static void FillColumn(GridModel grid, int x, List<Vector2Int> newCells)
        {
            for (int y = 0; y < grid.Rows; y++)
            {
                if (grid.IsEmpty(x, y))
                {
                    grid.FillCell(x, y);
                    newCells.Add(new Vector2Int(x, y));
                }
            }
        }
    }
}
