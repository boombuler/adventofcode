using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace _2015_22
{
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
            
            foreach((var spell, var duration) in ActiveSpells)
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
}
