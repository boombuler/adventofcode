namespace AdventOfCode._2015;

using static Parser;

class Day08 : Solution
{
    private static readonly Parser<Unit> EscapedBackslash = Char('\\').Take(2).Return(Unit.Value);
    private static readonly Parser<Unit> HexCharacter = ("\\x" + Any.Take(2)).Return(Unit.Value);
    private static readonly Parser<Unit> EscapedQuote = Char('\\').Then(Char('"')).Return(Unit.Value);
    private static readonly Parser<Unit> AnyNonQuoteChar = CharExcept('"').Return(Unit.Value);
    private static readonly Func<string, Unit[]> UnquotedCharacters =
        "\"" + (EscapedQuote | EscapedBackslash | HexCharacter | AnyNonQuoteChar).Many() + "\"";

    private static long CountChars(string input)
        => input.Lines().Sum(s => (long)s.Length);

    private static long CountUnquoted(string input)
        => input.Lines().Sum(l => UnquotedCharacters(l).Length);

    private static long CountQuoted(string s)
        => s.Aggregate(2L, (acc, c) => acc + (c is '"' or '\\' ? 2 : 1));

    private static long CountQuotedStringChars(string input)
        => input.Lines().Sum(CountQuoted);

    protected override long? Part1()
    {
        Assert(CountChars(Sample()), 23);
        Assert(CountUnquoted(Sample()), 11);
        return CountChars(Input) - CountUnquoted(Input);
    }

    protected override long? Part2()
    {
        Assert(CountQuotedStringChars(Sample()), 42);
        return CountQuotedStringChars(Input) - CountChars(Input);
    }
}
