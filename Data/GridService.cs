using System;
using System.Threading.Tasks;

namespace Minesweeper.Data
{
    public class GridService
    {
        private static readonly int hrows = 50;
        private static readonly int vrows = 30;

        public Task<Cell[,]> GetGrid()
        {
            var rnd = new Random();
            var grid = new Cell[hrows, vrows];
            for (int i = 0; i < hrows; i++)
            {
                for (int j = 0; j < vrows; j++)
                {
                    grid[i, j].NeighbouringBombs = rnd.Next(10);
                }
            }
            return Task.FromResult(
                grid
            );
        }
    }
}