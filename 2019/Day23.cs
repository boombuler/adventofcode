namespace AdventOfCode._2019;

class Day23 : Solution
{
    const int COMPUTER_COUNT = 50;
    class NAT
    {
        private readonly Dictionary<long, Queue<(long, long)>> fQueue = [];
        private readonly HashSet<long> fIdleDevices = [];
        public int IdleCount => FIdleDevices.Count;
        public (long X, long Y)? Memory { get; private set; }

        public HashSet<long> FIdleDevices => fIdleDevices;

        public void ResumeNetwork()
        {
            if (Memory.HasValue)
                Send(0, Memory.Value.X, Memory.Value.Y);
        }

        private Queue<(long, long)> GetQueue(long addr)
        {
            if (fQueue.TryGetValue(addr, out var q))
                return q;
            return fQueue[addr] = new Queue<(long, long)>();
        }

        public bool TryFetch(long address, out long X, out long Y)
        {
            if (GetQueue(address).TryDequeue(out var res))
            {
                (X, Y) = res;
                FIdleDevices.Remove(address);
                return true;
            }
            FIdleDevices.Add(address);
            (X, Y) = (-1, -1);
            return false;
        }
        public void Send(long address, long X, long Y)
        {
            if (address == 255)
                Memory = (X, Y);
            else
            {
                GetQueue(address).Enqueue((X, Y));
                FIdleDevices.Remove(address);
            }
        }
    }
    class Computer
    {
        private IntCodeVM fCurrent;
        private readonly Queue<long> fInputQueue;
        private readonly long fAddress;
        private readonly long[] fPacket;
        private int fIdx;

        public Computer(IntCodeVM state, long address)
        {
            fCurrent = state;
            fAddress = address;
            fPacket = new long[3];
            fIdx = 0;
            fInputQueue = new Queue<long>();
            fInputQueue.Enqueue(address);
        }

        public void Step(NAT nat)
        {
            var (data, next) = fCurrent.Step(() =>
            {
                if (fInputQueue.Count == 0)
                {
                    if (!nat.TryFetch(fAddress, out long x, out long y))
                        return -1;

                    fInputQueue.Enqueue(x);
                    fInputQueue.Enqueue(y);
                }
                return fInputQueue.Dequeue();
            });
            fCurrent = next;
            if (data.HasValue)
            {
                fPacket[fIdx++] = data.Value;
                if (fIdx == 3)
                {
                    fIdx = 0;
                    var (addr, (x, (y, _))) = fPacket;
                    nat.Send(addr, x, y);
                }
            }
        }
    }

    private (NAT, List<Computer>) BuildNetwork()
    {
        var state = new IntCodeVM(Input);
        return (new NAT(), Enumerable.Range(0, COMPUTER_COUNT).Select(a => new Computer(state, a)).ToList());
    }

    protected override long? Part1()
    {
        var (nat, comps) = BuildNetwork();
        while (!nat.Memory.HasValue)
        {
            foreach (var c in comps)
                c.Step(nat);
        }
        return nat.Memory.Value.Y;
    }

    protected override long? Part2()
    {
        var (nat, comps) = BuildNetwork();
        var seenPackets = new HashSet<long>();
        while (true)
        {
            while (nat.IdleCount < COMPUTER_COUNT || !nat.Memory.HasValue)
            {
                foreach (var c in comps)
                    c.Step(nat);
            }

            if (!seenPackets.Add(nat.Memory.Value.Y))
                return nat.Memory.Value.Y;
            nat.ResumeNetwork();
        }
    }
}
