using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Money
    {
        public ValueWithIntModifiers cuprumPoints = new ValueWithIntModifiers();
        public ValueWithIntModifiers silverPoints = new ValueWithIntModifiers();
        public ValueWithIntModifiers goldenPoints = new ValueWithIntModifiers();
        public ValueWithIntModifiers platinumPoints = new ValueWithIntModifiers();

        public object Clone
        {
            get
            {
                var clone = new Money();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(Money other)
        {
            if (!cuprumPoints.Equals(other.cuprumPoints))
                return false;
            if (!silverPoints.Equals(other.silverPoints))
                return false;
            if (!goldenPoints.Equals(other.goldenPoints))
                return false;
            if (!platinumPoints.Equals(other.platinumPoints))
                return false;
            return true;
        }

        public Money Fill(Money source)
        {
            if (source == null)
                return this;
            cuprumPoints = source.cuprumPoints.Clone as ValueWithIntModifiers;
            silverPoints = source.silverPoints.Clone as ValueWithIntModifiers;
            goldenPoints = source.goldenPoints.Clone as ValueWithIntModifiers;
            platinumPoints = source.platinumPoints.Clone as ValueWithIntModifiers;
            return this;
        }
    }
}
