using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode.Utils
{
    public static class TextReaderHelper
    {


        public static bool TryRead(this TextReader tr, out char c)
        {
            int val = tr.Read();
            c = (char)val;
            return (val >= 0);
        }

        public static string ReadToTerm(this TextReader tr, char term)
        {
            var result = new StringBuilder();

            while(tr.TryRead(out char ch))
            {
                if (ch != term)
                    result.Append(ch);
                else
                    break;
            }

            return result.ToString();
        }
    }
}
