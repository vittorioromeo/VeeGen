#region
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace VeeGen.Pathfinding
{
    public static class PGUtils
    {
        public static double GetNodeManhattanDistance(PGNode mStart, PGNode mEnd)
        {
            return Math.Max(Math.Abs(mStart.X - mEnd.X), Math.Abs(mStart.Y - mEnd.Y));
        }

        public static double GetNodeDiagonalDistance(PGNode mStart, PGNode mEnd)
        {
            return Math.Abs(mStart.X - mEnd.X) + Math.Abs(mStart.Y - mEnd.Y);
        }

        public static double GetNodeEuclideanDistance(PGNode mStart, PGNode mEnd)
        {
            return Math.Sqrt((mStart.X - mEnd.X) ^ 2 + (mStart.Y - mEnd.Y) ^ 2);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public class PriorityQueue<TP, TV>
    {
        private readonly SortedDictionary<TP, Queue<TV>> _list = new SortedDictionary<TP, Queue<TV>>();

        public bool IsEmpty
        {
            get { return !_list.Any(); }
        }

        public void Enqueue(TP priority, TV value)
        {
            Queue<TV> q;
            if (!_list.TryGetValue(priority, out q))
            {
                q = new Queue<TV>();
                _list.Add(priority, q);
            }
            q.Enqueue(value);
        }

        public TV Dequeue()
        {
            // will throw if there isn’t any first element!
            var pair = _list.First();
            var v = pair.Value.Dequeue();
            if (pair.Value.Count == 0) // nothing left of the top priority.
                _list.Remove(pair.Key);
            return v;
        }
    }
}