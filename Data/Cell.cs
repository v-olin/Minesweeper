using System;

namespace Minesweeper.Data {
    public enum BoxType {
        Empty,
        Bomb,
        Neighbour
    }

    public class Cell {
        public BoxType Type { get; set; } // is BoxType.Emty by default
        public bool Revealed { get; set; }
        public int NeighbouringBombs { get; set; }

        public string SquareContent
        {
            get
            {
                switch (Type) {
                    case BoxType.Bomb: return "B";
                    case BoxType.Neighbour: return NeighbouringBombs.ToString();
                    default: return "";
                }
            }
        }
    }
}