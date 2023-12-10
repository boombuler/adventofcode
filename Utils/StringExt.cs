namespace AdventOfCode.Utils;

using System.IO;

static class StringExt
{
    public static IEnumerable<string> Lines(this string str)
    {
        using var sr = new StringReader(str ?? string.Empty);

        while (true)
        {
            var line = sr.ReadLine();
            if (line == null)
                yield break;
            yield return line;
        }
    }

    public static Dictionary<Point2D, char> Cells(this string str, Func<char, bool> filter = null) => str.Cells(c => c, filter);

    public static Dictionary<Point2D, T> Cells<T>(this string str, Func<char, T> selector, Func<char, bool> filter = null)
        => str.Lines()
            .SelectMany((l, y) => l.Select((c, x) => (x, y, c)))
            .Where(n => filter?.Invoke(n.c) ?? true)
            .ToDictionary(n => new Point2D(n.x, n.y), n => selector(n.c));

    public static StringMap<T> AsMap<T>(this string str, Func<char, T> selector)
        => new StringMap<T>(str, selector);

    public static StringMap<char> AsMap(this string str)
        => new StringMap<char>(str, c => c);
}
