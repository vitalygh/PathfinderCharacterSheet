#define SAVE_DELTA
using System;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntLimit: IApplicable<int>, IEquatable<IntLimit>, IPrototype<IntLimit>
    {
        private static readonly IntLimit Default = new IntLimit();
        public const bool DefaultMinLimit = false;
        public string minLimit
        {
            get =>
#if SAVE_DELTA
                DefaultMinLimit == MinLimit ? null :
#endif
                MinLimit.ToString();
            set => MinLimit = bool.TryParse(value, out bool outValue) ? outValue : DefaultMinLimit;
        }
        internal bool MinLimit { get; set; } = DefaultMinLimit;

        public const int DefaultMinValue = 0;
        public string minValue
        {
            get =>
#if SAVE_DELTA
                DefaultMinValue == MinValue ? null :
#endif
                MinValue.ToString();
            set => MinValue = int.TryParse(value, out int outValue) ? outValue : DefaultMinValue;
        }
        internal int MinValue { get; set; } = DefaultMinValue;

        public const bool DefaultMaxLimit = false;
        public string maxLimit
        {
            get =>
#if SAVE_DELTA
                DefaultMaxLimit == MaxLimit ? null :
#endif
                MaxLimit.ToString();
            set => MaxLimit = bool.TryParse(value, out bool outValue) ? outValue : DefaultMaxLimit;
        }
        internal bool MaxLimit { get; set; } = DefaultMaxLimit;

        public const int DefaultMaxValue = 0;
        public string maxValue
        {
            get =>
#if SAVE_DELTA
                DefaultMaxValue == MaxValue ? null :
#endif
                MaxValue.ToString();
            set => MaxValue = int.TryParse(value, out int outValue) ? outValue : DefaultMaxValue;
        }
        internal int MaxValue { get; set; } = DefaultMaxValue;

        public static bool HasDefaultValue(IntLimit other)
        {
            return Default.Equals(other);
        }

        public int Apply(int value)
        {
            if (MinLimit)
                value = Math.Max(MinValue, value);
            if (MaxLimit)
                value = Math.Min(MaxValue, value);
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

            MinLimit = source.MinLimit;
            MinValue = source.MinValue;
            MaxLimit = source.MaxLimit;
            MaxValue = source.MaxValue;

            return this;
        }

        public bool Equals(IntLimit other)
        {
            if (other == null)
                return false;
            if (MinLimit != other.MinLimit)
                return false;
            if (MinValue != other.MinValue)
                return false;
            if (MaxLimit != other.MaxLimit)
                return false;
            if (MaxValue != other.MaxValue)
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
            hash = (hash * 7) + MinLimit.GetHashCode();
            hash = (hash * 7) + MinValue.GetHashCode();
            hash = (hash * 7) + MaxLimit.GetHashCode();
            hash = (hash * 7) + MaxValue.GetHashCode();
            return hash;
        }

        public string AsString()
        {
            if (!MinLimit && !MaxLimit)
                return string.Empty;
            var limit = new StringBuilder('[');
            if (MinLimit)
                limit.Append(MinValue);
            limit.Append(';');
            if (MaxLimit)
                limit.Append(MaxValue);
            limit.Append(']');
            return limit.ToString();
        }
    }
}
