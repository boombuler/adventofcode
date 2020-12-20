using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2016_04
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        class Room
        {
            public string Name;
            public int Id;
            public string CheckSum;

            private static string Alphabet = "abcdefghijklmnopqrstuvwxyz";

            public string DecodeName()
                => new string(
                Name.Trim('-').Select(c => Alphabet.IndexOf(c))
                    .Select(i => i == -1 ? ' ' : Alphabet[(i + Id) % Alphabet.Length])
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

        private static Func<string, Room> RoomFactory = new Regex(@"(?<Name>(\w+\-)+)(?<Id>\d+)\[(?<CheckSum>\w{5})\]", RegexOptions.Compiled).ToFactory<Room>();

        private long SumValidRoomIDs(string file)
            => ReadLines(file).Select(RoomFactory).Where(r => r.ValidCheckSum()).Select(r => r.Id).Sum();


        protected override long? Part1()
        {
            Assert(SumValidRoomIDs("Sample"), 1514);
            return SumValidRoomIDs("Input");
        }


        protected override long? Part2()
        {
            Assert((new Room() { Name = "qzmt-zixmtkozy-ivhz", Id = 343 }).DecodeName(), "very encrypted name");

            return ReadLines("Input")
                .Select(RoomFactory)
                .Where(r => r.ValidCheckSum())
                .Where(r => r.DecodeName() == "northpole object storage")
                .First().Id;
        }
    }
}
