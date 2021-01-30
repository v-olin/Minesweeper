using System;

namespace Minesweeper.Data {
    public enum BoxType {
        Bomb,
        Empty,
        Neighbour
    }

    public class Cell {
        public BoxType Type { get; set; }
        public bool Revealed { get; set; }
        public int NeighbouringBombs { get; set; }
    }
}