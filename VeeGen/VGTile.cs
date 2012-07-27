using System.Diagnostics;
using VeeGen.Pathfinding;

namespace VeeGen
{
    public class VGTile
    {
        internal VGTile(int mX, int mY, int mValue, bool mPassable = true)
        {
            Debug.Assert(X >= 0);
            Debug.Assert(Y >= 0);

            X = mX;
            Y = mY;
            Value = mValue;
            Passable = mPassable;
            Node = new PGNode(this);
        }

        public int Value { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public PGNode Node { get; set; }
        public bool Passable { get; set; }

        public void Set(int mValue, bool mPassable = true)
        {
            Value = mValue;
            Passable = mPassable;
        }

        public VGTile Clone() { return new VGTile(X, Y, Value, Passable); }
    }
}