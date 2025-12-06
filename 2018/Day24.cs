namespace AdventOfCode._2018;

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

class Day24 : Solution
{
    class Group
    {
        public static readonly Func<string, Group?> Parse = new Regex(@"(?<units>\d+) units each with (?<hp>\d+) hit points (\(((; )?((immune to ((, )?(?<immune>\w+))+)|(weak to ((, )?(?<weak>\w+))+)))+\) )?with an attack that does (?<damage>\d+) (?<damageType>\w+) damage at initiative (?<initiative>\d+)").ToFactory<Group>();
        public int Units { get; private set; }
        public int HP { get; private set; }
        public FrozenSet<string> Weaknesses { get; }
        public FrozenSet<string> Immunities { get; }
        public int Damage { get; private set; }
        public string DamageType { get; private set; }
        public int Initiative { get; private set; }
        public int EffectivePower => Units * Damage;
        public bool ImmuneSystem { get; set; }

        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by factory (reflection)")]
        private Group(int units, int hp, string[] immune, string[] weak, int damage, string damageType, int initiative)
        {
            Units = units;
            HP = hp;
            Weaknesses = new HashSet<string>(weak ?? Enumerable.Empty<string>()).ToFrozenSet();
            Immunities = new HashSet<string>(immune ?? Enumerable.Empty<string>()).ToFrozenSet();
            Damage = damage;
            DamageType = damageType;
            Initiative = initiative;
        }

        public int GetDamageAmount(Group other)
        {
            if (other.ImmuneSystem == ImmuneSystem || other.Immunities.Contains(DamageType))
                return 0;
            if (other.Weaknesses.Contains(DamageType))
                return EffectivePower * 2;
            return EffectivePower;
        }

        public bool TakeDamageFrom(Group other)
        {
            var amount = other.GetDamageAmount(this);
            var deadUnits = Math.Min(Units, amount / HP);
            Units -= deadUnits;
            return deadUnits > 0;
        }

        public void BoostBy(int amount) => Damage += amount;

        public Group? SelectTarget(IEnumerable<Group> groups)
        {
            var (_, max) = groups.MinMaxBy(g => (GetDamageAmount(g), g.EffectivePower, g.Initiative));
            if (GetDamageAmount(max) > 0)
                return max;

            return null;
        }
    }

    private static IEnumerable<Group> ParseGroups(string input)
    {
        var team = string.Empty;
        foreach (var l in input.Lines())
        {
            if (string.IsNullOrWhiteSpace(l))
                continue;
            if (l.EndsWith(':'))
                team = l.TrimEnd(':');
            else
            {
                var g = Group.Parse(l) ?? throw new InvalidInputException();
                g.ImmuneSystem = team == "Immune System";
                yield return g;
            }
        }
    }

    private static (bool Survive, int Units) SimulateFight(string input, int boost = 0)
    {
        var allGroups = ParseGroups(input).ToImmutableList();
        foreach (var grp in allGroups.Where(g => g.ImmuneSystem))
            grp.BoostBy(boost);

        bool keepFighting = true;
        while (keepFighting)
        {
            keepFighting = false;
            // TargetSelection
            var possibleTargets = allGroups;
            var selectedTargets = new Dictionary<Group, Group>();
            foreach (var t in allGroups.OrderByDescending(g => (g.EffectivePower, g.Initiative)))
            {
                var target = t.SelectTarget(possibleTargets);
                if (target == null)
                    continue;
                possibleTargets = possibleTargets.Remove(target);
                selectedTargets[t] = target;
            }

            // Attacking

            foreach (var (att, def) in selectedTargets.OrderByDescending(kvp => kvp.Key.Initiative))
            {
                if (def.TakeDamageFrom(att))
                {
                    if (def.Units == 0)
                        allGroups = allGroups.Remove(def);
                    keepFighting = true;
                }
            }

        }
        return (allGroups.All(g => g.ImmuneSystem), allGroups.Sum(g => g.Units));
    }

    protected override long? Part1()
    {
        Assert(SimulateFight(Sample()).Units, 5216);
        return SimulateFight(Input).Units;
    }

    protected override long? Part2()
    {
        Assert(SimulateFight(Sample(), 1570).Units, 51);

        var (low, high) = (0, int.MaxValue);
        while (true)
        {
            var mid = low + ((high - low) / 2);
            if (mid == low)
                break;
            if (SimulateFight(Input, mid).Survive)
                high = mid;
            else
                low = mid;
        }

        return SimulateFight(Input, high).Units;
    }
}
