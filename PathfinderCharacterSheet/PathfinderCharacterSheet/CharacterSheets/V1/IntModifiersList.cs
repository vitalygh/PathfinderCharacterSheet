using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntModifiersList: List<IntModifier>, IContextValue<int>, IEquatable<IntModifiersList>, IPrototype<IntModifiersList>
    {

        public IntModifiersList(): base()
        {

        }
            
        public IntModifiersList(IntModifiersList other): base()
        {
            if (other != null)
                AddRange(other);
        }

        public virtual int GetValue(CharacterSheet  context)
        {
            int value = 0;
            foreach (var modifier in this)
            {
                if (modifier is null)
                    continue;
                if (!modifier.active)
                    continue;
                value += modifier.GetValue(context);
            }
            return value;
        }

        public virtual IntModifiersList Clone
        {
            get
            {
                var modifiers = new IntModifiersList();
                modifiers.Fill(this);
                return modifiers;
            }
        }

        public virtual IntModifiersList Fill(IntModifiersList source)
        {
            if (source == null)
                return this;
            Clear();
            foreach (var modifier in source)
                Add((IntModifier)modifier.Clone);
            return this;
        }

        public bool Equals(IntModifiersList other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return System.Linq.Enumerable.SequenceEqual(this, other);
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as IntModifiersList);
        }

        public static bool operator ==(IntModifiersList first, IntModifiersList second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(IntModifiersList first, IntModifiersList second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
