﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntMultiplier: IApplicable<int>, IEquatable<IntMultiplier>, IPrototype<IntMultiplier>
    {
        public const RoundingTypes DefaultRounding = RoundingTypes.Up;

        public int additionalBefore = 0;
        public int multiplier = 1;
        public int divider = 1;
        public string roundingType = DefaultRounding.ToString();
        public RoundingTypes RoundingType
        {
            get { return CharacterSheet.GetEnumValue(roundingType, DefaultRounding); }
            set { roundingType = value.ToString(); }
        }

        public int additionalAfter = 0;

        public IntLimit limit = new IntLimit();

        public int Apply(int value)
        {
            float fval = multiplier * (value + additionalBefore);
            if (divider != 0)
                fval /= divider;
            fval += additionalAfter;
            switch (RoundingType)
            {
                case RoundingTypes.ToNearest:
                    value = (int)Math.Floor(fval + (fval < 0.0f ? -0.5f : 0.5f));
                    break;
                case RoundingTypes.Down:
                    value = (int)Math.Floor(fval);
                    break;
                default:
                    value = (int)Math.Ceiling(fval);
                    break;
            }
            if (limit != null)
                value = limit.Apply(value);
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
            if (source.limit != null)
                limit = source.limit.Clone;

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
            if (limit != null)
            {
                if (!limit.Equals(other.limit))
                    return false;
            }
            else if ((other.limit != null) && !other.limit.Equals(limit))
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
            return Equals(other as IntMultiplier);
        }

        public static bool operator ==(IntMultiplier first, IntMultiplier second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, limit) ? limit.GetHashCode() : 0);
            return hash;
        }

        public string AsString(string applyTo)
        {
            if (string.IsNullOrWhiteSpace(applyTo))
                applyTo = "x";
            if (additionalBefore != 0)
            {
                if (additionalBefore > 0)
                    applyTo += " + " + additionalBefore;
                else
                    applyTo += " - " + Math.Abs(additionalBefore);
                applyTo = "(" + applyTo + ")";
            }
            var addBrackets = false;
            if (multiplier != 1)
            {
                applyTo = multiplier + " * " + applyTo;
                addBrackets = true;
            }
            if (divider != 1)
            {
                applyTo += " / " + divider;
                addBrackets = true;
            }
            if (divider == 0)
                applyTo = "Infinity";
            if (additionalAfter != 0)
            {
                if (additionalAfter > 0)
                    applyTo += " + " + additionalAfter;
                else
                    applyTo += " - " + Math.Abs(additionalAfter);
                addBrackets = true;
            }
            if (addBrackets)
                applyTo = "(" + applyTo + ")";
            applyTo += limit.AsString();
            return applyTo;
        }
    }
}
