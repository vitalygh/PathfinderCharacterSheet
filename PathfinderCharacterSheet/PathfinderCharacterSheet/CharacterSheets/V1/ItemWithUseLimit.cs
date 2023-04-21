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
            var text = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(name))
                text.Append(name);

            if (hasUseLimit)
            {
                if (!string.IsNullOrWhiteSpace(text.ToString()))
                    text.Append(" ");
                var ul = useLimit.GetValue(sheet);
                text.Append("[").Append(ul);
                var dul = dailyUseLimit.GetValue(sheet);
                if (dul > 0)
                    text.Append(" / ").Append(dul);
                text.Append("]");
            }

            return text.ToString();
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
            if (other is null)
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
            if (first is null)
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
            hash = (hash * 7) + (useLimit is null ? 0 : useLimit.GetHashCode());
            hash = (hash * 7) + (dailyUseLimit is null ? 0 : dailyUseLimit.GetHashCode());
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
