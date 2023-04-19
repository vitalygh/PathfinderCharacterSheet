using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Feat : ItemWithDescription
    {
        public override object Clone
        {
            get
            {
                var clone = new Feat();
                clone.Fill(this);
                return clone;
            }
        }
    }
}
