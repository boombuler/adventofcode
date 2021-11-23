using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Utils
{
    static class StringExt
    {
        public static IEnumerable<string> Lines(this string str)
        {
            using(var sr = new StringReader(str ?? string.Empty))
            {
                while(true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        yield break;
                    yield return line;
                }
            }
        }

        public static Dictionary<Point2D, char> Cells(this string str)
            => str.Lines()
                .SelectMany((l, y) => l.Select((c, x) => (x, y, c)))
                .ToDictionary(n => new Point2D(n.x, n.y), n => n.c);
    }
}
