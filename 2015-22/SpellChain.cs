using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace _2015_22
{
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

        public override string ToString() => string.Join(" -> ", this.Reverse());
    }
}
