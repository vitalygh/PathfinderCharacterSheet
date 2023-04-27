using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntMultiplier: IApplicable<int>, IEquatable<IntMultiplier>, IPrototype<IntMultiplier>
    {
        public static readonly IntMultiplier Empty = new IntMultiplier();

        public int additionalBefore = 0;
        public int multiplier = 1;
        public int divider = 1;
        public static readonly RoundingType DefaultRounding = RoundingType.Up;
        public string roundingType = DefaultRounding.ToString();
        internal RoundingType RoundingType
        {
            get { return Helpers.GetEnumValue(roundingType, DefaultRounding); }
            set { roundingType = value.ToString(); }
        }

        public int additionalAfter = 0;

        public IntLimit limit
        {
            get => Limit == IntLimit.Empty ? null : Limit;
            set => Limit = value;
        }
        internal IntLimit Limit { get; set; } = null;

        public int Apply(int value)
        {
            float fval = multiplier * (value + additionalBefore);
            if (divider != 0)
                fval /= divider;
            fval += additionalAfter;
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

            additionalBefore = source.additionalBefore;
            multiplier = source.multiplier;
            divider = source.divider;
            additionalAfter = source.additionalAfter;
            RoundingType = source.RoundingType;
            Limit = source.Limit?.Clone;

            return this;
        }

        public bool Equals(IntMultiplier other)
        {
            if (other == null)
                return false;
            if (additionalBefore != other.additionalBefore)
                return false;
            if (multiplier != other.multiplier)
                return false;
            if (divider != other.divider)
                return false;
            if (additionalAfter != other.additionalAfter)
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
            hash = (hash * 7) + additionalBefore.GetHashCode();
            hash = (hash * 7) + multiplier.GetHashCode();
            hash = (hash * 7) + divider.GetHashCode();
            hash = (hash * 7) + additionalAfter.GetHashCode();
            hash = (hash * 7) + RoundingType.GetHashCode();
            hash = (hash * 7) + (Limit is null ? 0 : Limit.GetHashCode());
            return hash;
        }

        public string AsString(string applyTo)
        {
            if (string.IsNullOrWhiteSpace(applyTo))
                applyTo = "x";
            var text = new StringBuilder(applyTo);
            if (additionalBefore != 0)
            {
                if (additionalBefore > 0)
                    text.Append(" + ").Append(additionalBefore);
                else
                    text.Append(" - ").Append(Math.Abs(additionalBefore));
                text.Insert(0, "(").Append(")");
            }
            var addBrackets = false;
            if (multiplier != 1)
            {
                text.Insert(0, " * ").Insert(0, multiplier.ToString());
                addBrackets = true;
            }
            if (divider != 1)
            {
                text.Append(" / ").Append(divider);
                addBrackets = true;
            }
            if (divider == 0)
                text = new StringBuilder("Infinity");
            if (additionalAfter != 0)
            {
                if (additionalAfter > 0)
                    text.Append(" + ").Append(additionalAfter);
                else
                    text.Append(" - ").Append(Math.Abs(additionalAfter));
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
