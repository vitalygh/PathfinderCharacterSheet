using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntLimit: IApplicable<int>, IEquatable<IntLimit>, IPrototype<IntLimit>
    {
        public static readonly IntLimit Empty = new IntLimit();

        public bool minLimit = false;
        public int minValue = 0;
        public bool maxLimit = false;
        public int maxValue = 0;

        public int Apply(int value)
        {
            if (minLimit)
                value = Math.Max(minValue, value);
            if (maxLimit)
                value = Math.Min(maxValue, value);
            return value;
        }

        public virtual IntLimit Clone
        {
            get
            {
                var clone = new IntLimit();
                clone.Fill(this);
                return clone;
            }
        }

        public virtual IntLimit Fill(IntLimit source)
        {
            if (source == null)
                return this;

            minLimit = source.minLimit;
            minValue = source.minValue;
            maxLimit = source.maxLimit;
            maxValue = source.maxValue;

            return this;
        }

        public bool Equals(IntLimit other)
        {
            if (other == null)
                return false;
            if (minLimit != other.minLimit)
                return false;
            if (minValue != other.minValue)
                return false;
            if (maxLimit != other.maxLimit)
                return false;
            if (maxValue != other.maxValue)
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
            return Equals(other as IntLimit);
        }

        public static bool operator ==(IntLimit first, IntLimit second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(IntLimit first, IntLimit second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + minLimit.GetHashCode();
            hash = (hash * 7) + minValue.GetHashCode();
            hash = (hash * 7) + maxLimit.GetHashCode();
            hash = (hash * 7) + maxValue.GetHashCode();
            return hash;
        }

        public string AsString()
        {
            if (!minLimit && !maxLimit)
                return string.Empty;
            var limit = new StringBuilder("[");
            if (minLimit)
                limit.Append(minValue);
            limit.Append(";");
            if (maxLimit)
                limit.Append(maxValue);
            limit.Append("]");
            return limit.ToString();
        }
    }
}
