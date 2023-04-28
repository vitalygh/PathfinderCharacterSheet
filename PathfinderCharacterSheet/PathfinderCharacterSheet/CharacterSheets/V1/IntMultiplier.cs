#define SAVE_DELTA
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntMultiplier: IApplicable<int>, IEquatable<IntMultiplier>
    {
        public const int DefaultAdditionalBefore = 0;
        public string additionalBefore
        {
            get =>
#if SAVE_DELTA
                DefaultAdditionalBefore == AdditionalBefore ? null :
#endif
                AdditionalBefore.ToString();
            set => AdditionalBefore = int.TryParse(value, out int outValue) ? outValue : DefaultAdditionalBefore;
        }
        internal int AdditionalBefore { get; set; } = DefaultAdditionalBefore;

        public const int DefaultMultiplier = 1;
        public string multiplier
        {
            get =>
#if SAVE_DELTA
                DefaultMultiplier == Multiplier ? null :
#endif
                Multiplier.ToString();
            set => Multiplier = int.TryParse(value, out int outValue) ? outValue : DefaultMultiplier;
        }
        internal int Multiplier { get; set; } = DefaultMultiplier;

        public const int DefaultDivider = 1;
        public string divider
        {
            get =>
#if SAVE_DELTA
                DefaultDivider == Divider ? null :
#endif
                Divider.ToString();
            set => Divider = int.TryParse(value, out int outValue) ? outValue : DefaultDivider;
        }
        internal int Divider { get; set; } = DefaultDivider;

        public const int DefaultAdditionalAfter = 0;
        public string additionalAfter
        {
            get =>
#if SAVE_DELTA
                DefaultAdditionalAfter == AdditionalAfter ? null :
#endif
                AdditionalAfter.ToString();
            set => AdditionalAfter = int.TryParse(value, out int outValue) ? outValue : DefaultAdditionalAfter;
        }
        internal int AdditionalAfter { get; set; } = DefaultAdditionalAfter;

        public const RoundingType DefaultRounding = RoundingType.Up;
        public string roundingType
        {
            get =>
#if SAVE_DELTA
                DefaultRounding == RoundingType ? null :
#endif
                RoundingType.ToString();
            set => RoundingType = Helpers.GetEnumValue(value, DefaultRounding);
        }
        internal RoundingType RoundingType { get; set; } = DefaultRounding;

        public IntLimit limit
        {
            get =>
#if SAVE_DELTA
                Limit == new IntLimit() ? null :
#endif
                Limit;
            set => Limit = value;
        }
        internal IntLimit Limit { get; set; } = null;

        public int Apply(int value)
        {
            float fval = Multiplier * (value + AdditionalBefore);
            if (Divider != 0)
                fval /= Divider;
            fval += AdditionalAfter;
            switch (RoundingType)
            {
                case RoundingType.ToNearest:
                    value = (int)Math.Floor(fval + (fval < 0.0f ? -0.5f : 0.5f));
                    break;
                case RoundingType.Down:
                    value = (int)Math.Floor(fval);
                    break;
                default:
                    value = (int)Math.Ceiling(fval);
                    break;
            }
            if (Limit != null)
                value = Limit.Apply(value);
            return value;
        }

        public virtual IntMultiplier Clone
        {
            get
            {
                var clone = new IntMultiplier();
                clone.Fill(this);
                return clone;
            }
        }

        public virtual IntMultiplier Fill(IntMultiplier source)
        {
            if (source == null)
                return this;

            AdditionalBefore = source.AdditionalBefore;
            Multiplier = source.Multiplier;
            Divider = source.Divider;
            AdditionalAfter = source.AdditionalAfter;
            RoundingType = source.RoundingType;
            Limit = source.Limit?.Clone;

            return this;
        }

        public bool Equals(IntMultiplier other)
        {
            if (other == null)
                return false;
            if (AdditionalBefore != other.AdditionalBefore)
                return false;
            if (Multiplier != other.Multiplier)
                return false;
            if (Divider != other.Divider)
                return false;
            if (AdditionalAfter != other.AdditionalAfter)
                return false;
            if (RoundingType != other.RoundingType)
                return false;
            if (Limit != other.Limit)
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
            return Equals(other as IntMultiplier);
        }

        public static bool operator ==(IntMultiplier first, IntMultiplier second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(IntMultiplier first, IntMultiplier second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + AdditionalBefore.GetHashCode();
            hash = (hash * 7) + Multiplier.GetHashCode();
            hash = (hash * 7) + Divider.GetHashCode();
            hash = (hash * 7) + AdditionalAfter.GetHashCode();
            hash = (hash * 7) + RoundingType.GetHashCode();
            hash = (hash * 7) + (Limit is null ? 0 : Limit.GetHashCode());
            return hash;
        }

        public string AsString(string applyTo)
        {
            if (string.IsNullOrWhiteSpace(applyTo))
                applyTo = "x";
            var text = new StringBuilder(applyTo);
            if (AdditionalBefore != 0)
            {
                if (AdditionalBefore > 0)
                    text.Append(" + ").Append(AdditionalBefore);
                else
                    text.Append(" - ").Append(Math.Abs(AdditionalBefore));
                text.Insert(0, "(").Append(")");
            }
            var addBrackets = false;
            if (Multiplier != 1)
            {
                text.Insert(0, " * ").Insert(0, Multiplier.ToString());
                addBrackets = true;
            }
            if ((Divider != 1) && (Divider != 0))
            {
                text.Append(" / ").Append(Divider);
                addBrackets = true;
            }
            if (AdditionalAfter != 0)
            {
                if (AdditionalAfter > 0)
                    text.Append(" + ").Append(AdditionalAfter);
                else
                    text.Append(" - ").Append(Math.Abs(AdditionalAfter));
                addBrackets = true;
            }
            if (addBrackets)
                text.Insert(0, "(").Append(")");
            if (Limit != null)
                text.Append(Limit.AsString());
            return text.ToString();
        }
    }
}
