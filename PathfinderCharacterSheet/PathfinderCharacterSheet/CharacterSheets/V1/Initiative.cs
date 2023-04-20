using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Initiative: IPrototype<Initiative>, IEquatable<Initiative>
    {
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
        public int GetInitiative(CharacterSheet sheet)
        {
            return CharacterSheet.GetAbilityModifier(sheet, Ability.Dexterity) + miscModifiers.GetValue(sheet);
        }

        public virtual Initiative Clone
        {
            get
            {
                var clone = new Initiative();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(Initiative other)
        {
            if (miscModifiers != other.miscModifiers)
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
            return Equals(other as Initiative);
        }

        public static bool operator ==(Initiative first, Initiative second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(Initiative first, Initiative second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, miscModifiers) ? miscModifiers.GetHashCode() : 0);
            return hash;
        }

        public Initiative Fill(Initiative source)
        {
            if (source == null)
                return this;
            miscModifiers = source.miscModifiers?.Clone;
            return this;
        }
    }
}
