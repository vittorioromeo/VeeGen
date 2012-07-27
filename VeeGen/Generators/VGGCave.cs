namespace VeeGen.Generators
{
    public class VGGCave : VGGenerator
    {
        public VGGCave(int mValuePassable = 0, int mValueSolid = 1, int mInitialSolidPercent = 40, int mRequiredSolidToStarve = 3, int mRequiredSolidToSolidfy = 5, int mIterations = 1)
        {
            ValuePassable = mValuePassable;
            ValueSolid = mValueSolid;
            InitialSolidPercent = mInitialSolidPercent;
            RequiredSolidToStarve = mRequiredSolidToStarve;
            RequiredSolidToSolidify = mRequiredSolidToSolidfy;
            Iterations = mIterations;
        }
        public int ValuePassable { get; set; }
        public int ValueSolid { get; set; }
        public int InitialSolidPercent { get; set; }
        public int RequiredSolidToStarve { get; set; }
        public int RequiredSolidToSolidify { get; set; }
        public int Iterations { get; set; }

        public override void Generate(VGArea mArea)
        {
            // Randomly put solid tiles in the world
            int solidTilesAmount = (mArea.Width*mArea.Height)/100*InitialSolidPercent;
            for (int i = 0; i < solidTilesAmount; i++) mArea.GetRandomTile().Set(ValueSolid);

            // Iterate generation
            for (int iteration = 0; iteration < Iterations; iteration++)
            {
                // Create a clone of the world's map
                VGArea areaClone = mArea.Clone();

                // Cellar automata rule 4-5
                for (int iY = 0; iY < areaClone.Height; iY++)
                {
                    for (int iX = 0; iX < areaClone.Width; iX++)
                    {
                        if (areaClone.GetTileNeighborsCountByValue(iX, iY, 1, ValueSolid) <= RequiredSolidToStarve) mArea[iX, iY].Set(ValuePassable);
                        else if (areaClone.GetTileNeighborsCountByValue(iX, iY, 1, ValueSolid) >= RequiredSolidToSolidify) mArea[iX, iY].Set(ValueSolid);
                    }
                }
            }
        }
    }
}