using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public static class FastLevenshtein
    {
        private static readonly ConcurrentDictionary<string, int[]> _cache = new();

        public static float GetSimilarity(string a, string b)
        {
            if (a.Length == 0 || b.Length == 0)
                return 0;

            var key = a.Length > b.Length ? $"{a}:{b}" : $"{b}:{a}";
            var buffer = _cache.GetOrAdd(key, _ => new int[Math.Max(a.Length, b.Length) + 1]);

            int distance = Calculate(a, b, buffer);
            return 1 - (float)distance / Math.Max(a.Length, b.Length);
        }

        private static int Calculate(ReadOnlySpan<char> a, ReadOnlySpan<char> b, int[] buffer)
        {
            for (int i = 0; i <= a.Length; i++) buffer[i] = i;

            for (int j = 1; j <= b.Length; j++)
            {
                int prevDiagonal = buffer[0];
                buffer[0] = j;

                for (int i = 1; i <= a.Length; i++)
                {
                    int oldDiagonal = buffer[i];
                    buffer[i] = Math.Min(
                        Math.Min(buffer[i - 1] + 1, buffer[i] + 1),
                        prevDiagonal + (a[i - 1] == b[j - 1] ? 0 : 1)
                    );
                    prevDiagonal = oldDiagonal;
                }
            }

            return buffer[a.Length];
        }
    }
}
