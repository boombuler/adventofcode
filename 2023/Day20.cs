namespace AdventOfCode._2023;

class Day20 : Solution
{
    class PulseQueue
    {
        private readonly Queue<(Module? from, Module? to, bool value)> fPendingSignals = new();
        public long LowCount { get; private set; }
        public long HighCount { get; private set; }
        public long ButtonPresses { get; private set; }

        public void Send(Module? from, Module? to, bool value)
        {
            if (value)
                HighCount++;
            else 
                LowCount++;
            fPendingSignals.Enqueue((from, to, value));
        }

        public void PushButton(Module to)
        {
            ButtonPresses++;
            Send(null, to, false);
            while(fPendingSignals.TryDequeue(out var cur))
                cur.to?.Pulse(this, cur.from, cur.value);
        }
    }

    class Module 
    {
        protected readonly List<Module> fOutgoing = [];

        public void ConnectTo(Module module)
        {
            fOutgoing.Add(module);
            module.ConnectedFrom(this);
        }
        
        protected virtual void ConnectedFrom(Module module)
        {
        }

        public virtual void Pulse(PulseQueue queue, Module? sender, bool hi)
        {
            foreach (var mod in fOutgoing)
                queue.Send(this, mod, hi);
        }
    }

    class FlipFlop : Module
    {
        private bool fValue = false;
        public override void Pulse(PulseQueue queue, Module? sender, bool hi)
        {
            if (!hi)
            {
                fValue = !fValue;
                base.Pulse(queue, sender, fValue);
            }
        }
    }

    class Conjunction : Module
    {
        public Dictionary<Module, bool> Incoming { get; } = [];

        protected override void ConnectedFrom(Module module) => Incoming.Add(module, false);

        public override void Pulse(PulseQueue queue, Module? sender, bool hi)
        {
            if (sender != null)
                Incoming[sender] = hi;

            base.Pulse(queue, sender, !Incoming.All(r => r.Value));
        }
    }

    class RxModule : Module
    {
        private readonly Dictionary<Module, long> fDividers = [];
        private Conjunction? fParent = null;

        public long? Result { get; private set; }

        protected override void ConnectedFrom(Module module) 
            => fParent = (module as Conjunction) ?? throw new InvalidOperationException("Parent has to be a Conjunction");

        public override void Pulse(PulseQueue queue, Module? sender, bool hi)
        {
            if (fParent == null)
                throw new InvalidOperationException();
            var high = fParent.Incoming.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
            foreach (var hm in high)
            {
                if (!fDividers.ContainsKey(hm))
                    fDividers[hm] = queue.ButtonPresses;
            }

            if (fParent.Incoming.Count == fDividers.Count)
                Result = fDividers.Values.Aggregate(MathExt.LCM);
        }
    }

    private static (Module Broadcaster, RxModule? Rx) BuildNetwork(string input)
    {
        var modules = new Dictionary<string, Module>();
        var connections = new List<(Module, string)>();
        foreach(var line in input.Lines())
        {
            var (name, (connStr, _)) = line.Split(" -> ");
            (name, Module mod) = name switch
            {
                ['%', .. var n] => (n, new FlipFlop()),
                ['&', .. var n] => (n, new Conjunction()),
                _ => (name, new Module())
            };
            modules[name] = mod;
            foreach(var conn in connStr.Split(", "))
                connections.Add((mod, conn));
        }
        foreach(var (from, to)  in connections)
        {
            if (!modules.TryGetValue(to, out var toMod))
            {
                toMod = new RxModule();
                modules[to] = toMod;
            }
            from.ConnectTo(toMod);
        }
        return (modules["broadcaster"], modules.GetValueOrDefault("rx") as RxModule);
    }

    protected override long? Part1()
    {
        static long Solve(string input)
        {
            var (bc, _) = BuildNetwork(input);
            var queue = new PulseQueue();
            for (int i = 0; i < 1000; i++)
                queue.PushButton(bc);
            return queue.LowCount * queue.HighCount;
        }

        Assert(Solve(Sample("1")), 32000000);
        Assert(Solve(Sample("2")), 11687500);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        var (bc, rx) = BuildNetwork(Input);
        if (rx == null)
            throw new InvalidOperationException();
        var queue = new PulseQueue();
        while (!rx.Result.HasValue)
            queue.PushButton(bc);
        return rx.Result;
    }
}
