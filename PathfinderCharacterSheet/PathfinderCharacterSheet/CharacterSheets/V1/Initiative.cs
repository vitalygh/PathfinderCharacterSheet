using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Initiative: IPrototype<Initiative>, IEquatable<Initiative>
    {
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
        public int GetInitiative(CharacterSheet sheet)
        {
            if (sheet == null)
                return 0;
            return sheet.GetAbilityModifier(Ability.Dexterity) + miscModifiers.GetValue(sheet);
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
            if (other == null)
                return false;
            if (miscModifiers != other.miscModifiers)
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
            return Equals(other as Initiative);
        }

        public static bool operator ==(Initiative first, Initiative second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
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
            hash = (hash * 7) + (miscModifiers is null ? 0 : miscModifiers.GetHashCode());
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
