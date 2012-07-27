#region
using System;
using System.Collections.Generic;
using System.Linq;
using VeeGen.Pathfinding;

#endregion
namespace VeeGen.Generators
{
    public sealed class VGGBSPDungeon : VGGenerator
    {
        public VGGBSPDungeon(int mValueRoom = 0, int mValuePath = 0, int mValueSolid = 1, int mSplits = 2, int mMinimumSplitDistance = 4, int mSplitOffset = 1,
                             bool mIsConnected = true, bool mIsCarved = true, int mCarveOffset = 0, int mRemoveChancePercent = 5, bool mIsBorder = true)
        {
            ValueRoom = mValueRoom;
            ValuePath = mValuePath;
            ValueSolid = mValueSolid;
            Splits = mSplits;
            MinimumSplitDistance = mMinimumSplitDistance;
            SplitOffset = mSplitOffset;
            FinalAreas = new List<VGArea>();
            IsConnected = mIsConnected;
            IsCarved = mIsCarved;
            CarveOffset = mCarveOffset;
            RemoveChancePercent = mRemoveChancePercent;
            IsBorder = mIsBorder;
        }

        public bool IsRandomizedConnectionOrder { get; set; }
        public int ValuePath { get; set; }
        public int ValueRoom { get; set; }
        public int ValueSolid { get; set; }
        public int Splits { get; set; }
        public int SplitOffset { get; set; }
        public List<VGArea> FinalAreas { get; set; }
        public int MinimumSplitDistance { get; set; }
        public bool IsConnected { get; set; }
        public bool IsCarved { get; set; }
        public int CarveOffset { get; set; }
        public int RemoveChancePercent { get; set; }
        public bool IsBorder { get; set; }

        public override void Generate(VGArea mArea)
        {
            FinalAreas.Add(mArea.Clone());

            for (int i = 0; i < Splits; i++) FinalAreas = Split(FinalAreas, i%2 == 0, i == Splits - 1);

            List<VGArea> toRemove = FinalAreas.Where(t => VGUtils.GetRandomInt(0, 100) <= RemoveChancePercent).ToList();

            foreach (VGArea area in toRemove) FinalAreas.Remove(area);

            if (IsBorder) foreach (VGArea area in FinalAreas) foreach(VGTile tile in area.GetBorderTiles()) tile.Set(ValueSolid, false);

            if (IsCarved) foreach (VGArea area in FinalAreas) 
                for (int iY = 1 + CarveOffset; iY < area.Height - 1 - CarveOffset; iY++)
                    for (int iX = 1 + CarveOffset; iX < area.Width - 1 - CarveOffset; iX++) area[iX, iY].Set(ValueRoom);

            if (IsConnected) for (int index = 0; index < FinalAreas.Count - 1; index++) Connect(FinalAreas[index], FinalAreas[index + 1]);

            mArea.SetBorder(1);
        }

        public List<VGArea> Split(List<VGArea> mAreas, bool mHorizontal, bool mApplySplitOffset)
        {
            List<VGArea> result = new List<VGArea>();

            foreach (VGArea area in mAreas)
            {
                int splitX;
                splitX = (area.XEnd - area.XStart)/2 > MinimumSplitDistance ? VGUtils.Random.Next(area.XStart + ((area.XEnd - area.XStart)/2) - MinimumSplitDistance, area.XStart + ((area.XEnd - area.XStart)/2) + MinimumSplitDistance) : VGUtils.Random.Next(area.XStart, area.XEnd);

                int splitY;
                splitY = (area.YEnd - area.YStart)/2 > MinimumSplitDistance ? VGUtils.Random.Next(area.YStart + ((area.YEnd - area.YStart)/2) - MinimumSplitDistance, area.YStart + ((area.YEnd - area.YStart)/2) + MinimumSplitDistance) : VGUtils.Random.Next(area.YStart, area.YEnd);

                int offset = 0;
                if (mApplySplitOffset) offset = SplitOffset;

                VGArea split1, split2;

                if (mHorizontal)
                {
                    if ((area.XEnd - SplitOffset) - (area.XStart + SplitOffset) <= SplitOffset) offset = 0;
                    if ((splitY - SplitOffset) - (area.YStart + SplitOffset) <= SplitOffset) offset = 0;
                    if ((splitY + SplitOffset) - (area.YEnd - SplitOffset) <= SplitOffset) offset = 0;

                    split1 = new VGArea(area.World, area.XStart + offset, area.YStart + offset, area.XEnd - offset, splitY - offset);
                    if (area.World.WorldArea.Contains(area.XStart, splitY - 1)) splitY--;
                    split2 = new VGArea(area.World, area.XStart + offset, splitY + offset, area.XEnd - offset, area.YEnd - offset);                  
                }
                else
                {
                    if ((splitX - offset) - (area.XStart + SplitOffset) <= SplitOffset) offset = 0;
                    if ((area.XEnd - SplitOffset) - (splitX + SplitOffset) <= SplitOffset) offset = 0;
                    if ((area.YEnd - SplitOffset) - (area.YStart + SplitOffset) <= SplitOffset) offset = 0;

                    split1 = new VGArea(area.World, area.XStart + offset, area.YStart + offset, splitX - offset, area.YEnd - offset);
                    if (area.World.WorldArea.Contains(splitX - 1, area.YStart)) splitX--;
                    split2 = new VGArea(area.World, splitX + offset, area.YStart + offset, area.XEnd - offset, area.YEnd - offset);
                }

                result.Add(split1);
                result.Add(split2);
            }

            return result;
        }

        public void Connect2(VGArea mAreaStart, VGArea mAreaEnd)
        {
            VGTile start = mAreaStart.GetRandomBorderTile();
            VGTile end = mAreaEnd.GetRandomBorderTile();

            start.Set(ValuePath, true);
            end.Set(ValuePath, true);

            Path path = PGPathfinder.FindPath(mAreaStart.World.WorldArea, start.Node, end.Node);

            if (path == null) return;

            List<PGNode> nodes = new List<PGNode>();

            for (Path p = path; p != null; p = p.PreviousSteps)
                nodes.Add(p.LastStep);

            foreach (PGNode node in nodes)
            {
                node.Tile.Set(ValuePath);
                //foreach (VGTile tile in mAreaStart.World.WorldArea.GetTileNeighbors(node.X, node.Y)) if (tile.Value == 0) tile.Set(ValueSolid, false);
            }            
        }

        public void Connect(VGArea mAreaStart, VGArea mAreaEnd)
        {
            int mStartCX = mAreaStart.XStart + mAreaStart.Width/2;
            int mStartCY = mAreaStart.YStart + mAreaStart.Height/2;
            int mEndCX = mAreaEnd.XStart + mAreaEnd.Width/2;
            int mEndCY = mAreaEnd.YStart + mAreaEnd.Height/2;

            if (Math.Abs(mEndCX - mStartCX) < Math.Abs(mEndCY - mStartCY))
            {
                if (mStartCX > mEndCX && mStartCY == mEndCY)
                {
                    mStartCX -= mAreaStart.Width/2;
                    mEndCX += mAreaEnd.Width/2;
                }

                if (mStartCX < mEndCX && mStartCY == mEndCY)
                {
                    mStartCX += mAreaStart.Width/2;
                    mEndCX -= mAreaEnd.Width/2;
                }
            }

            else
            {
                if (mStartCY > mEndCY && mStartCX == mEndCX)
                {
                    mStartCY -= mAreaStart.Height/2;
                    mEndCY += mAreaEnd.Height/2;
                }

                if (mStartCY < mEndCY && mStartCX == mEndCX)
                {
                    mStartCY += mAreaStart.Height/2;
                    mEndCY -= mAreaEnd.Height/2;
                }
            }

            int currentX = mStartCX;
            int currentY = mStartCY;

            mAreaStart.World.WorldArea[currentX, currentY].Set(ValuePath);

            if (Math.Abs(mEndCX - mStartCX) < Math.Abs(mEndCY - mStartCY))
            {
                while (currentX != mEndCX)
                {
                    if (currentX > mEndCX) currentX--;
                    else currentX++;

                    mAreaStart.World.WorldArea[currentX, currentY].Set(ValuePath);
                }

                while (currentY != mEndCY)
                {
                    if (currentY > mEndCY) currentY--;
                    else currentY++;

                    mAreaStart.World.WorldArea[currentX, currentY].Set(ValuePath);
                }
            }
            else
            {
                while (currentY != mEndCY)
                {
                    if (currentY > mEndCY) currentY--;
                    else currentY++;

                    mAreaStart.World.WorldArea[currentX, currentY].Set(ValuePath);
                }

                while (currentX != mEndCX)
                {
                    if (currentX > mEndCX) currentX--;
                    else currentX++;

                    mAreaStart.World.WorldArea[currentX, currentY].Set(ValuePath);
                }
            }
        }
    }
}