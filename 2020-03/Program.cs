using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2020_03
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        class Slope
        {
            public static readonly Slope Default = new Slope(1,3);

            public static readonly Slope[] All = new[]
            {
                new Slope(1,1), new Slope(1,3), new Slope(1, 5), new Slope(1, 7), new Slope(2, 1)
            };

            public int Down { get; }
            public int Right { get; }

            public Slope(int down, int right)
                => (Down, Right) = (down, right);

            public long Navigate(bool[][] map)
            {
                long hits = 0;
                for (int y = 0, x = 0; y < map.Length; y += Down, x += Right)
                {
                    var row = map[y];
                    var hit = row[x % row.Length];
                    if (hit)
                        hits++;
                }

                return hits;
            }
        }

        private bool[][] ReadTreeMap(string mapFile)
            => ReadLines(mapFile)
                .Select(s => s.Select(c => c == '#').ToArray())
                .ToArray();
        
        private long CheckSlopes(string mapFile, params Slope[] slopes)
        {
            var map = ReadTreeMap(mapFile);
            return slopes.Select(s => s.Navigate(map)).Aggregate((a, b) => a * b);
        }

        protected override long? Part1()
        {
            Assert(CheckSlopes("Sample", Slope.Default), 7);
            return CheckSlopes("Input", Slope.Default);
        }

        protected override long? Part2()
        {
            Assert(CheckSlopes("Sample", Slope.All), 336);
            return CheckSlopes("Input", Slope.All);
        }
    }
}
