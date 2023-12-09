namespace AdventOfCode._2015;

class Day21 : Solution
{
    class Mob
    {
        public int Attack { get; }
        public int HP { get; set; }
        public int Defense { get; }

        private Mob(int a, int h, int d)
            => (Attack, HP, Defense) = (a, h, d);
        public static Mob Boss(string data)
        {
            var stats = data.Lines().ToDictionary(line => line[..line.IndexOf(':')], line => int.Parse(line[(line.IndexOf(':') + 2)..]));
            return new Mob(stats["Damage"], stats["Hit Points"], stats["Armor"]);
        }

        public static Mob Player(int hp, IEnumerable<Item> items)
            => new(items.Sum(i => i.Attack), hp, items.Sum(i => i.Defense));
    }

    class Item
    {
        public int Attack { get; }
        public int Defense { get; }
        public int Cost { get; }
        public string Name { get; }

        public Item(string n, int c, int a, int d)
            => (Name, Cost, Defense, Attack) = (n, c, d, a);
        public override string ToString() => Name;
    }

    private readonly Item[] Weapons =
    [
        new ("Dagger"    ,  8, 4, 0),
        new ("Shortsword", 10, 5, 0),
        new ("Warhammer" , 25, 6, 0),
        new ("Longsword" , 40, 7, 0),
        new ("Greataxe"  , 74, 8, 0),
    ];

    private readonly Item[] Armors =
    [
        new ("Leather"   ,  13, 0, 1),
        new ("Chainmail" ,  31, 0, 2),
        new ("Splintmail",  53, 0, 3),
        new ("Bandedmail",  75, 0, 4),
        new ("Platemail" , 102, 0, 5),
    ];

    private readonly Item[] Rings = 
    [
        new ("Damage +1",  25, 1, 0),
        new ("Damage +2",  50, 2, 0),
        new ("Damage +3", 100, 3, 0),
        new ("Defense +1", 20, 0, 1),
        new ("Defense +2", 54, 0, 2),
        new ("Defense +3", 80, 0, 3),
    ];

    private IEnumerable<IEnumerable<Item>> SelectItems()
    {
        var weapons = Take(Weapons, 1, 1).ToList();
        var armors = Take(Armors, 0, 1).ToList();
        var rings = Take(Rings, 0, 2).ToList();

        foreach (var cWeap in weapons)
        {
            foreach (var cArmor in armors)
            {
                foreach (var cRing in rings)
                    yield return cWeap.Union(cArmor).Union(cRing);
            }
        }
    }

    private static List<Item[]> Take(Span<Item> p, int count)
    {
        var result = new List<Item[]>();
        if (count == 0)
            result.Add([]);
        else
        {
            for (int i = 0; i <= (p.Length - count); i++)
            {
                var itm = new[] { p[i] };
                if (count == 1)
                    result.Add(itm);
                else
                {
                    var others = Take(p[(i + 1)..], count - 1);
                    foreach (var o in others)
                        result.Add(o.Union(itm).ToArray());
                }
            }
        }
        return result;
    }

    private static IEnumerable<Item[]> Take(Item[] pool, int min, int max)
        => Enumerable.Range(min, max - min + 1).SelectMany(c => Take(pool.AsSpan(), c));

    private bool WouldWin(IEnumerable<Item> equip)
    {
        var player = Mob.Player(100, equip);
        var boss = Mob.Boss(Input);

        var def = player;
        var att = boss;
        while (att.HP > 0)
        {
            (att, def) = (def, att);
            var dmg = Math.Max(1, att.Attack - def.Defense);
            def.HP -= dmg;
        }
        return att == boss;
    }

    protected override long? Part1() => SelectItems().Where(WouldWin).Select(itms => itms.Sum(i => i.Cost)).Min();

    protected override long? Part2() => SelectItems().Where(i => !WouldWin(i)).Select(itms => itms.Sum(i => i.Cost)).Max();
}
