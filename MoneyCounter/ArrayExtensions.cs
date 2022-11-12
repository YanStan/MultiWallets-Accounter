using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyCounter
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T[]> Partition<T>(this IReadOnlyCollection<T> source, int size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
                yield return source.Skip(size * i).Take(size).ToArray();
        }

        public static IEnumerable<int> AllIndicesOf(this string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }
    }
}
