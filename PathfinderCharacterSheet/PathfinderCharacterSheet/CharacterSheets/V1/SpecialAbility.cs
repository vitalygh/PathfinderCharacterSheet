using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SpecialAbility: ItemWithUseLimit, IEquatable<SpecialAbility>
    {
        public override ItemWithDescription Clone
        {
            get
            {
                var clone = new SpecialAbility();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(SpecialAbility other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            return true;
        }
    }
}
