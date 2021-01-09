using AdventOfCode.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdventOfCode._2015
{
    class Day22 : Solution
    {
        class Spell
        {
            public int Cost { get; }
            public int Damage { get; }
            public int Heal { get; }
            public int Armor { get; }
            public int Mana { get; }
            public int Duration { get; }

            public Spell(int cost, int dmg = 0, int heal = 0, int armor = 0, int mana = 0, int duration = 0)
                => (Cost, Damage, Heal, Armor, Mana, Duration) = (cost, dmg, heal, armor, mana, duration);

            public static readonly Spell MagicMissile = new Spell(cost: 53, dmg: 4, duration: 1);
            public static readonly Spell Drain = new Spell(cost: 73, dmg: 2, heal: 2, duration: 1);
            public static readonly Spell Shield = new Spell(cost: 113, armor: 7, duration: 6);
            public static readonly Spell Poison = new Spell(cost: 173, dmg: 3, duration: 6);
            public static readonly Spell Recharge = new Spell(cost: 229, mana: 101, duration: 5);

            public static IEnumerable<Spell> AllSpells = new[]
            {
                MagicMissile, Drain, Shield, Poison, Recharge
            };
        }
        
        class SpellChain : IEnumerable<Spell>
        {
            class Enumerator : IEnumerator<Spell>
            {
                private SpellChain fRoot;
                private SpellChain fCurrent;
                public Spell Current => fCurrent?.Spell;

                object IEnumerator.Current => Current;

                public Enumerator(SpellChain c) => fRoot = c;
                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (fCurrent == null)
                        fCurrent = fRoot;
                    else
                        fCurrent = fCurrent?.Previous;
                    return fCurrent != null;
                }

                public void Reset()
                {
                    fCurrent = fRoot;
                }
            }

            public SpellChain Previous { get; }
            public Spell Spell { get; }
            public SpellChain(SpellChain prev, Spell spell)
                => (Previous, Spell) = (prev, spell);

            public IEnumerator<Spell> GetEnumerator() => new Enumerator(this);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class GameState
        {
            public int Mana { get; private set; }
            public int PlayerHP { get; private set; }
            public int BossHP { get; private set; }
            public int TotalManaSpent { get; private set; }

            public ReadOnlyDictionary<Spell, int> ActiveSpells { get; private set; }

            public SpellChain AllCastsSpells { get; private set; }
            private GameState() { }
            public GameState(int mana, int playerHP, int bossHP)
                => (Mana, PlayerHP, BossHP) = (mana, playerHP, bossHP);

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

                var armor = ActiveSpells.Keys.Select(s => s.Armor).Sum();

                return new GameState()
                {
                    Mana = state.Mana,
                    BossHP = state.BossHP,
                    PlayerHP = state.PlayerHP - Math.Max(1, bossDmg - armor),
                    ActiveSpells = state.ActiveSpells,
                    AllCastsSpells = state.AllCastsSpells,
                    TotalManaSpent = state.TotalManaSpent,
                };
            }
            private GameState TickMalus(int malus)
            {
                if (malus == 0)
                    return this;
                return new GameState()
                {
                    Mana = this.Mana,
                    BossHP = this.BossHP,
                    PlayerHP = this.PlayerHP - malus,
                    ActiveSpells = this.ActiveSpells,
                    AllCastsSpells = this.AllCastsSpells,
                    TotalManaSpent = this.TotalManaSpent,
                };
            }
            private GameState TickSpells()
            {
                if (ActiveSpells == null)
                    return this;
                int dmg = 0, heal = 0, mana = 0;

                var newActiveSpells = new Dictionary<Spell, int>();

                foreach ((var spell, var duration) in ActiveSpells)
                {
                    dmg += spell.Damage;
                    heal += spell.Heal;
                    mana += spell.Mana;
                    if (duration > 1)
                        newActiveSpells[spell] = duration - 1;
                }

                return new GameState()
                {
                    Mana = this.Mana + mana,
                    PlayerHP = this.PlayerHP + heal,
                    BossHP = this.BossHP - dmg,
                    ActiveSpells = new ReadOnlyDictionary<Spell, int>(newActiveSpells),
                    AllCastsSpells = this.AllCastsSpells,
                    TotalManaSpent = this.TotalManaSpent,
                };
            }
            public GameState TickPlayer(Spell spell, int malus)
            {
                var state = TickMalus(malus);
                if (state.Won.HasValue)
                    return state;

                state = state.TickSpells();
                var spells = new Dictionary<Spell, int>();
                if (state.ActiveSpells != null)
                {
                    foreach ((var k, var v) in state.ActiveSpells)
                        spells[k] = v;
                }
                spells[spell] = spell.Duration;
                return new GameState()
                {
                    Mana = state.Mana - spell.Cost,
                    PlayerHP = state.PlayerHP,
                    BossHP = state.BossHP,
                    ActiveSpells = new ReadOnlyDictionary<Spell, int>(spells),
                    AllCastsSpells = new SpellChain(state.AllCastsSpells, spell),
                    TotalManaSpent = state.TotalManaSpent + spell.Cost
                };
            }

            public bool ValidSpell(Spell s)
            {
                if (s.Cost > Mana)
                    return false;
                if (ActiveSpells != null && ActiveSpells.TryGetValue(s, out int time))
                    return time <= 1;
                return true;
            }

        }

        private long PlayGame(int mana, int playerHP, int bossHP, int bossDmg, int malus = 0, bool WriteSpells = false)
        {
            var gs = new GameState(mana, playerHP, bossHP);


            int minMana = int.MaxValue;
            SpellChain minSpells = null;

            Queue<GameState> openGames = new Queue<GameState>();
            openGames.Enqueue(gs);

            while(openGames.TryDequeue(out var curGame))
            {
                foreach(var spell in Spell.AllSpells.Where(curGame.ValidSpell))
                {
                    var g = curGame.TickPlayer(spell, malus);
                    if (g.TotalManaSpent >= minMana)
                        continue;
                    if (g.Won == true)
                    {
                        minMana = g.TotalManaSpent;
                        minSpells = g.AllCastsSpells; 
                    }
                    g = g.TickBoss(bossDmg);
                    switch (g.Won)
                    {
                        case true:
                            minMana = g.TotalManaSpent;
                            minSpells = g.AllCastsSpells;
                            break;
                        case false:
                            continue;
                        default:
                            openGames.Enqueue(g); break;
                    }
                }
            }
            return minMana;
        }

        private int GetStat(string name)
            => Input.Lines().Where(l => l.StartsWith(name + ": ")).Select(l => int.Parse(l.Substring(name.Length + 2))).FirstOrDefault();

        private int BossHitPoints => GetStat("Hit Points");
        private int BossDamage => GetStat("Damage");

        protected override long? Part1()
        {
            Assert(PlayGame(250, 10, 13, 8), 226);
            Assert(PlayGame(250, 10, 14, 8), 641);
            return PlayGame(500, 50, BossHitPoints, BossDamage);
        }

        protected override long? Part2()
        {
            return PlayGame(500, 50, BossHitPoints, BossDamage, 1);
        }
    }
}
