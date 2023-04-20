using System;
using System.Collections.Generic;
using System.Text;

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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, maxHP) ? maxHP.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, hp) ? hp.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, damageResist) ? damageResist.GetHashCode() : 0);
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
