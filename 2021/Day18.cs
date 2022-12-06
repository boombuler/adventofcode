namespace AdventOfCode._2021;

using System.IO;

class Day18 : Solution
{
    class SFNumber
    {
        private SFNumber fLeft;
        private SFNumber fRight;
        public SFNumber Parent { get; private set; }
        public SFNumber Left
        {
            get => fLeft;
            set => SetNode(ref fLeft, value);
        }
        public SFNumber Right
        {
            get => fRight;
            set => SetNode(ref fRight, value);
        }
        public int Value { get; set; }
        public bool IsLeaf => Left == null;

        public long Magnitude => IsLeaf ? Value : (3 * Left.Magnitude + 2 * Right.Magnitude);

        private SFNumber() { }

        public static SFNumber Parse(string input)
        {
            using var rd = new StringReader(input);
            var stack = new Stack<SFNumber>();
            while (rd.TryRead(out var c))
            {
                switch (c)
                {
                    case ',': break;
                    case '[': stack.Push(new SFNumber()); break;
                    case ']':
                        var (right, left) = (stack.Pop(), stack.Pop());
                        var node = stack.Peek();
                        node.Left = left;
                        node.Right = right;
                        break;
                    default:
                        stack.Push(new SFNumber() { Value = c - '0' }); break;
                }
            }
            return stack.Peek();
        }

        private void SetNode(ref SFNumber valueStore, SFNumber newValue)
        {
            if (valueStore != null)
                valueStore.Parent = null;
            valueStore = newValue;
            if (valueStore != null)
                valueStore.Parent = this;
        }

        public SFNumber Reduce()
        {
            while (Explode(0) || Split()) 
                /* do nothing */;
            return this;
        }

        private static SFNumber Neighbour(SFNumber node, Func<SFNumber, SFNumber> wantedSide, Func<SFNumber, SFNumber> otherSide)
        {
            while (node.Parent != null && otherSide(node.Parent) != node)
                node = node.Parent;

            if (node.Parent == null)
                return null;

            node = wantedSide(node.Parent);
            while (!node.IsLeaf)
                node = otherSide(node);
            return node;
        }

        private bool Explode(int lvl)
        {
            if (IsLeaf)
                return false;

            if (lvl >= 4 && Left.IsLeaf && Right.IsLeaf)
            {
                var rN = Neighbour(Right, n => n.Right, n => n.Left);
                if (rN != null)
                    rN.Value += Right.Value;
                var lN = Neighbour(Left, n => n.Left, n => n.Right);
                if (lN != null)
                    lN.Value += Left.Value;
                Left = Right = null;
                Value = 0;
                return true;
            }
            return Left.Explode(lvl + 1) || Right.Explode(lvl + 1);
        }

        private bool Split()
        {
            if (!IsLeaf)
                return Left.Split() || Right.Split();
            if (Value >= 10)
            {
                var res = ((decimal)Value) / 2;
                Left = new SFNumber() { Value = (int)Math.Floor(res) };
                Right = new SFNumber() { Value = (int)Math.Ceiling(res) };
                return true;
            }
            return false;
        }

        public static SFNumber Add(SFNumber a, SFNumber b)
            => new SFNumber() { Left = a, Right = b }.Reduce();

        public override string ToString()
        {
            if (Left == null)
                return Value.ToString();
            return $"[{Left},{Right}]";
        }
    }

    protected override long? Part1()
    {
        string TestReduce(string input)
            => SFNumber.Parse(input).Reduce().ToString();

        Assert(TestReduce("[[[[[9,8],1],2],3],4]"), "[[[[0,9],2],3],4]", "Explode 1");
        Assert(TestReduce("[7,[6,[5,[4,[3,2]]]]]"), "[7,[6,[5,[7,0]]]]", "Explode 2");
        Assert(TestReduce("[[6,[5,[4,[3,2]]]],1]"), "[[6,[5,[7,0]]],3]", "Explode 3");
        Assert(TestReduce("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]"), "[[3,[2,[8,0]]],[9,[5,[7,0]]]]", "Explode 4");
        Assert(TestReduce("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]"), "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", "Reduce");
        Assert(SFNumber.Parse("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]").Magnitude, 1384, "Magnitude");

        SFNumber AddAllLines(string input)
            => input.Lines().Select(SFNumber.Parse).Aggregate(SFNumber.Add);

        Assert(AddAllLines(Sample()).ToString(), "[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]", "Addition");
        return AddAllLines(Input).Magnitude;
    }

    protected override long? Part2()
    {
        static long LargestMag(string input)
            => input.Lines()
                .Pairs()
                .SelectMany(r => new[] { (r.A, r.B), (A: r.B, B: r.A) })
                .Max(tpl => SFNumber.Add(SFNumber.Parse(tpl.A), SFNumber.Parse(tpl.B)).Magnitude);
        Assert(LargestMag(Sample()), 3993);
        return LargestMag(Input);
    }
}
