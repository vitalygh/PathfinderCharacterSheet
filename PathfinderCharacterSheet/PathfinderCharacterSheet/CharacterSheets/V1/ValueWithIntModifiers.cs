//#define OPTIMIZE
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ValueWithIntModifiers: IContextValue<int, CharacterSheet>, IEquatable<ValueWithIntModifiers>, IPrototype<ValueWithIntModifiers>
    {
        public const int DefaultBaseValue = 0;
        public string baseValue
        {
            get => DefaultBaseValue == BaseValue ? null : BaseValue.ToString();
            set => BaseValue = int.TryParse(value, out int outValue) ? outValue : DefaultBaseValue;
        }
        internal int BaseValue { get; set; } = DefaultBaseValue;
        public IntModifiersList modifiers = null;

#if OPTIMIZE
        private static readonly List<ValueWithIntModifiers> allValuesWithIntModifiers = new List<ValueWithIntModifiers>();
        public ValueWithIntModifiers() : base()
        {
            allValuesWithIntModifiers.Add(this);
        }

        public static void Optimize()
        {
            foreach (var valueWithIntModifiers in allValuesWithIntModifiers)
            {
                if (valueWithIntModifiers is null)
                    continue;
                valueWithIntModifiers.Fill(valueWithIntModifiers);
                if (valueWithIntModifiers.modifiers is null)
                    continue;
                if (valueWithIntModifiers.modifiers.Count > 0)
                    continue;
                valueWithIntModifiers.modifiers = null;
            }
            allValuesWithIntModifiers.Clear();
        }
#endif


        public virtual int GetValue(CharacterSheet context)
        {
            var value = BaseValue;
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
            BaseValue = source.BaseValue;
            modifiers = source.modifiers?.Clone;
            return this;
        }

        public bool Equals(ValueWithIntModifiers other)
        {
            if (other == null)
                return false;
            if (BaseValue != other.BaseValue)
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
            hash = (hash * 7) + BaseValue.GetHashCode();
            hash = (hash * 7) + (modifiers is null ? 0 : modifiers.GetHashCode());
            return hash;
        }
    }
}
