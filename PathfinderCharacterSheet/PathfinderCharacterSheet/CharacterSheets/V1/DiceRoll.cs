using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class DiceRoll
    {
        public ValueWithIntModifiers diceCount = new ValueWithIntModifiers();
        public ValueWithIntModifiers diceSides = new ValueWithIntModifiers();
        public ValueWithIntModifiers additional = new ValueWithIntModifiers();
        public string description = null;
        public string AsString(CharacterSheet sheet)
        {
            var roll = string.Empty;
            var sides = diceSides.GetTotal(sheet);
            var count = diceCount.GetTotal(sheet);
            var add = additional.GetTotal(sheet);
            if (((sides <= 0) || (count <= 0)) && (add == 0))
                return roll;
            if ((sides > 0) && (count > 0))
            {
                roll += count + "d" + sides;
                if (add < 0)
                    roll += " - " + Math.Abs(add);
                else if (add > 0)
                    roll += " + " + add;
            }
            else
                roll += add;
            if (!string.IsNullOrWhiteSpace(description))
                roll += " " + description;
            return "(" + roll + ")";
        }

        public object Clone
        {
            get
            {
                var clone = new DiceRoll();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(DiceRoll other)
        {
            if (!diceCount.Equals(other.diceCount))
                return false;
            if (!diceSides.Equals(other.diceSides))
                return false;
            if (!additional.Equals(other.additional))
                return false;
            if (description != other.description)
                return false;
            return true;
        }

        public DiceRoll Fill(DiceRoll source)
        {
            if (source == null)
                return this;
            diceCount = source.diceCount.Clone as ValueWithIntModifiers;
            diceSides = source.diceSides.Clone as ValueWithIntModifiers;
            additional = source.additional.Clone as ValueWithIntModifiers;
            description = source.description;
            return this;
        }
    }
}
