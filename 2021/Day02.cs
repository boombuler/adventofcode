using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode._2021
{
    class Day02 : Solution
    {
        private long RunCommands(Func<Point3D, string, Point3D> getOffset)
        {
            var pt = Input.Lines()
                .Select(l => l.Split(' '))
                .Select(line => new { Dir = line[0], Amount = long.Parse(line[1]) })
                .Aggregate(Point3D.Origin, (cur, cmd) => cur + cmd.Amount * getOffset(cur, cmd.Dir));
            return pt.X * pt.Y;
        }

        protected override long? Part1()
            => RunCommands((_, dir) => dir switch 
            {
                "up"   => new Point3D(0, -1, 0),
                "down" => new Point3D(0,  1, 0),
                _      => new Point3D(1,  0, 0)
            });

        protected override long? Part2()
            => RunCommands((cur, dir) => dir switch
            {
                "up"   => new Point3D(0,     0, -1),
                "down" => new Point3D(0,     0,  1),
                _      => new Point3D(1, cur.Z,  0)
            });
    }
}
