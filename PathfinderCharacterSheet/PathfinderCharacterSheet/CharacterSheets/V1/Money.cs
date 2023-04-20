using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Money: IPrototype<Money>, IEquatable<Money>
    {
        public ValueWithIntModifiers cuprumPoints = new ValueWithIntModifiers();
        public ValueWithIntModifiers silverPoints = new ValueWithIntModifiers();
        public ValueWithIntModifiers goldenPoints = new ValueWithIntModifiers();
        public ValueWithIntModifiers platinumPoints = new ValueWithIntModifiers();

        public virtual Money Clone
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
            if (cuprumPoints != other.cuprumPoints)
                return false;
            if (silverPoints != other.silverPoints)
                return false;
            if (goldenPoints != other.goldenPoints)
                return false;
            if (platinumPoints != other.platinumPoints)
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
            return Equals(other as Money);
        }

        public static bool operator ==(Money first, Money second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(Money first, Money second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, cuprumPoints) ? cuprumPoints.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, silverPoints) ? silverPoints.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, goldenPoints) ? goldenPoints.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, platinumPoints) ? platinumPoints.GetHashCode() : 0);
            return hash;
        }

        public Money Fill(Money source)
        {
            if (source == null)
                return this;
            cuprumPoints = source.cuprumPoints?.Clone;
            silverPoints = source.silverPoints?.Clone;
            goldenPoints = source.goldenPoints?.Clone;
            platinumPoints = source.platinumPoints?.Clone;
            return this;
        }
    }
}
