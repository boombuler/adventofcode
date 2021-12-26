namespace AdventOfCode._2018;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day07 : Solution<string, int>
{
    record Dependency(char Name, char DependsOn);

    class TaskList
    {
        private readonly List<Dependency> fDependencies;
        private readonly List<char> fOpen;

        public TaskList(string dependencies)
        {
            fDependencies = dependencies.Lines().Select(ParseDep).ToList();
            fOpen = fDependencies.SelectMany(d => new[] { d.Name, d.DependsOn }).Distinct().ToList();
        }

        public bool TryDequeueTask(out char task)
        {
            task = fOpen.Where(n => !fDependencies.Any(x => x.Name == n)).OrderBy(n => n).FirstOrDefault();
            if (task != '\0')
            {
                fOpen.Remove(task);
                return true;
            }
            return false;
        }

        public bool FinishTask(char task)
        {
            fDependencies.RemoveAll(d => d.DependsOn == task);
            return fDependencies.Count == 0 && fOpen.Count == 0;
        }
    }

    private static readonly Func<string, Dependency> ParseDep = new Regex(@"Step (?<DependsOn>\w) must be finished before step (?<Name>\w) can begin\.", RegexOptions.Compiled).ToFactory<Dependency>();

    private static string GetExecutionOrder(string dependencies)
    {
        var tasks = new TaskList(dependencies);
        var result = new StringBuilder();
        while (tasks.TryDequeueTask(out char c))
        {
            result.Append(c);
            tasks.FinishTask(c);
        }
        return result.ToString();
    }

    private static int GetExecutionTime(string dependencies, int workerCount, int delay)
    {
        var tasks = new TaskList(dependencies);
        var currentWorkload = new List<(char Task, int FinishTime)>();
        int idleWorkers = workerCount;
        int tick = 0;
        while (true)
        {
            bool anyFinished = tick == 0;
            foreach (var done in currentWorkload.Where(wl => wl.FinishTime == tick).ToList())
            {
                currentWorkload.Remove(done);
                if (tasks.FinishTask(done.Task) && currentWorkload.Count == 0)
                    return tick;
                anyFinished = true;
                idleWorkers++;
            }

            while (anyFinished && idleWorkers > 0 && tasks.TryDequeueTask(out char nextTask))
            {
                idleWorkers--;
                currentWorkload.Add((nextTask, tick + delay + 1 + (nextTask - 'A')));
            }
            tick = currentWorkload.Min(c => c.FinishTime);
        }
    }

    protected override string Part1()
    {
        Assert(GetExecutionOrder(Sample()), "CABDFE");
        return GetExecutionOrder(Input);
    }

    protected override int Part2()
    {
        Assert(GetExecutionTime(Sample(), 2, 0), 15);
        return GetExecutionTime(Input, 5, 60);
    }

}
