using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day10 : Solution<string, int>
    {
        record Star
        {
            public Point2D Position { get; init; }
            private Point2D Velocity { get; }
            public Star(long PosX, long PosY, long VelX, long VelY)
                => (Position, Velocity) = ((PosX, PosY), (VelX, VelY));
            public Star Move() => this with { Position = Position + Velocity };
        }
        private static readonly Func<string, Star> ParseStar = new Regex(@"position=<\s*(?<PosX>-?\d+),\s*(?<PosY>-?\d+)> velocity=<\s*(?<VelX>-?\d+),\s*(?<VelY>-?\d+)>").ToFactory<Star>();

        private (string Text, int Second) Align(string stars)
        {
            var curStars =  stars.Lines().Select(ParseStar).ToArray();
            var next = new Star[curStars.Length];
            int seconds = 1;
            while(true)
            {
                for (int i = 0; i < curStars.Length; i++)
                    next[i] = curStars[i].Move();
                var (minY, maxY) = next.MinMax(s => s.Position.Y);
                if (maxY - minY == OCR.CharHeight-1)
                {
                    var (minX, maxX) = next.MinMax(s => s.Position.X);

                    var charMap = new char[maxX + 1 - minX, maxY + 1 - minY];
                    foreach (var s in next)
                        charMap[s.Position.X - minX, s.Position.Y - minY] = '#';

                    return (OCR.Decode(charMap), seconds);
                }
                    
                (curStars, next) = (next, curStars);
                seconds++;
            }
        }

        private OCR OCR => new OCR10x6();

        protected override string Part1() => Align(Input).Text;
      

        protected override int Part2() => Align(Input).Second;
    }
}
