using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventHelper
{
    public static class EnumerableHelper
    {
        public static void RotateRight<T>(this T[] array, int count)
        {
            if (array.Length == 0)
                return;

            while (count < 0)
                count += array.Length;
            count = count % array.Length;
            if (count == 0)
                return;

            T[] buff = new T[count];
            Array.Copy(array, array.Length - count, buff, 0, count);
            Array.Copy(array, 0, array, count, array.Length - count);
            Array.Copy(buff, array, count);
        }
        public static IEnumerable<int> Generate(int start = 0, int step = 1)
        {
            var value = start;

            while (true)
            {
                yield return value;
                value += step;
            }
        }

        public static IEnumerable<T[]> Permuatate<T>(this IEnumerable<T> items)
        {
            var arr = items.ToArray();

            List<T[]> result = new List<T[]>();
            int max = arr.Length - 1;
            void permutateIdx(int index)
            {
                if (index == max)
                    result.Add((T[])arr.Clone());
                else
                {
                    for (int i = index; i <= max; i++)
                    {
                        (arr[index], arr[i]) = (arr[i], arr[index]);
                        permutateIdx(index + 1);
                        (arr[index], arr[i]) = (arr[i], arr[index]);
                    }
                }
            }
            permutateIdx(0);
            return result;
        }
    }

}
