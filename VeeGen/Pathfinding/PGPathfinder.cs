#region
using System.Collections;
using System.Collections.Generic;

#endregion

namespace VeeGen.Pathfinding
{
    public static class PGPathfinder
    {
        public static Path FindPath(VGArea mMap, PGNode mStart, PGNode mEnd)
        {
            foreach(VGTile tile in mMap.Tiles) tile.Node.Clear();

            var closed = new HashSet<PGNode>();
            var queue = new PriorityQueue<double, Path>();
            queue.Enqueue(0, new Path(mStart));
            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();
                if (!path.LastStep.Passable || closed.Contains(path.LastStep))
                    continue;
                if (path.LastStep.Equals(mEnd))
                {
                    return path;
                }
                closed.Add(path.LastStep);
                foreach (PGNode n in mMap.GetTileNodeNeighbors(path.LastStep.Tile.X, path.LastStep.Tile.Y))
                {
                    double d = PGUtils.GetNodeDiagonalDistance(path.LastStep, n);
                    var newPath = path.AddStep(n, d);
                    queue.Enqueue(newPath.TotalCost + PGUtils.GetNodeManhattanDistance(n, mEnd), newPath);
                }
            }
            return null;
        }
    }

    public class Path : IEnumerable
    {
        private Path(PGNode lastStep, Path previousSteps, double totalCost)
        {
            LastStep = lastStep;
            PreviousSteps = previousSteps;
            TotalCost = totalCost;
        }

        public Path(PGNode start)
            : this(start, null, 0)
        {
        }

        public PGNode LastStep { get; private set; }
        public Path PreviousSteps { get; private set; }
        public double TotalCost { get; private set; }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public Path AddStep(PGNode step, double stepCost)
        {
            return new Path(step, this, TotalCost + stepCost);
        }

        public IEnumerator GetEnumerator()
        {
            for (Path p = this; p != null; p = p.PreviousSteps)
                yield return p.LastStep;
        }
    }
}