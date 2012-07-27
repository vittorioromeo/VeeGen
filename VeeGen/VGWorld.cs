using System.Diagnostics;

namespace VeeGen
{
    public class VGWorld
    {
        public VGWorld(int mWidth, int mHeight, int mValue)
        {
            Debug.Assert(mWidth > 0);
            Debug.Assert(mHeight > 0);

            Width = mWidth;
            Height = mHeight;
            Tiles = new VGTile[Width,Height];

            for (int iY = 0; iY < Height; iY++) for (int iX = 0; iX < Width; iX++) Tiles[iX, iY] = new VGTile(iX, iY, mValue);

            WorldArea = new VGArea(this, 0, 0, mWidth, mHeight);
        }

        public VGTile[,] Tiles { get; private set; }
        public VGArea WorldArea { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}