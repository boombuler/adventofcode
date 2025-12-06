namespace AdventOfCode.Utils;

using System.Diagnostics.CodeAnalysis;
using System.IO;

public static class TextReaderHelper
{
    public static bool TryRead(this TextReader tr, out char c)
    {
        int val = tr.Read();
        c = (char)val;
        return (val >= 0);
    }

    public static bool TryReadWhile(this TextReader tr, Func<char, bool> predicate, out string txt)
    {
        var sb = new StringBuilder();
        while(true)
        {
            var c = tr.Peek();
            if (c >= 0 && predicate((char)c))
                sb.Append((char)tr.Read());
            else
                break;
        }
        txt = sb.ToString();
        return sb.Length > 0;
    }

    public static bool TryReadLine(this TextReader tr, [NotNullWhen(true)] out string? line)
    {
        line = tr.ReadLine();
        return (line != null);
    }

    public static string ReadToTerm(this TextReader tr, char term)
    {
        var result = new StringBuilder();

        while (tr.TryRead(out char ch))
        {
            if (ch != term)
                result.Append(ch);
            else
                break;
        }

        return result.ToString();
    }
}
