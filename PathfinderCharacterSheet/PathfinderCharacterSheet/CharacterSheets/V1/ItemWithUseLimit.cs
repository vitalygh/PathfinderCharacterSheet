using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ItemWithUseLimit : ItemWithDescription, IEquatable<ItemWithUseLimit>
    {
        public bool hasUseLimit = false;
        public ValueWithIntModifiers useLimit = new ValueWithIntModifiers();
        public ValueWithIntModifiers dailyUseLimit = new ValueWithIntModifiers();

        public override string AsString(CharacterSheet sheet)
        {
            var text = name;

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
                var clone = new ItemWithUseLimit();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(ItemWithUseLimit other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (hasUseLimit != other.hasUseLimit)
                return false;
            if (!useLimit.Equals(other.useLimit))
                return false;
            if (!dailyUseLimit.Equals(other.dailyUseLimit))
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
            return Equals(other as ItemWithUseLimit);
        }

        public static bool operator ==(ItemWithUseLimit first, ItemWithUseLimit second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(ItemWithUseLimit first, ItemWithUseLimit second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + hasUseLimit.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, useLimit) ? useLimit.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, dailyUseLimit) ? dailyUseLimit.GetHashCode() : 0);
            return hash;
        }

        public ItemWithUseLimit Fill(ItemWithUseLimit source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            hasUseLimit = source.hasUseLimit;
            useLimit = source.useLimit?.Clone;
            dailyUseLimit = source.dailyUseLimit?.Clone;
            return this;
        }
    }
}
