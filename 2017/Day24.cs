﻿using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode._2017
{
    class Day24 : Solution
    {
        record Connector (int A, int B)
        {
            public static readonly Func<string, Connector> Parse = new Regex(@"(?<A>\d+)/(?<B>\d+)").ToFactory<Connector>();
        }

        record Bridge
        {
            public int Tip { get; private init; }
            public ImmutableHashSet<Connector> Connections { get; private init; } = ImmutableHashSet<Connector>.Empty;

            public int Strength { get; private init; }
            public int Length => Connections.Count;

            public Bridge Push(Connector c)
                => new Bridge()
                {
                    Tip = c.A == Tip ? c.B : c.A,
                    Strength = Strength + c.A + c.B,
                    Connections = Connections.Add(c)
                };
        }

        private IEnumerable<Bridge> BuildBridges(string input)
        {
            var connections = input.Lines().Select(Connector.Parse)
                .SelectMany(c => new[] { (c.A, c), (c.B, c) })
                .ToLookup(x => x.Item1, x => x.c);

            var open = new Stack<Bridge>();
            open.Push(new Bridge());
            while (open.TryPop(out var bridge))
            {
                bool done = true;
                foreach (var option in connections[bridge.Tip].Where(i => !bridge.Connections.Contains(i)))
                {
                    open.Push(bridge.Push(option));
                    done = false;
                }
                if (done)
                    yield return bridge;
            }
        }

        private long GetStrongestBridge(string input)
            => BuildBridges(input).Max(b => b.Strength);

        protected override long? Part1()
        {
            Assert(GetStrongestBridge(Sample()), 31);
            return GetStrongestBridge(Input);
        }

        protected override long? Part2()
            => BuildBridges(Input).OrderByDescending(b => b.Length).ThenByDescending(b => b.Strength).First().Strength;
    }
}
