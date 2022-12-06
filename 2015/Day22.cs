namespace AdventOfCode._2015;

class Day22 : Solution
{
    record Spell(int ID, int Cost, int Damage = 0, int Heal = 0, int Armor = 0, int Mana = 0, int Duration = 0)
    {
        public static readonly Spell MagicMissile = new(0, Cost: 53, Damage: 4, Duration: 1);
        public static readonly Spell Drain = new(1, Cost: 73, Damage: 2, Heal: 2, Duration: 1);
        public static readonly Spell Shield = new(2, Cost: 113, Armor: 7, Duration: 6);
        public static readonly Spell Poison = new(3, Cost: 173, Damage: 3, Duration: 6);
        public static readonly Spell Recharge = new(4, Cost: 229, Mana: 101, Duration: 5);

        public static Spell[] AllSpells { get; } = new[]
        {
                MagicMissile, Drain, Shield, Poison, Recharge
            };
    }

    record GameState(int Mana, int PlayerHP, int BossHP)
    {
        public int TotalManaSpent { get; private init; }
        public ulong ActiveSpells { get; private init; }
        public bool? Won
        {
            get
            {
                if (BossHP > 0 && PlayerHP > 0)
                    return null;
                return BossHP <= 0;
            }
        }

        public GameState TickBoss(int bossDmg)
        {
            GameState state = TickSpells();
            if (state.Won.HasValue)
                return state;

            var armor = GetRemainingTime(Spell.Shield.ID) > 0 ? Spell.Shield.Armor : 0;

            return state with
            {
                PlayerHP = state.PlayerHP - Math.Max(1, bossDmg - armor),
            };
        }

        private int GetRemainingTime(int spellId)
            => (int)((ActiveSpells >> (spellId * 3)) & 0b111);

        private static ulong UpdateSpellTime(ulong spellTimes, int spellId, int newDuration)
        {
            var mask = ~(0b111UL << (spellId * 3));
            var value = ((ulong)newDuration) << (spellId * 3);
            return (spellTimes & mask) | (value);
        }

        private GameState TickMalus(int malus)
        {
            if (malus == 0)
                return this;
            return this with { PlayerHP = PlayerHP - malus };
        }
        private GameState TickSpells()
        {
            if (ActiveSpells == 0)
                return this;
            int dmg = 0, heal = 0, mana = 0;

            ulong newActiveSpells = 0;

            foreach (var spell in Spell.AllSpells)
            {
                var dur = GetRemainingTime(spell.ID);
                if (dur == 0)
                    continue;

                dmg += spell.Damage;
                heal += spell.Heal;
                mana += spell.Mana;
                newActiveSpells = UpdateSpellTime(newActiveSpells, spell.ID, dur - 1);
            }

            return this with
            {
                Mana = Mana + mana,
                PlayerHP = PlayerHP + heal,
                BossHP = BossHP - dmg,
                ActiveSpells = newActiveSpells
            };
        }
        public GameState TickPlayer(Spell spell, int malus)
        {
            var state = TickMalus(malus);
            if (state.Won.HasValue)
                return state;

            state = state.TickSpells();

            return state with
            {
                Mana = state.Mana - spell.Cost,
                ActiveSpells = UpdateSpellTime(state.ActiveSpells, spell.ID, spell.Duration),
                TotalManaSpent = state.TotalManaSpent + spell.Cost
            };
        }

        public bool ValidSpell(Spell s)
        {
            if (s.Cost > Mana)
                return false;
            return GetRemainingTime(s.ID) <= 1;
        }
    }

    private static long PlayGame(int mana, int playerHP, int bossHP, int bossDmg, int malus = 0)
    {
        var gs = new GameState(mana, playerHP, bossHP);

        int minMana = int.MaxValue;

        var openGames = new Queue<GameState>();
        openGames.Enqueue(gs);

        while (openGames.TryDequeue(out var curGame))
        {
            foreach (var spell in Spell.AllSpells.Where(curGame.ValidSpell))
            {
                var g = curGame.TickPlayer(spell, malus);
                if (g.TotalManaSpent >= minMana)
                    continue;

                if (g.Won == true)
                    minMana = g.TotalManaSpent;

                g = g.TickBoss(bossDmg);
                switch (g.Won)
                {
                    case false: continue;
                    case true:
                        minMana = g.TotalManaSpent; break;
                    default:
                        openGames.Enqueue(g); break;
                }
            }
        }
        return minMana;
    }

    private int GetStat(string name)
        => Input.Lines().Where(l => l.StartsWith(name + ": ")).Select(line => int.Parse(line[(name.Length + 2)..])).FirstOrDefault();

    private int BossHitPoints => GetStat("Hit Points");
    private int BossDamage => GetStat("Damage");

    protected override long? Part1()
    {
        Assert(PlayGame(250, 10, 13, 8), 226);
        Assert(PlayGame(250, 10, 14, 8), 641);
        return PlayGame(500, 50, BossHitPoints, BossDamage);
    }

    protected override long? Part2() => PlayGame(500, 50, BossHitPoints, BossDamage, 1);
}
