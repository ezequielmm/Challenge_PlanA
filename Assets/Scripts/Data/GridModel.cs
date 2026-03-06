using UnityEngine;

namespace ChallengePlanA.Data
{
    /// <summary>
    /// Pure data representation of the grid. No MonoBehaviour dependency
    /// so its easy to test and keeps logic separated from visuals.
    /// </summary>
    public class GridModel
    {
        private int[,] _grid; // [col, row], -1 means empty
        private int _columns;
        private int _rows;
        private int _blockTypeCount;

        public int Columns => _columns;
        public int Rows => _rows;

        public GridModel(int columns, int rows, int blockTypeCount)
        {
            _columns = columns;
            _rows = rows;
            _blockTypeCount = blockTypeCount;
            _grid = new int[columns, rows];
        }

        public void FillRandom()
        {
            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    _grid[x, y] = Random.Range(0, _blockTypeCount);
                }
            }
        }

        public int GetBlock(int x, int y)
        {
            if (!IsInBounds(x, y)) return -1;
            return _grid[x, y];
        }

        public void SetBlock(int x, int y, int type)
        {
            if (IsInBounds(x, y))
                _grid[x, y] = type;
        }

        public void ClearBlock(int x, int y)
        {
            SetBlock(x, y, -1);
        }

        public bool IsEmpty(int x, int y)
        {
            return GetBlock(x, y) == -1;
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < _columns && y >= 0 && y < _rows;
        }

        // fills a single cell with a random type, used after gravity
        public int FillCell(int x, int y)
        {
            int type = Random.Range(0, _blockTypeCount);
            _grid[x, y] = type;
            return type;
        }
    }
}
