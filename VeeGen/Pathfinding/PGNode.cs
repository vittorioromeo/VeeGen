#region
using System;
using System.Collections.Generic;

#endregion

namespace VeeGen.Pathfinding
{
    public class PGNode
    {
        public PGNode(VGTile mTile)
        {
            Tile = mTile;
        }

        public VGTile Tile { get; set; }
        public int X { get { return Tile.X; } }
        public int Y { get { return Tile.Y; } }
        public double G { get; set; }
        public double H { get; set; }
        public double F
        {
            get { return G + H; }
        }
        public PGNode Parent { get; set; }
        public bool Passable { get { return Tile.Passable; } }
        public void Clear()
        {
            G = 0;
            H = 0;
            Parent = null;
        }
    }
}