using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class CriticalHit
    {
        public ValueWithIntModifiers min = new ValueWithIntModifiers();
        public ValueWithIntModifiers max = new ValueWithIntModifiers();
        public ValueWithIntModifiers multiplier = new ValueWithIntModifiers();
        public string AsString(CharacterSheet sheet)
        {
            var mint = min.GetTotal(sheet);
            var maxt = max.GetTotal(sheet);
            var mul = multiplier.GetTotal(sheet);
            if (mul <= 0)
                return string.Empty;
            var crit = string.Empty;
            if (mint < maxt)
                crit += mint + "-";
            crit += maxt;
            crit += "/x" + mul;
            return crit;
        }

        public object Clone
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
            if (!min.Equals(other.min))
                return false;
            if (!max.Equals(other.max))
                return false;
            if (!multiplier.Equals(other.multiplier))
                return false;
            return true;
        }

        public CriticalHit Fill(CriticalHit source)
        {
            if (source == null)
                return this;
            min = source.min.Clone as ValueWithIntModifiers;
            max = source.max.Clone as ValueWithIntModifiers;
            multiplier = source.multiplier.Clone as ValueWithIntModifiers;
            return this;
        }
    }
}
