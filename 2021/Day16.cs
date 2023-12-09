namespace AdventOfCode._2021;

class Day16 : Solution
{
    class BITS
    {
        private readonly byte[] fData;
        private readonly int fEnd;
        private int fOffset;

        public bool EOF => fOffset >= fEnd;

        public BITS(string data)
            : this(Convert.FromHexString(data), 0, data.Length * 8)
        {
        }

        private BITS(byte[] data, int offset, int len)
        {
            fData = data;
            fOffset = offset;
            fEnd = offset + len;
        }

        private BITS Slice(int len)
        {
            var res = new BITS(fData, fOffset, len);
            fOffset += len;
            return res;
        }

        private int Read(int len)
        {
            var res = 0;
            for (int i = 0; i < len; i++)
            {
                int c = fData[fOffset / 8];
                var bit = (c >> (7 - (fOffset % 8))) & 1;
                res = (res << 1) | bit;
                fOffset++;
            }
            return res;
        }

        public Packet ReadPacket()
        {
            var version = Read(3);
            var type = Read(3);
            if (type == 4)
            {
                var value = 0L;
                long tmp;
                do
                {
                    tmp = Read(5);
                    value = (value << 4) | (tmp & 0x0F);
                } while ((tmp & 0b10000) != 0);
                return new Packet(version, type, value, []);
            }

            if (Read(1) == 0)
            {
                var subPkgBuffer = Slice(Read(15));
                var subPkgs = ImmutableList<Packet>.Empty;
                while (!subPkgBuffer.EOF)
                    subPkgs = subPkgs.Add(subPkgBuffer.ReadPacket());
                return new Packet(version, type, subPkgs.Count, subPkgs);
            }
            else
                return new Packet(version, type, 0, Enumerable.Range(0, Read(11)).Select(n => ReadPacket()).ToImmutableList());
        }
    }

    record Packet(int Version, int Type, long Value, ImmutableList<Packet> Childs)
    {
        public long VersionSum() => Version + Childs.Sum(c => c.VersionSum());

        public long Eval()
        {
            var childVals = Childs.Select(c => c.Eval()).ToArray();
            return Type switch
            {
                0 => childVals.Sum(),
                1 => childVals.Aggregate((a, b) => a * b),
                2 => childVals.Min(),
                3 => childVals.Max(),
                4 => Value,
                5 => childVals[0] > childVals[1] ? 1 : 0,
                6 => childVals[0] < childVals[1] ? 1 : 0,
                _ => childVals[0] == childVals[1] ? 1 : 0
            };
        }
    };

    private static long SumVersions(string input) => new BITS(input).ReadPacket().VersionSum();

    private static long Eval(string input) => new BITS(input).ReadPacket().Eval();

    protected override long? Part1()
    {
        Assert(SumVersions("38006F45291200"), 9);
        Assert(SumVersions("EE00D40C823060"), 14);
        Assert(SumVersions("8A004A801A8002F478"), 16);
        Assert(SumVersions("620080001611562C8802118E34"), 12);
        Assert(SumVersions("C0015000016115A2E0802F182340"), 23);
        Assert(SumVersions("A0016C880162017C3686B18A3D4780"), 31);
        return SumVersions(Input);
    }

    protected override long? Part2()
    {
        Assert(Eval("C200B40A82"), 3);
        Assert(Eval("04005AC33890"), 54);
        Assert(Eval("880086C3E88112"), 7);
        Assert(Eval("CE00C43D881120"), 9);
        Assert(Eval("D8005AC2A8F0"), 1);
        Assert(Eval("F600BC2D8F"), 0);
        Assert(Eval("9C005AC2F8F0"), 0);
        Assert(Eval("9C0141080250320F1802104A08"), 1);
        return Eval(Input);
    }
}
