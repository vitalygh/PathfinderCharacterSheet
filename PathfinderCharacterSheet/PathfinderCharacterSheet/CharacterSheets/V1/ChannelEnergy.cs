using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ChannelEnergy: IPrototype<ChannelEnergy>, IEquatable<ChannelEnergy>
    {
        public ValueWithIntModifiers left = new ValueWithIntModifiers();
        public ValueWithIntModifiers total = new ValueWithIntModifiers();
        public DiceRoll points = new DiceRoll();

        public virtual ChannelEnergy Clone
        {
            get
            {
                var clone = new ChannelEnergy();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(ChannelEnergy other)
        {
            if (other == null)
                return false;
            if (left != other.left)
                return false;
            if (total != other.total)
                return false;
            if (points != other.points)
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
            return Equals(other as ChannelEnergy);
        }

        public static bool operator ==(ChannelEnergy first, ChannelEnergy second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(ChannelEnergy first, ChannelEnergy second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (left is null ? 0 : left.GetHashCode());
            hash = (hash * 7) + (total is null ?  0 : total.GetHashCode());
            hash = (hash * 7) + (points is null ? 0 : points.GetHashCode());
            return hash;
        }

        public ChannelEnergy Fill(ChannelEnergy source)
        {
            if (source == null)
                return this;
            left = source.left?.Clone;
            total = source.total?.Clone;
            points = source.points?.Clone;
            return this;
        }
    }
}
