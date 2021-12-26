namespace AdventOfCode._2020;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day14 : Solution
{
    private static readonly Regex MASK = new(@"mask = (?<Mask>[\dX]{36})");
    record MaskMatch(string Mask);
    private static readonly Regex SET_VAL = new(@"mem\[(?<Addr>\d+)\] = (?<Value>\d+)");
    record SetValMatch(long Addr, long Value);

    private static IEnumerable<(string Mask, long Address, long Value)> ParseCommands(string codes)
    {
        var m = string.Empty;
        foreach (var cmd in codes.Lines())
        {
            if (MASK.TryMatch(cmd, out MaskMatch match))
            {
                m = match.Mask;
                m = new string(m.Reverse().ToArray());
            }
            else if (SET_VAL.TryMatch(cmd, out SetValMatch setVal))
            {
                yield return (m, setVal.Addr, setVal.Value);
            }
        }
    }

    private static long RunProgram(string codes)
    {
        var mem = new Dictionary<long, long>();
        foreach (var (mask, address, value) in ParseCommands(codes))
        {
            var val = value;
            for (int i = 0; i < 36; i++)
            {
                switch (mask[i])
                {
                    case '0': val &= ~(1L << i); break;
                    case '1': val |= (1L << i); break;
                }
            }
            mem[address] = val;
        }

        return mem.Values.Sum();
    }

    private static long RunProgram2(string codes)
    {
        var mem = new Dictionary<long, long>();

        foreach (var (mask, addr, value) in ParseCommands(codes))
        {
            IEnumerable<long> addresses = new[] { 0L };
            for (int i = 35; i >= 0; i--)
            {
                switch (mask[i])
                {
                    case '0':
                        var b = ((addr >> i) & 1);
                        addresses = addresses.Select(a => (a << 1) | b); break;
                    case '1':
                        addresses = addresses.Select(a => (a << 1) | 1); break;
                    default:
                        addresses = addresses.SelectMany(a => new[] { (a << 1) | 1, (a << 1) }).ToList();
                        break;
                }
            }
            foreach (var a in addresses)
                mem[a] = value;
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
