using System.Collections.Generic;
using System.Linq;

namespace AdventHelper
{
    public static class EnumerableHelper
    {
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
