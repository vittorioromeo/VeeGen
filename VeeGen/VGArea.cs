#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VeeGen.Pathfinding;

#endregion
namespace VeeGen
{
    public class VGArea
    {
        public VGArea(VGWorld mWorld, int mXStart, int mYStart, int mXEnd, int mYEnd)
        {
            Debug.Assert(XStart >= 0 && XStart <= XEnd && XEnd < mWorld.Width);
            Debug.Assert(YStart >= 0 && YStart <= YEnd && YEnd < mWorld.Height);

            World = mWorld;
            XStart = mXStart;
            YStart = mYStart;
            XEnd = mXEnd;
            YEnd = mYEnd;
            Tiles = new VGTile[Width,Height];

            for (int iY = 0; iY < Height; iY++) 
                for (int iX = 0; iX < Width; iX++) 
                    Tiles[iX, iY] = World.Tiles[iX + XStart, iY + YStart];
        }

        public VGWorld World { get; private set; }
        public VGTile[,] Tiles { get; private set; }
        public int XStart { get; private set; }
        public int YStart { get; private set; }
        public int XEnd { get; private set; }
        public int YEnd { get; private set; }
        public int Width
        {
            get { return XEnd - XStart; }
        }
        public int Height
        {
            get { return YEnd - YStart; }
        }

        public VGTile this[int mX, int mY]
        {
            get
            {
                Debug.Assert(mX >= 0 && mX < Width);
                Debug.Assert(mY >= 0 && mY < Height);

                return Tiles[mX, mY];
            }
        }

        public List<VGTile> GetBorderTiles(int mOffset = 0)
        {
            Debug.Assert(mOffset >= 0);
            Debug.Assert(Width - mOffset >= mOffset);
            Debug.Assert(Height - mOffset >= mOffset);

            List<VGTile> result = new List<VGTile>();

            if (Width - mOffset - 1 <= 0 || Height - mOffset - 1 <= 0) return result;

            for (int iX = mOffset; iX < Width - mOffset; iX++)
            {
                result.Add(Tiles[iX, 0]);
                result.Add(Tiles[iX, Height - mOffset - 1]);
            }

            for (int iY = mOffset; iY < Height - mOffset; iY++)
            {
                result.Add(Tiles[0, iY]);
                result.Add(Tiles[Width - mOffset - 1, iY]);
            }

            return result;
        }
        public void SetBorder(int mValue, int mOffset = 0)
        {
            foreach(VGTile tile in GetBorderTiles(mOffset)) tile.Set(mValue);
                  
        }
        public void Clear(int mValue) { for (int iY = 0; iY < Height; iY++) for (int iX = 0; iX < Width; iX++) Tiles[iX + XStart, iY + YStart].Set(mValue); }

        public bool Contains(int mX, int mY) { return mX >= 0 && mX < Width && mY >= 0 && mY < Height; }
        public VGTile GetRandomTile(int mOffset = 0)
        {
            Debug.Assert(mOffset >= 0);
            Debug.Assert(Width - mOffset >= mOffset);
            Debug.Assert(Height - mOffset >= mOffset);

            return Tiles[VGUtils.GetRandomInt(mOffset, Width - mOffset), VGUtils.GetRandomInt(mOffset, Height - mOffset)];
        }

        public List<VGTile> GetTileNeighbors(int mX, int mY, int mRadius = 1)
        {
            Debug.Assert(mRadius > 0);

            List<VGTile> result = new List<VGTile>();

            for (int iY = -mRadius; iY < mRadius + 1; iY++) 
                for (int iX = -mRadius; iX < mRadius + 1; iX++) 
                if (Contains(mX + iX, mY + iY))
                    if (iX != mX || iY != mY) 
                        result.Add(Tiles[mX + iX, mY + iY]);

            return result;
        }
        public int GetTileCountByValue(List<VGTile> mTiles, int mValue) { return mTiles.Count(tile => tile.Value == mValue); }
        public int GetTileNeighborsCountByValue(int mX, int mY, int mValue, int mRadius = 1)
        {
            Debug.Assert(mRadius > 0);

            return GetTileCountByValue(GetTileNeighbors(mX, mY, mRadius), mValue);
        }
        public VGTile GetRandomBorderTile(int mOffset = 0)
        {
            Debug.Assert(mOffset >= 0);
            Debug.Assert(Width - mOffset >= mOffset);
            Debug.Assert(Height - mOffset >= mOffset);

            List<VGTile> tiles = new List<VGTile>();

            if (Width - mOffset - 1 <= 0 || Height - mOffset - 1 <= 0) return null;

            for (int iX = mOffset; iX < Width - mOffset; iX++)
            {
                tiles.Add(Tiles[iX, 0]);
                tiles.Add(Tiles[iX, Height - mOffset - 1]);
            }

            for (int iY = mOffset; iY < Height - mOffset; iY++)
            {
                tiles.Add(Tiles[0, iY]);
                tiles.Add(Tiles[Width - mOffset - 1, iY]);
            }

            return tiles.Count == 0 ? null : tiles[VGUtils.GetRandomInt(0, tiles.Count)];
        }
        public List<PGNode> GetTileNodeNeighbors(int mX, int mY, int mRadius = 1)
        {
            Debug.Assert(mRadius > 0);

            List<PGNode> result = new List<PGNode>();

            for (int iY = -mRadius; iY < mRadius + 1; iY++)
                for (int iX = -mRadius; iX < mRadius + 1; iX++)
                    if (Contains(mX + iX, mY + iY))
                        if (iX != mX || iY != mY)
                            result.Add(Tiles[mX + iX, mY + iY].Node);

            return result;
        }

        public VGArea Clone()
        {
            VGArea result = new VGArea(World, XStart, YStart, XEnd, YEnd);

            for (int iY = 0; iY < Height; iY++) for (int iX = 0; iX < Width; iX++) result.Tiles[iX, iY] = Tiles[iX, iY].Clone();

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int iY = 0; iY < Height; iY++)
            {
                for (int iX = 0; iX < Width; iX++)
                {
                    switch (Tiles[iX, iY].Value)
                    {
                        case 0:
                            sb.Append(" ");
                            break;
                        case 1:
                            sb.Append("░");
                            break;
                        case 2:
                            sb.Append("█");
                            break;
                        case 3:
                            sb.Append(" ");
                            break;
                        case 4:
                            sb.Append(" ");
                            break;
                    }
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
        public int[,] ToArray()
        {
            int[,] result = new int[Width, Height];

            for (int iY = 0; iY < Height; iY++) for (int iX = 0; iX < Width; iX++) result[iX, iY] = Tiles[iX, iY].Value;

            return result;
        }
    }
}