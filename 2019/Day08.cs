using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

namespace AdventOfCode._2019
{
    class Day08 : Solution<long?, string>
    {
        const int LayerWidth = 25;
        const int LayerHeight = 6;
        const int LayerSize = LayerHeight * LayerWidth;

        protected override long? Part1()
        {
            var (min, _) = Input.Chunk(LayerSize).MinMaxBy(r => r.Count(n => n == '0'));
            return min.Count(n => n == '1') * min.Count(n => n == '2');
        }

        protected override string Part2()
        {
            var image = Input.Chunk(LayerSize).Aggregate<IEnumerable<char>>((a, b) => a.Zip(b, (a, b) => a == '2' ? b : a)).ToArray();
            return new OCR6x5().Decode((x, y) => image[x + (LayerWidth * y)] == '1', LayerWidth);
        }
    }
}
