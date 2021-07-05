using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Utils
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
        public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> items, int chunckSize)
        {
            if (chunckSize <= 0)
                throw new ArgumentException();
            return items.Select((itm, i) => new { Value = itm, ChunkNo = i / chunckSize })
                .GroupBy(c => c.ChunkNo)
                .Select(c => c.Select(i => i.Value));
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

        public static IEnumerable<T> Unfold<T>(this T seed, Func<T, T> generator)
        {
            while (true)
            {
                seed = generator(seed);
                yield return seed;
            }
        }

        public static IEnumerable<T> Unfold<TSeed, T>(this TSeed seed, Func<TSeed, (T Value, TSeed NextSeed)> generator)
        {
            while (true)
            {
                var next = generator(seed);
                yield return next.Value;
                seed = next.NextSeed;
            }
        }

        /// <summary>
        /// returns an enumerable of each item combination of <paramref name="itemCount"/> items from the <paramref name="items"/>
        /// </summary>
        /// <example>
        /// <code>
        ///     new int[]{ 1, 2, 3, 4 }.Combinations(2)
        /// </code>
        /// would return { (1, 2), (1, 3), (1, 4), (2, 3), (2, 4), (3, 4)
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> items, int itemCount)
        {
            int remainingItems = itemCount - 1;
            for (int i = 0; i < items.Count() - remainingItems; i++)
            {
                var itm = items.ElementAt(i);
                if (remainingItems > 0)
                {
                    foreach (var subCombination in Combinations(items.Skip(i + 1), remainingItems))
                        yield return subCombination.Prepend(itm);
                }
                else yield return new[] { itm };
            }
        }

        public static void Deconstruct<T>(this IEnumerable<T> items, out T first, out IEnumerable<T> rest)
        {
            first = items.First();
            rest = items.Skip(1);
        }

        public static (T min, T max) MinMax<T>(this IEnumerable<T> items) where T : IComparable<T>
            => MinMax(items, x => x);

        public static (T min, T max) MinMax<TItem, T>(this IEnumerable<TItem> items, Func<TItem, T> selector) where T : IComparable<T>
        {
            T min = default, max = default;
            bool init = true;
            foreach(var itm in items.Select(selector))
            {
                if (init)
                {
                    init = false;
                    min = itm;
                    max = itm;
                }
                else
                {
                    if (min.CompareTo(itm) > 0)
                        min = itm;
                    else if (max.CompareTo(itm) < 0)
                        max = itm;
                }
            }
            return (min, max);
        }

        public static T Min<T>(this IEnumerable<T> items, IComparer<T> comparer)
        {
            T result = default;
            bool first = true;

            foreach(var itm in items)
            {
                if (first)
                {
                    result = itm;
                    first = false;
                }
                else if (comparer.Compare(result, itm) > 0)
                    result = itm;
            }
            return result;
        }
    
        public static (T min, T max) MinMaxBy<T, TVal>(this IEnumerable<T> items, Func<T, TVal> selector, IComparer<TVal> comparer = null)
        {
            comparer ??= Comparer<TVal>.Default;

            T maxItm = default;
            TVal maxVal = default;
            T minItm = default;
            TVal minVal = default;
            bool first = true;

            foreach(var itm in items)
            {
                var v = selector(itm);
                var f = first;
                if (f || comparer.Compare(v, maxVal) > 0)
                {
                    maxVal = v;
                    maxItm = itm;
                    first = false;
                }
                if (f || comparer.Compare(v, minVal) < 0)
                {
                    minVal = v;
                    minItm = itm;
                    first = false;
                }
            }
            if (first)
                throw new InvalidOperationException();
            return (minItm, maxItm);
        }
    }
}
