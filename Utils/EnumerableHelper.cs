namespace AdventOfCode.Utils;

public static class EnumerableHelper
{
    public static void RotateRight<T>(this T[] array, int count)
    {
        if (array.Length == 0)
            return;

        while (count < 0)
            count += array.Length;
        count %= array.Length;
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

        var result = new List<T[]>();
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

    public static IEnumerable<T> Unfold<TSeed, T>(this TSeed seed, Func<TSeed, (T Value, TSeed NextSeed)> generator, Func<TSeed, bool> @continue = null)
    {
        while (true)
        {
            var (value, nextSeed) = generator(seed);
            yield return value;
            if (@continue?.Invoke(nextSeed) ?? true)
                seed = nextSeed;
            else
                break;
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
            else
                yield return new[] { itm };
        }
    }

    /// <summary>
    /// Like <see cref="Combinations{T}(IEnumerable{T}, int)"/> but only for two items.
    /// </summary>
    public static IEnumerable<(T A, T B)> CombinationPairs<T>(this IEnumerable<T> items)
    {
        var seenItems = new List<T>();
        foreach (var i in items)
        {
            foreach (var o in seenItems)
                yield return (o, i);
            seenItems.Add(i);
        }
    }

    public static IEnumerable<TVal> CombinationPairs<T, TVal>(this IEnumerable<T> items, Func<T, T, TVal> selector)
        => CombinationPairs(items).Select(n => selector(n.A, n.B));

    /// <summary>
    /// returns a sliding window with <paramref name="windowSize"/> items from the <paramref name="items"/>
    /// </summary>
    /// <example>
    /// <code>
    ///     new int[]{ 1, 2, 3, 4 }.SlidingWindow(2)
    /// </code>
    /// would return { (1, 2), (2, 3), (3, 4) }
    /// </example>
    public static IEnumerable<T[]> SlidingWindow<T>(this IEnumerable<T> items, int windowSize)
    {
        var it = items.GetEnumerator();
        var buf = new T[windowSize];
        for (int i = 0; i < windowSize; i++)
        {
            if (!it.MoveNext())
                yield break;
            buf[i] = it.Current;
        }
        yield return buf;

        while (it.MoveNext())
        {
            var newBuf = new T[windowSize];
            Array.Copy(buf, 1, newBuf, 0, windowSize - 1);
            newBuf[windowSize - 1] = it.Current;
            yield return buf = newBuf;
        }
    }

    /// <summary>
    /// Like SlidingWindow(2) but returns a tuple of the two items
    /// </summary>
    public static IEnumerable<TRes> Pairwise<T, TRes>(this IEnumerable<T> items, Func<T, T, TRes> selector)
    {
        var it = items.GetEnumerator();
        if (!it.MoveNext())
            yield break;
        T last = it.Current;

        while (it.MoveNext())
        {
            T cur = it.Current;
            yield return selector(last, cur);
            last = cur;
        }
    }

    /// <summary>
    /// Like SlidingWindow(2) but returns a tuple of the two items
    /// </summary>
    public static IEnumerable<(T A, T B)> Pairwise<T>(this IEnumerable<T> items)
        => Pairwise(items, (a, b) => (a, b));


    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var itm in items)
            action?.Invoke(itm);
    }

    public static IEnumerable<TVal> Scan<T, TVal>(this IEnumerable<T> items, TVal seed, Func<TVal, T, TVal> selector)
    {
        yield return seed;
        foreach (var itm in items)
        {
            seed = selector(seed, itm);
            yield return seed;
        }
    }

    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        foreach (var i in items)
        {
            yield return i;
            if (predicate(i))
                break;
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
        foreach (var itm in items.Select(selector))
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

    public static (T min, T max) MinMaxBy<T, TVal>(this IEnumerable<T> items, Func<T, TVal> selector, IComparer<TVal> comparer = null)
    {
        comparer ??= Comparer<TVal>.Default;

        T maxItm = default;
        TVal maxVal = default;
        T minItm = default;
        TVal minVal = default;
        bool first = true;

        foreach (var itm in items)
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

    public static int GetCollectionHashCode<T>(this ICollection<T> items)
    {
        var hc = new HashCode();
        hc.Add(items.Count);
        foreach (var i in items)
            hc.Add(i);
        return hc.ToHashCode();
    }

    public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> valueFactory)
        => dictionary.TryGetValue(key, out var res) ? res : dictionary[key] = valueFactory();

    public static T RandomElement<T>(this IEnumerable<T> items)
        => items.ElementAt(Random.Shared.Next(items.Count()));
}
