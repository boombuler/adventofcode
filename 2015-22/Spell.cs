using System;
using System.Collections.Generic;
using System.Text;

namespace _2015_22
{
    class Spell
    {
        public string Name { get; }
        public int Cost { get; }
        public int Damage { get; }
        public int Heal { get; }
        public int Armor { get; }
        public int Mana { get; }
        public int Duration { get; }

        public Spell(string name, int cost, int dmg = 0, int heal = 0, int armor = 0, int mana = 0, int duration = 0)
            => (Name, Cost, Damage, Heal, Armor, Mana, Duration) = (name, cost, dmg, heal, armor, mana, duration);

        public static readonly Spell MagicMissile = new Spell("Magic Missile", cost: 53, dmg: 4, duration: 1);
        public static readonly Spell Drain = new Spell("Drain", cost: 73, dmg: 2, heal: 2, duration: 1);
        public static readonly Spell Shield = new Spell("Shield", cost: 113, armor: 7, duration: 6);
        public static readonly Spell Poison = new Spell("Poison", cost: 173, dmg: 3, duration: 6);
        public static readonly Spell Recharge = new Spell("Recharge", cost: 229, mana: 101, duration: 5);

        public static IEnumerable<Spell> AllSpells = new[]
        {
            MagicMissile, Drain, Shield, Poison, Recharge
        };

        public override string ToString() => Name;
    }
}
