using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class DiceRoll: IPrototype<DiceRoll>, IEquatable<DiceRoll>
    {
        public ValueWithIntModifiers diceCount = new ValueWithIntModifiers();
        public ValueWithIntModifiers diceSides = new ValueWithIntModifiers();
        public ValueWithIntModifiers additional = new ValueWithIntModifiers();
        public string description = null;
        public string AsString(CharacterSheet sheet)
        {
            var roll = string.Empty;
            var sides = diceSides.GetValue(sheet);
            var count = diceCount.GetValue(sheet);
            var add = additional.GetValue(sheet);
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

        public virtual DiceRoll Clone
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
            if (diceCount != other.diceCount)
                return false;
            if (diceSides != other.diceSides)
                return false;
            if (additional != other.additional)
                return false;
            if (description != other.description)
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
            return Equals(other as DiceRoll);
        }

        public static bool operator ==(DiceRoll first, DiceRoll second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(DiceRoll first, DiceRoll second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, diceCount) ? diceCount.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, diceSides) ? diceSides.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, additional) ? additional.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, description) ? description.GetHashCode() : 0);
            return hash;
        }

        public DiceRoll Fill(DiceRoll source)
        {
            if (source == null)
                return this;
            diceCount = source.diceCount?.Clone;
            diceSides = source.diceSides?.Clone;
            additional = source.additional?.Clone;
            description = source.description;
            return this;
        }
    }
}
