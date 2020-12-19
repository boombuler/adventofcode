using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventHelper
{
    public static class TextReaderHelper
    {

        public static string ReadToTerm(this TextReader tr, char term)
        {
            var result = new StringBuilder();

            int c;
            while((c = tr.Read()) >= 0)
            {
                char ch = (char)c;
                if (ch != term)
                    result.Append(ch);
                else
                    break;
            }

            return result.ToString();
        }
    }
}
