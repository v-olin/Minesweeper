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
            var grid = new Cell[vrows, hrows];
            PopulateGridWithBombs(ref grid, rnd);
            FillRemainingGrid(ref grid);
            return Task.FromResult(
                grid
            );
        }

        private void PopulateGridWithBombs(ref Cell[,] grid, Random rnd)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // 36/(50*30)=0,024 - estimated 36 bombs on full grid
                    // each square has a 2.4% chance of being a bomb
                    var cell = new Cell();
                    var bombPoss = rnd.Next(1000);
                    if (bombPoss < 25)
                    {
                        cell.Type = BoxType.Bomb;
                    }
                    grid[i, j] = cell;
                }
            }
        }

        private void FillRemainingGrid(ref Cell[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    PopulateNeighbour(ref grid, i, j);
                }
            }
        }

        private void PopulateNeighbour(ref Cell[,] grid, int i, int j)
        {
            var nearBombs = 0;

            (int b, int f)[] offsets = Perimeter(i, j, grid.GetLength(0), grid.GetLength(1));

            try
            {
                for (int x = offsets[0].b; x <= offsets[0].f; x++)
                {
                    for (int y = offsets[1].b; y <= offsets[1].f; y++)
                    {
                        var xp = i + x; //xprime
                        var yp = j + y; //yprime

                        if (!(xp == i && yp == j))
                            if (grid[xp, yp].Type == BoxType.Bomb)
                                nearBombs++;
                    }
                }
            }
            catch (Exception){}

            if (nearBombs != 0)
            {
                grid[i, j].Type = BoxType.Neighbour;
                grid[i, j].NeighbouringBombs = nearBombs;
            }
        }

        private (int b, int f)[] Perimeter(int i, int j, int maxh, int maxw) // i = height offset, j = width offset
        {
            (int h, int w)[] offset = { (0, 0), (0, 0) }; //[0] = height offset, [1] = width offset

            if (i == 0) offset[0] = (0, 1); //0 steps up, 1 step up
            else if (i == maxh - 1) offset[0] = (-1, 0);
            else offset[0] = (-1, 1);

            if (j == 0) offset[1] = (0, 1);
            else if (j == maxw - 1) offset[1] = (-1, 0);
            else offset[1] = (-1, 1);

            return offset;
        }
    }
}