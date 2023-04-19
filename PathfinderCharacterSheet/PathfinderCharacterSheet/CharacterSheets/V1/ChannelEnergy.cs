using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ChannelEnergy
    {
        public ValueWithIntModifiers left = new ValueWithIntModifiers();
        public ValueWithIntModifiers total = new ValueWithIntModifiers();
        public DiceRoll points = new DiceRoll();

        public object Clone
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
            if (!left.Equals(other.left))
                return false;
            if (!total.Equals(other.total))
                return false;
            if (!points.Equals(other.points))
                return false;
            return true;
        }

        public ChannelEnergy Fill(ChannelEnergy source)
        {
            if (source == null)
                return this;
            left = source.left.Clone as ValueWithIntModifiers;
            total = source.total.Clone as ValueWithIntModifiers;
            points = source.points.Clone as DiceRoll;
            return this;
        }
    }
}
