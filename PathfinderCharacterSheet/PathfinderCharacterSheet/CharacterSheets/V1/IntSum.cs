using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public struct IntSum : ISummable<int>
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
