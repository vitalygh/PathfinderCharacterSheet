using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class DiceRollList: List<DiceRoll>, IEquatable<DiceRollList>, IPrototype<DiceRollList>
    {
        public DiceRollList() : base()
        {
        }
        
        public DiceRollList(DiceRollList other): base()
        {
            if (other != null)
                AddRange(other);
        }

        public virtual DiceRollList Clone
        {
            get
            {
                var diceRollsList = new DiceRollList();
                diceRollsList.Fill(this);
                return diceRollsList;
            }
        }

        public virtual DiceRollList Fill(DiceRollList source)
        {
            if (source == null)
                return this;
            Clear();
            foreach (var roll in source)
                Add(roll?.Clone);
            return this;
        }

        public bool Equals(DiceRollList other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return System.Linq.Enumerable.SequenceEqual(this, other);
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as DiceRollList);
        }

        public static bool operator ==(DiceRollList first, DiceRollList second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(DiceRollList first, DiceRollList second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
