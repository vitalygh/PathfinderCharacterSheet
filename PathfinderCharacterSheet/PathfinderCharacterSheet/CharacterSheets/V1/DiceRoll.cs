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
            var roll = new StringBuilder();
            var sides = diceSides.GetValue(sheet);
            var count = diceCount.GetValue(sheet);
            var add = additional.GetValue(sheet);
            if (((sides <= 0) || (count <= 0)) && (add == 0))
                return string.Empty;
            if ((sides > 0) && (count > 0))
            {
                roll.Append(count).Append("d").Append(sides);
                if (add < 0)
                    roll.Append(" - ").Append(Math.Abs(add));
                else if (add > 0)
                    roll.Append(" + ").Append(add);
            }
            else
                roll.Append(add);
            if (!string.IsNullOrWhiteSpace(description))
                roll.Append(" ").Append(description);
            roll.Insert(0, "(").Append(")");
            return roll.ToString();
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
            if (other == null)
                return false;
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
            if (other is null)
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
            if (first is null)
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
            hash = (hash * 7) + (diceCount is null ? 0 : diceCount.GetHashCode());
            hash = (hash * 7) + (diceSides is null ? 0 :diceSides.GetHashCode());
            hash = (hash * 7) + (additional is null ? 0 : additional.GetHashCode());
            hash = (hash * 7) + (description is null ? 0 : description.GetHashCode());
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
