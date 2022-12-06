namespace AdventOfCode._2019;

class Day08 : Solution<long?, string>
{
    const int LAYER_WIDTH = 25;
    const int LAYER_HEIGHT = 6;
    const int LAYER_SIZE = LAYER_HEIGHT * LAYER_WIDTH;

    protected override long? Part1()
    {
        var (min, _) = Input.Chunk(LAYER_SIZE).MinMaxBy(r => r.Count(n => n == '0'));
        return min.Count(n => n == '1') * min.Count(n => n == '2');
    }

    protected override string Part2()
    {
        var image = Input.Chunk(LAYER_SIZE).Aggregate<IEnumerable<char>>((a, b) => a.Zip(b, (a, b) => a == '2' ? b : a)).ToArray();
        return new OCR6x5().Decode((x, y) => image[x + (LAYER_WIDTH * y)] == '1', LAYER_WIDTH);
    }
}
