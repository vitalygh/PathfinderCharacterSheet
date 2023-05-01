using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class HP: IPrototype<HP>, IEquatable<HP>
    {
        public ValueWithIntModifiers maxHP = new ValueWithIntModifiers();
        public ValueWithIntModifiers hp = new ValueWithIntModifiers();
        public ValueWithIntModifiers damageResist = new ValueWithIntModifiers();

        public virtual HP Clone
        {
            get
            {
                var clone = new HP();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(HP other)
        {
            if (other == null)
                return false;
            if (maxHP != other.maxHP)
                return false;
            if (hp != other.hp)
                return false;
            if (damageResist != other.damageResist)
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
            return Equals(other as HP);
        }

        public static bool operator ==(HP first, HP second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(HP first, HP second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (maxHP is null ? 0 : maxHP.GetHashCode());
            hash = (hash * 7) + (hp is null ? 0 : hp.GetHashCode());
            hash = (hash * 7) + (damageResist is null ? 0 : damageResist.GetHashCode());
            return hash;
        }

        public HP Fill(HP source)
        {
            if (source == null)
                return this;
            maxHP = source.maxHP?.Clone;
            hp = source.hp?.Clone;
            damageResist = source.damageResist?.Clone;
            return this;
        }
    }
}
