namespace VeeGen.Generators
{
    public class VGGOutliner : VGGenerator
    {
        public VGGOutliner(int mValuePassable = 0, int mValueSolid = 1, int mValueWall = 2)
        {
            ValuePassable = mValuePassable;
            ValueSolid = mValueSolid;
            ValueWall = mValueWall;
        }

        public int ValuePassable { get; set; }
        public int ValueSolid { get; set; }
        public int ValueWall { get; set; }

        public override void Generate(VGArea mArea)
        {
            // Create a clone of the world's map
            VGArea areaClone = mArea.Clone();

            // Set all solids to walls
            for (int iY = 0; iY < areaClone.Height; iY++) for (int iX = 0; iX < areaClone.Width; iX++) if (areaClone[iX, iY].Value == ValueSolid) mArea[iX, iY].Set(ValueWall);

            // Create a clone of the world's map
            areaClone = mArea.Clone();

            // Sets all walls with 9 wall neighbors to solid
            for (int iY = 0; iY < areaClone.Height; iY++) for (int iX = 0; iX < areaClone.Width; iX++) if (areaClone.GetTileNeighborsCountByValue(iX, iY, ValueWall) > 8) mArea[iX, iY].Set(ValueSolid);
        }
    }
}