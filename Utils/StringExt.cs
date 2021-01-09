using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
