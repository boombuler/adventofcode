namespace AdventOfCode._2018;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day08 : Solution
{
    record TreeNode(TreeNode[] Children, int[] Metadata)
    {
        public long MetadataSum => Metadata.Sum() + Children.Sum(c => c.MetadataSum);

        public long Value =>
            (Children.Length == 0) ?
                Metadata.Sum() :
                Metadata.Where(m => m <= Children.Length).Sum(m => Children[m - 1].Value);
    }

    private static TreeNode ReadTree(string input)
    {
        static (TreeNode Node, IEnumerable<int>) ReadNode(IEnumerable<int> items)
        {
            int childCount, metaCount;
            (childCount, (metaCount, items)) = items;
            var childs = new TreeNode[childCount];
            for (int i = 0; i < childCount; i++)
                (childs[i], items) = ReadNode(items);
            return (new TreeNode(childs, items.Take(metaCount).ToArray()), items.Skip(metaCount));
        }
        return ReadNode(input.Split(' ').Select(int.Parse)).Node;
    }

    protected override long? Part1()
    {
        Assert(ReadTree(Sample()).MetadataSum, 138);
        return ReadTree(Input).MetadataSum;
    }

    protected override long? Part2()
    {
        Assert(ReadTree(Sample()).Value, 66);
        return ReadTree(Input).Value;
    }
}
