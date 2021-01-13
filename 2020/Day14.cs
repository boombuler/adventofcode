using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2020
{
    class Day14 : Solution
    {
        private static readonly Regex MASK = new Regex(@"mask = (?<mask>[\dX]{36})");
        record MaskMatch(string mask);
        private static readonly Regex SET_VAL = new Regex(@"mem\[(?<addr>\d+)\] = (?<val>\d+)");
        record SetValMatch(long addr, long val);
        
        private IEnumerable<(string Mask, long Address, long Value)> ParseCommands(string codes)
        {
            var m = string.Empty;
            foreach (var cmd in codes.Lines())
            {
                if (MASK.TryMatch(cmd, out MaskMatch match))
                {
                    m = match.mask;
                    m = new string(m.Reverse().ToArray());
                }
                else if (SET_VAL.TryMatch(cmd, out SetValMatch setVal))
                {
                    yield return (m, setVal.addr, setVal.val);
                }
            }
        }

        private long RunProgram(string codes)
        {
            Dictionary<long, long> mem = new Dictionary<long, long>();
            foreach(var cmd in ParseCommands(codes))
            {
                var value = cmd.Value;
                for(int i = 0; i < 36; i++)
                {
                    switch(cmd.Mask[i])
                    {
                        case '0': value &= ~(1L << i); break;
                        case '1': value |= (1L << i); break;
                    }
                }
                mem[cmd.Address] = value;
            }

            return mem.Values.Sum();
        }


        private long RunProgram2(string codes)
        {
            Dictionary<long, long> mem = new Dictionary<long, long>();

            foreach (var cmd in ParseCommands(codes))
            {
                IEnumerable<long> Addresses = new [] { 0L };
                for(int i = 35; i >= 0; i--)
                {
                    switch(cmd.Mask[i])
                    {
                        case '0':
                            var b = ((cmd.Address >> i) & 1);
                            Addresses = Addresses.Select(a => (a << 1) | b); break;
                        case '1':
                            Addresses = Addresses.Select(a => (a << 1) | 1); break;
                        default:
                            Addresses = Addresses.SelectMany(a => new[] { (a << 1) | 1, (a << 1) }).ToList();
                            break;
                    }
                }
                foreach (var a in Addresses)
                    mem[a] = cmd.Value;
            }
            return mem.Values.Sum();
        }

        protected override long? Part1()
        {
            Assert(RunProgram(Sample()), 165);
            return RunProgram(Input);
        }

        protected override long? Part2() => RunProgram2(Input);
    }
}
