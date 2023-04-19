using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Note : ItemWithDescription
    {
        public override object Clone
        {
            get
            {
                var clone = new Note();
                clone.Fill(this);
                return clone;
            }
        }
    }
}
