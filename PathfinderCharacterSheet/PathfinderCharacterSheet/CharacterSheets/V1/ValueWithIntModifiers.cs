#define OPTIMIZE
#define SAVE_DELTA
using System;
using System.Collections.Generic;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ValueWithIntModifiers: IContextValue<int, CharacterSheet>, IEquatable<ValueWithIntModifiers>, IPrototype<ValueWithIntModifiers>
    {
        public const int DefaultBaseValue = 0;
        public string baseValue
        {
            get =>
#if SAVE_DELTA
                DefaultBaseValue == BaseValue ? null :
#endif
                BaseValue.ToString();
            set => BaseValue = int.TryParse(value, out int outValue) ? outValue : DefaultBaseValue;
        }
        internal int BaseValue { get; set; } = DefaultBaseValue;
        public IntModifiersList modifiers
        {
            get =>
#if SAVE_DELTA
                Modifiers?.Count <= 0 ? null :
#endif
                Modifiers;
            set => Modifiers = value;
        }
        internal IntModifiersList Modifiers { get; set; } = new IntModifiersList();


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
            }
            allValuesWithIntModifiers.Clear();
        }
#endif


        public virtual int GetValue(CharacterSheet context)
        {
            var value = BaseValue;
            if (Modifiers != null)
                value += Modifiers.GetValue(context);
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
            Modifiers = source.Modifiers?.Clone;
            return this;
        }

        public bool Equals(ValueWithIntModifiers other)
        {
            if (other == null)
                return false;
            if (BaseValue != other.BaseValue)
                return false;
            if (Modifiers != other.Modifiers)
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
            hash = (hash * 7) + (Modifiers is null ? 0 : Modifiers.GetHashCode());
            return hash;
        }
    }
}
