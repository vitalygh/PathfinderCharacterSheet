using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SpecialAbility : ItemWithUseLimit
    {
        public override object Clone
        {
            get
            {
                var clone = new SpecialAbility();
                clone.Fill(this);
                return clone;
            }
        }
    }
}
