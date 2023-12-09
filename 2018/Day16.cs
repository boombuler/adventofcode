namespace AdventOfCode._2018;

class Day16 : Solution
{
    record SampleRow(int OpCode, int A, int B, int C, int[] Before, int[] After)
    {
        public static Func<string, SampleRow> Parse = new Regex(@"Before:\s*\[((, )?(?<Before>\d+))+\]\n(?<OpCode>\d+) (?<A>\d+) (?<B>\d+) (?<C>\d+)\nAfter:\s*\[((, )?(?<After>\d+))+\]", RegexOptions.Compiled | RegexOptions.Multiline).ToFactory<SampleRow>();

        public bool Matches(OpCodeImpl impl)
            => After.SequenceEqual(impl(A, B, C, Before));

        public int CountMatching(IEnumerable<OpCodeImpl> impl)
            => impl.Where(Matches).Count();
    }

    delegate int[] OpCodeImpl(int a, int b, int c, int[] registers);

    private static int[] Set(int[] registers, int c, int val)
    {
        var res = (int[])registers.Clone();
        res[c] = val;
        return res;
    }

    private static OpCodeImpl BinOpR(Func<int, int, int> calc) => (int a, int b, int c, int[] registers) => Set(registers, c, calc(registers[a], registers[b]));
    private static OpCodeImpl BinOpI(Func<int, int, int> calc) => (int a, int b, int c, int[] registers) => Set(registers, c, calc(registers[a], b));

    private static OpCodeImpl Addr => BinOpR((a, b) => a + b);
    private static OpCodeImpl Addi => BinOpI((a, b) => a + b);
    private static OpCodeImpl Mulr => BinOpR((a, b) => a * b);
    private static OpCodeImpl Muli => BinOpI((a, b) => a * b);
    private static OpCodeImpl Banr => BinOpR((a, b) => a & b);
    private static OpCodeImpl Bani => BinOpI((a, b) => a & b);
    private static OpCodeImpl Borr => BinOpR((a, b) => a | b);
    private static OpCodeImpl Bori => BinOpI((a, b) => a | b);
    private static int[] Setr(int a, int b, int c, int[] registers) => Set(registers, c, registers[a]);
    private static int[] Seti(int a, int b, int c, int[] registers) => Set(registers, c, a);
    private static int[] Gtir(int a, int b, int c, int[] registers) => Set(registers, c, a > registers[b] ? 1 : 0);
    private static int[] Gtri(int a, int b, int c, int[] registers) => Set(registers, c, registers[a] > b ? 1 : 0);
    private static int[] Gtrr(int a, int b, int c, int[] registers) => Set(registers, c, registers[a] > registers[b] ? 1 : 0);
    private static int[] Eqir(int a, int b, int c, int[] registers) => Set(registers, c, a == registers[b] ? 1 : 0);
    private static int[] Eqri(int a, int b, int c, int[] registers) => Set(registers, c, registers[a] == b ? 1 : 0);
    private static int[] Eqrr(int a, int b, int c, int[] registers) => Set(registers, c, registers[a] == registers[b] ? 1 : 0);

    private readonly OpCodeImpl[] AllOpCodes = [
        Addr, Addi, Mulr, Muli, Banr, Bani, Borr, Bori, Setr, Seti, Gtir, Gtri, Gtrr, Eqir, Eqri, Eqrr
    ];

    private static List<SampleRow> ParseSamples(string input)
        => input.Split("\n\n").Select(SampleRow.Parse).Where(s => s != null).ToList();

    private OpCodeImpl[] RestoreOpCodeTable(string input)
    {
        var samples = ParseSamples(input);
        var result = new OpCodeImpl[AllOpCodes.Length];
        var unassigned = new List<OpCodeImpl>(AllOpCodes);

        while (unassigned.Count > 0)
        {
            var assign = samples.Select(s => new { s.OpCode, Sample = s, Count = s.CountMatching(unassigned) }).First(r => r.Count == 1);
            var op = unassigned.First(assign.Sample.Matches);
            unassigned.Remove(op);
            samples.RemoveAll(s => s.OpCode == assign.OpCode);
            result[assign.OpCode] = op;
        }
        return result;
    }

    protected override long? Part1()
        => ParseSamples(Input)
            .Where(s => s.CountMatching(AllOpCodes) >= 3)
            .Count();

    protected override long? Part2()
    {
        var opCodeTable = RestoreOpCodeTable(Input);
        var registers = new int[] { 0, 0, 0, 0 };
        var program = Input.Split("\n\n\n\n").Last();
        foreach (var line in program.Lines())
        {
            var (op, (a, (b, (c, _)))) = line.Split(' ').Select(int.Parse);
            registers = opCodeTable[op](a, b, c, registers);
        }
        return registers[0];
    }
}
