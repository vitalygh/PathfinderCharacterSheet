using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1 
{
    public class GearItem: ItemWithUseLimit, IEquatable<GearItem>
    {
        public bool active = false;
        public ValueWithIntModifiers amount = new ValueWithIntModifiers() { baseValue = 1, };
        public ValueWithIntModifiers weight = new ValueWithIntModifiers();

        public int TotalWeight(CharacterSheet sheet) { return amount.GetValue(sheet) * weight.GetValue(sheet); }
        public override string AsString(CharacterSheet sheet)
        {
            var text = string.Empty;
            var count = amount.GetValue(sheet);
            if (count > 1)
                text += "(" + count + ")";

            if (!string.IsNullOrWhiteSpace(name))
                text += " ";
            text += name;

            if (hasUseLimit)
            {
                if (!string.IsNullOrWhiteSpace(text))
                    text += " ";
                var ul = useLimit.GetValue(sheet);
                text += "[" + ul;
                var dul = dailyUseLimit.GetValue(sheet);
                if (dul > 0)
                    text += " / " + dul;
                text += "]";
            }

            return text;
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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, amount) ? amount.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, weight) ? weight.GetHashCode() : 0);
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
