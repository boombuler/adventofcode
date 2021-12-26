namespace AdventOfCode._2018;

class Day19 : Solution
{
    private static int RunCompiled(string code, int initialR0 = 0)
    {
        var vm = ElfCompiler.CompileCode(code);
        var regs = new ElfCompiler.Registers();
        regs[0] = initialR0;
        vm(regs);
        return regs[0];
    }

    protected override long? Part1()
    {
        Assert(RunCompiled(Sample()), 6);
        return RunCompiled(Input);
    }

    protected override long? Part2() => RunCompiled(Input, 1);
}
