using System;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class GearItem: ItemWithUseLimit, IEquatable<GearItem>
    {
        public bool active = false;
        public ValueWithIntModifiers amount = new ValueWithIntModifiers() { BaseValue = 1, };
        public ValueWithIntModifiers weight = new ValueWithIntModifiers();

        public int TotalWeight(CharacterSheet sheet) { return amount.GetValue(sheet) * weight.GetValue(sheet); }
        public override string AsString(CharacterSheet sheet)
        {
            var text = new StringBuilder();
            var count = amount.GetValue(sheet);
            if (count > 1)
                text.Append('(').Append(count).Append(')');

            if (!string.IsNullOrWhiteSpace(name))
                text.Append(' ');
            text.Append(name);

            if (hasUseLimit)
            {
                if (!string.IsNullOrWhiteSpace(text.ToString()))
                    text.Append(' ');
                var ul = useLimit.GetValue(sheet);
                text.Append('[').Append(ul);
                var dul = dailyUseLimit.GetValue(sheet);
                if (dul > 0)
                    text.Append(" / ").Append(dul);
                text.Append(']');
            }

            return text.ToString();
        }

        public override ItemWithDescription Clone
        {
            get
            {
                var clone = new GearItem();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(GearItem other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (active != other.active)
                return false;
            if (amount != other.amount)
                return false;
            if (weight != other.weight)
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
            return Equals(other as GearItem);
        }

        public static bool operator ==(GearItem first, GearItem second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(GearItem first, GearItem second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + active.GetHashCode();
            hash = (hash * 7) + (amount is null ? 0 : amount.GetHashCode());
            hash = (hash * 7) + (weight is null ? 0 : weight.GetHashCode());
            return hash;
        }

        public GearItem Fill(GearItem source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            active = source.active;
            amount = source.amount?.Clone;
            weight = source.weight?.Clone;
            return this;
        }
    }
}
