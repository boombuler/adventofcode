namespace AdventOfCode._2016;

class Day07 : Solution
{
    private static readonly Regex MatchABBA = new(@"(\w)((?:(?!\1).))\2\1", RegexOptions.Compiled);
    private static readonly Regex FindHyperNet = new(@"\[[^\]]*\]", RegexOptions.Compiled);
    private static readonly Regex MatchABA = new(@"(?<a>\w)(?<b>(?:(?!\1).))\1", RegexOptions.Compiled);

    private bool SupportsTLS(string ipv7Addr)
    {
        if (FindHyperNet.Matches(ipv7Addr).Select(m => m.Value).Any(MatchABBA.IsMatch))
            return false;
        return MatchABBA.IsMatch(ipv7Addr);
    }

    protected override long? Part1()
    {
        Assert(SupportsTLS("abba[mnop]qrst"), true);
        Assert(SupportsTLS("abcd[bddb]xyyx"), false);
        Assert(SupportsTLS("aaaa[qwer]tyui"), false);
        Assert(SupportsTLS("ioxxoj[asdfgh]zxcvbn"), true);
        return Input.Lines().Count(SupportsTLS);
    }

    public bool SupportsSSL(string ipv7Addr)
    {
        var hypernet = string.Join(string.Empty, FindHyperNet.Matches(ipv7Addr).Select(m => m.Value));
        var supernet = FindHyperNet.Replace(ipv7Addr, "-");
        var aba = MatchABA.Match(supernet);
        while (aba.Success)
        {
            var bab = aba.Groups["b"].Value + aba.Groups["a"].Value + aba.Groups["b"].Value;
            if (hypernet.Contains(bab))
                return true;
            aba = MatchABA.Match(supernet, aba.Index + 1);
        }
        return false;
    }

    protected override long? Part2()
    {
        Assert(SupportsSSL("aba[bab]xyz"), true);
        Assert(SupportsSSL("xyx[xyx]xyx"), false);
        Assert(SupportsSSL("aaa[kek]eke"), true);
        Assert(SupportsSSL("zazbz[bzb]cdb"), true);
        return Input.Lines().Count(SupportsSSL);
    }
}
