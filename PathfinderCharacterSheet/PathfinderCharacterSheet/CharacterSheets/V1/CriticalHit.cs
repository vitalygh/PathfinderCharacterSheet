using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class CriticalHit: IPrototype<CriticalHit>, IEquatable<CriticalHit>
    {
        public ValueWithIntModifiers min = new ValueWithIntModifiers();
        public ValueWithIntModifiers max = new ValueWithIntModifiers();
        public ValueWithIntModifiers multiplier = new ValueWithIntModifiers();
        public string AsString(CharacterSheet sheet)
        {
            var mint = min.GetValue(sheet);
            var maxt = max.GetValue(sheet);
            var mul = multiplier.GetValue(sheet);
            if (mul <= 0)
                return string.Empty;
            var crit = new StringBuilder();
            if (mint < maxt)
                crit.Append(mint).Append("-");
            crit.Append(maxt);
            crit.Append("/x").Append(mul);
            return crit.ToString();
        }

        public virtual CriticalHit Clone
        {
            get
            {
                var clone = new CriticalHit();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(CriticalHit other)
        {
            if (other == null)
                return false;
            if (min != other.min)
                return false;
            if (max != other.max)
                return false;
            if (multiplier != other.multiplier)
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
            return Equals(other as CriticalHit);
        }

        public static bool operator ==(CriticalHit first, CriticalHit second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(CriticalHit first, CriticalHit second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (min is null ? 0 : min.GetHashCode());
            hash = (hash * 7) + (max is null ? 0 : max.GetHashCode());
            hash = (hash * 7) + (multiplier is null ? 0 :  multiplier.GetHashCode());
            return hash;
        }

        public CriticalHit Fill(CriticalHit source)
        {
            if (source == null)
                return this;
            min = source.min?.Clone;
            max = source.max?.Clone;
            multiplier = source.multiplier?.Clone;
            return this;
        }
    }
}
