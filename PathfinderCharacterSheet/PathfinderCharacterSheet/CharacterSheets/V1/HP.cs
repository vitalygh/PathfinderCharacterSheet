using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class HP
    {
        public ValueWithIntModifiers maxHP = new ValueWithIntModifiers();
        public ValueWithIntModifiers hp = new ValueWithIntModifiers();
        public ValueWithIntModifiers damageResist = new ValueWithIntModifiers();
    }
}
