using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntLimit
    {
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

        public virtual object Clone
        {
            get
            {
                var clone = new IntLimit();
                clone.Fill(this);
                return clone;
            }
        }

        public virtual object Fill(IntLimit source)
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

        public string AsString()
        {
            if (!minLimit && !maxLimit)
                return string.Empty;
            var limit = "[";
            if (minLimit)
                limit += minValue;
            limit += ";";
            if (maxLimit)
                limit += maxValue;
            limit += "]";
            return limit;
        }
    }
}
