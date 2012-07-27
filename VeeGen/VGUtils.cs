#region
using System;
using System.Collections.Generic;

#endregion
namespace VeeGen
{
    public static class VGUtils
    {
        static VGUtils() { Random = new Random(); }

        public static Random Random { get; set; }

        public static int GetRandomInt(int mMin, int mMax) { return Random.Next(mMin, mMax); }
        public static int GetRandomInt(List<int> mInts) { return mInts[Random.Next(0, mInts.Count)]; }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}