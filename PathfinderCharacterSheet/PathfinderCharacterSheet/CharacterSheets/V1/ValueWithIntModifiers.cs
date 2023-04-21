using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ValueWithIntModifiers: IContextValue<int>, IEquatable<ValueWithIntModifiers>, IPrototype<ValueWithIntModifiers>
    {
        public int baseValue = 0;
        public IntModifiersList modifiers = new IntModifiersList();

        public virtual int GetValue(CharacterSheet context)
        {
            var value = baseValue;
            if (modifiers != null)
                value += modifiers.GetValue(context);
            return value;
        }

        public virtual ValueWithIntModifiers Clone
        {
            get
            {
                var clone = new ValueWithIntModifiers();
                clone.Fill(this);
                return clone;
            }
        }

        public ValueWithIntModifiers Fill(ValueWithIntModifiers source)
        {
            if (source == null)
                return this;
            baseValue = source.baseValue;
            modifiers = source.modifiers?.Clone;
            return this;
        }

        public bool Equals(ValueWithIntModifiers other)
        {
            if (other == null)
                return false;
            if (baseValue != other.baseValue)
                return false;
            if (modifiers != other.modifiers)
                return false;
            return true;
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as ValueWithIntModifiers);
        }

        public static bool operator ==(ValueWithIntModifiers first, ValueWithIntModifiers second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(ValueWithIntModifiers first, ValueWithIntModifiers second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + baseValue.GetHashCode();
            hash = (hash * 7) + (modifiers is null ? 0 : modifiers.GetHashCode());
            return hash;
        }
    }
}
