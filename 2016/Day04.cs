namespace AdventOfCode._2016;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day04 : Solution
{
    record Room(string Name, int Id, string CheckSum = "")
    {
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        public string DecodeName()
            => new(
            Name.Trim('-').Select(c => ALPHABET.IndexOf(c))
                .Select(i => i == -1 ? ' ' : ALPHABET[(i + Id) % ALPHABET.Length])
                .ToArray());

        public bool ValidCheckSum()
        {
            var letters = Name.Where(c => c != '-').GroupBy(c => c).Select(grp => new { grp.Key, Count = grp.Count() })
                .OrderByDescending(c => c.Count);
            var minCount = letters.Skip(4).First().Count;

            var cs = new string(letters
                .TakeWhile(g => g.Count >= minCount)
                .OrderByDescending(g => g.Count).ThenBy(g => g.Key)
                .Select(g => g.Key)
                .Take(5).ToArray()
            );
            return cs == CheckSum;
        }
    }

    private static readonly Func<string, Room> RoomFactory = new Regex(@"(?<Name>(\w+\-)+)(?<Id>\d+)\[(?<CheckSum>\w{5})\]", RegexOptions.Compiled).ToFactory<Room>();

    private static long SumValidRoomIDs(string rooms)
        => rooms.Lines().Select(RoomFactory).Where(r => r.ValidCheckSum()).Select(r => r.Id).Sum();

    protected override long? Part1()
    {
        Assert(SumValidRoomIDs(Sample()), 1514);
        return SumValidRoomIDs(Input);
    }

    protected override long? Part2()
    {
        Assert(new Room("qzmt-zixmtkozy-ivhz", 343).DecodeName(), "very encrypted name");

        return Input.Lines()
            .Select(RoomFactory)
            .Where(r => r.ValidCheckSum())
            .Where(r => r.DecodeName() == "northpole object storage")
            .First().Id;
    }
}
