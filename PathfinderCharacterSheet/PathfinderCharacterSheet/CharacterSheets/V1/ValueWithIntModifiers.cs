using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ValueWithIntModifiers : ValueWithModifiers<IntModifier, int, IntSum>
    {
        public override object Clone
        {
            get
            {
                var clone = new ValueWithIntModifiers();
                clone.Fill(this);
                return clone;
            }
        }

        public ValueWithIntModifiers Fill(ValueWithIntModifiers source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            return this;
        }

        public bool Equals(ValueWithIntModifiers other)
        {
            if (other == null)
                return false;
            if (!other.baseValue.Equals(baseValue))
                return false;
            if (!CharacterSheet.IsEqual(other.modifiers, modifiers))
                return false;
            return true;
        }
    }
}
