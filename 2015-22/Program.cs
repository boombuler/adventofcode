using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2015_22
{
    class Program : ProgramBase
    {
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
            if (WriteSpells)
            {
                Debug(minSpells ?? (object)"No Winning Combination");
            }
            if (minSpells == null)
                return 0;
            return minMana;
        }

        static void Main(string[] args) => new Program().Run();

        protected override long? Part1()
        {
            Assert(PlayGame(250, 10, 13, 8), 226);
            Assert(PlayGame(250, 10, 14, 8), 641);
            return PlayGame(500, 50, 71, 10, WriteSpells: true);
        }

        protected override long? Part2()
        {
            return PlayGame(500, 50, 71, 10, 1, WriteSpells: true);
        }
    }
}
