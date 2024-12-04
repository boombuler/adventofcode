namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day04 : Solution
{
    private string Read(StringMap<char> map, Point pt, Point dir, int len)
        => Enumerable.Range(0, len).Aggregate(new StringBuilder(),
            (sb, i) => sb.Append(map.GetValueOrDefault(pt + dir * i))).ToString();

    protected override long? Part1()
    {
        long Solve(string input)
        {
            const string SearchTerm = "XMAS";
            var map = input.AsMap();
            return (
                from x in map
                where x.Value == SearchTerm[0]
                from dir in x.Index.Neighbours(true)
                where Read(map, x.Index, x.Index - dir, SearchTerm.Length) == SearchTerm
                select x).Count();
        }

        Assert(Solve(Sample()), 18);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input)
        {
            const string SearchTerm = "MAS";
            string reversedTerm = new string(SearchTerm.Reverse().ToArray());

            var map = input.AsMap();
            return (
                from x in map
                where x.Value == SearchTerm[1]
                let wordA = Read(map, x.Index - (1, 1), (1, 1), SearchTerm.Length)
                where wordA == SearchTerm || wordA == reversedTerm
                let wordB = Read(map, x.Index - (-1, 1), (-1, 1), SearchTerm.Length)
                where wordB == SearchTerm || wordB == reversedTerm
                select x
            ).Count();
        }

        Assert(Solve(Sample()), 9);
        return Solve(Input);
    }
}
