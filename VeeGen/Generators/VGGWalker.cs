#region
using System.Collections.Generic;

#endregion
namespace VeeGen.Generators
{
    public class VGGWalker : VGGenerator
    {
        public VGGWalker(int mValuePassable = 0, int mValueSolid = 1, int mWalkersAmount = 3, int mWalkerMinumumSteps = 20, int mWalkerMaximumSteps = 100, int mWalkerDeathChance = 5,
                         int mWalkerDirectionChangeChance = 15, bool mIsWrapped = true, bool mIsRoomCreatedOnDeath = true, int mRoomMinSize = 4, int mRoomMaxSize = 9, int mWalkerRadius = 1)
        {
            ValuePassable = mValuePassable;
            ValueSolid = mValueSolid;
            WalkerMinimumSteps = mWalkerMinumumSteps;
            WalkerMaximumSteps = mWalkerMaximumSteps;
            WalkerDeathChance = mWalkerDeathChance;
            WalkerDirectionChangeChance = mWalkerDirectionChangeChance;
            WalkerPaths = new List<VGGWalkerPath>();
            WalkersAmount = mWalkersAmount;
            IsWrapped = mIsWrapped;
            IsRoomCreatedOnDeath = mIsRoomCreatedOnDeath;
            RoomMinSize = mRoomMinSize;
            RoomMaxSize = mRoomMaxSize;
            WalkerRadius = mWalkerRadius;
        }
        public int ValuePassable { get; set; }
        public int ValueSolid { get; set; }
        public List<VGGWalkerPath> WalkerPaths { get; set; }
        public int WalkerMinimumSteps { get; set; }
        public int WalkerMaximumSteps { get; set; }
        public int WalkerDeathChance { get; set; }
        public int WalkersAmount { get; set; }
        public int WalkerDirectionChangeChance { get; set; }
        public bool IsWrapped { get; set; }
        public bool IsRoomCreatedOnDeath { get; set; }
        public int RoomMinSize { get; set; }
        public int RoomMaxSize { get; set; }
        public int WalkerRadius { get; set; }

        public override void Generate(VGArea mArea)
        {
            for (int i = 0; i < WalkersAmount; i++)
            {
                VGTile randomTile = mArea.GetRandomTile();
                WalkerPaths.Add(new VGGWalkerPath(randomTile.X, randomTile.Y, WalkerMinimumSteps, WalkerMaximumSteps, WalkerDeathChance, WalkerDirectionChangeChance));
            }

            bool running = true;

            while (running)
            {
                List<VGGWalkerPath> toRemove = new List<VGGWalkerPath>();
                running = false;

                foreach (VGGWalkerPath path in WalkerPaths)
                {
                    if (mArea.Contains(path.X, path.Y))
                    {
                        for (int iY = -WalkerRadius; iY < WalkerRadius + 1; iY++) for (int iX = -WalkerRadius; iX < WalkerRadius + 1; iX++) if (mArea.Contains(path.X + iX, path.Y + iY)) mArea[path.X + iX, path.Y + iY].Set(ValuePassable);
                    }
                    else if (IsWrapped)
                    {
                        if (path.X >= mArea.Width) path.X = 0;
                        else if (path.X < 0) path.X = mArea.Width - 1;

                        if (path.Y >= mArea.Height) path.Y = 0;
                        else if (path.Y < 0) path.Y = mArea.Height - 1;
                    }

                    path.Step();

                    if (path.Alive) running = true;
                    else if (IsRoomCreatedOnDeath)
                    {
                        toRemove.Add(path);

                        int roomWidth = VGUtils.GetRandomInt(RoomMinSize, RoomMaxSize);
                        int roomHeight = VGUtils.GetRandomInt(RoomMinSize, RoomMaxSize);

                        for (int iY = 0; iY < roomHeight; iY++) for (int iX = 0; iX < roomWidth; iX++) if (mArea.Contains(path.X - (roomWidth/2) + iX, path.Y - (roomHeight/2) + iY)) mArea[path.X - (roomWidth/2) + iX, path.Y - (roomHeight/2) + iY].Set(ValuePassable);
                    }
                }

                foreach (VGGWalkerPath pathToRemove in toRemove) WalkerPaths.Remove(pathToRemove);
            }
        }
        #region Nested type: VGGWalkerPath
        public class VGGWalkerPath
        {
            private int _nextX, _nextY, _steps;

            public VGGWalkerPath(int mX, int mY, int mWalkerMinumumSteps, int mWalkerMaximumSteps, int mWalkerDeathChance, int mWalkerDirectionChangeChance)
            {
                Alive = true;
                X = mX;
                Y = mY;
                WalkerMinimumSteps = mWalkerMinumumSteps;
                WalkerMaximumSteps = mWalkerMaximumSteps;
                WalkerDeathChance = mWalkerDeathChance;
                WalkerDirectionChangeChance = mWalkerDirectionChangeChance;

                _nextX = _nextY = 0;

                CalculateDirection();
            }

            public bool Alive { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int WalkerMinimumSteps { get; set; }
            public int WalkerMaximumSteps { get; set; }
            public int WalkerDeathChance { get; set; }
            public int WalkerDirectionChangeChance { get; set; }

            public void CalculateDirection()
            {
                int random = VGUtils.GetRandomInt(0, 4);

                switch (random)
                {
                    case 0:
                        _nextX = 1;
                        _nextY = 0;
                        break;
                    case 1:
                        _nextX = -1;
                        _nextY = 0;
                        break;
                    case 2:
                        _nextX = 0;
                        _nextY = 1;
                        break;
                    case 3:
                        _nextX = 0;
                        _nextY = -1;
                        break;
                }
            }

            public void Step()
            {
                if (VGUtils.GetRandomInt(0, 100) <= WalkerDirectionChangeChance) CalculateDirection();
                if ((_steps > WalkerMinimumSteps && VGUtils.GetRandomInt(0, 1000) < WalkerDeathChance) || _steps > WalkerMaximumSteps) Alive = false;
                X += _nextX;
                Y += _nextY;

                _steps++;
            }
        }
        #endregion
    }
}