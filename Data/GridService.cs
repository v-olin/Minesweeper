using System;
using System.Collections.Generic;
using System.Linq;
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
                    // each square has a 8% chance of becoming a bomb
                    var cell = new Cell();
                    var bombPoss = rnd.Next(100);
                    if (bombPoss < 8)
                        cell.Type = BoxType.Bomb;
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
                    if (grid[i, j].Type != BoxType.Bomb)
                        PopulateNeighbour(ref grid, i, j);
                }
            }
        }

        private void PopulateNeighbour(ref Cell[,] grid, int i, int j)
        {
            var nearBombs = 0;

            (int b, int f)[] offsets = Perimeter(i, j, grid.GetLength(0), grid.GetLength(1));

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

        public void FindNearbyCells(ref Cell[,] grid, ref List<(int i, int j)> emptyNeighbours, int i, int j)
        {
            emptyNeighbours.Add((i, j));

            (int b, int f)[] offsets = Perimeter(i, j, grid.GetLength(0), grid.GetLength(1));

            for (int x = offsets[0].b; x <= offsets[0].f; x++)
            {
                for (int y = offsets[1].b; y <= offsets[1].f; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        int ip = i + x, jp = j + y;
                        if (grid[ip, jp].Type == BoxType.Empty)
                        {
                            if (!emptyNeighbours.Contains((ip, jp)))
                            {
                                emptyNeighbours.Add((ip, jp));
                                FindNearbyCells(ref grid, ref emptyNeighbours, ip, jp);
                            }
                        }
                    }
                }
            }
        }
    }
}