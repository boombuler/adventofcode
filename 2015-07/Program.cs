using AdventHelper;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _2015_07
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private static readonly Regex Intermediate = new Regex(@"^(?<value>\w+) \-\> (?<wire>\w+)$");
        private static readonly Regex AND = new Regex(@"^(?<a>\w+) AND (?<b>\w+) \-\> (?<wire>\w+)$");
        private static readonly Regex OR = new Regex(@"^(?<a>\w+) OR (?<b>\w+) \-\> (?<wire>\w+)$");
        private static readonly Regex NOT = new Regex(@"^NOT (?<a>\w+) \-\> (?<wire>\w+)$");
        private static readonly Regex LSHIFT = new Regex(@"^(?<a>\w+) LSHIFT (?<val>\d+) \-\> (?<wire>\w+)$");
        private static readonly Regex RSHIFT = new Regex(@"^(?<a>\w+) RSHIFT (?<val>\d+) \-\> (?<wire>\w+)$");



        private ushort TestWire(string instructionFile, string wire, (string wire, ushort value)? wireOverride = null)
        {
            Dictionary<string, Func<ushort>> wireNet = new Dictionary<string, Func<ushort>>();
            Match match;
            Func<ushort> arg(string grp)
            {
                var v = match.Groups[grp].Value;
                if (ushort.TryParse(v, out ushort val))
                    return () => val;
                return () => wireNet[v]();
            }

            void AddCalculation(Func<ushort> generator)
            {
                var val = new Lazy<ushort>(generator);
                wireNet[match.Groups["wire"].Value] = () => val.Value;
            }

            foreach(var instruction in ReadLines(instructionFile))
            {
                bool TryMatch(Regex r)
                {
                    match = r.Match(instruction);
                    return match.Success;
                }

                if (TryMatch(Intermediate))
                    AddCalculation(arg("value"));
                else if (TryMatch(AND))
                {
                    var a = arg("a");
                    var b = arg("b");
                    AddCalculation(() => (ushort)(a() & b()));
                }
                else if (TryMatch(OR))
                {
                    var a = arg("a");
                    var b = arg("b");
                    AddCalculation(() => (ushort)(a() | b()));
                }
                else if (TryMatch(NOT))
                {
                    var a = arg("a");
                    AddCalculation(() => (ushort)~a());
                }
                else if (TryMatch(LSHIFT))
                {
                    var a = arg("a");
                    var val = int.Parse(match.Groups["val"].Value);
                    AddCalculation(() =>(ushort)(a() << val));
                }
                else if (TryMatch(RSHIFT))
                {
                    var a = arg("a");
                    var val = int.Parse(match.Groups["val"].Value);
                    AddCalculation(() => (ushort)(a() >> val));
                }
                else
                    throw new NotImplementedException("Unknown Instruction");
            }
            if (wireOverride.HasValue)
                wireNet[wireOverride.Value.wire] = () => wireOverride.Value.value;

            return wireNet[wire]();
        }

        protected override long? Part1()
        {
            Assert(TestWire("SampleInput.txt", "d"), 72, "d");
            Assert(TestWire("SampleInput.txt", "e"), 507, "e");
            Assert(TestWire("SampleInput.txt", "f"), 492, "f");
            Assert(TestWire("SampleInput.txt", "g"), 114, "g");
            Assert(TestWire("SampleInput.txt", "h"), 65412, "h");
            Assert(TestWire("SampleInput.txt", "i"), 65079, "i");
            Assert(TestWire("SampleInput.txt", "x"), 123, "x");
            Assert(TestWire("SampleInput.txt", "y"), 456, "y");
            return TestWire("Input.txt", "a");
        }
        protected override long? Part2()
        {
            var oldA = TestWire("Input.txt", "a");
            return TestWire("Input.txt", "a", ("b", oldA));
        }
    }
}
