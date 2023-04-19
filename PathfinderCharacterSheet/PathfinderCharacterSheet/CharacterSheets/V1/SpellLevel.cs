using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SpellLevel
    {
        public ValueWithIntModifiers spellsKnown = new ValueWithIntModifiers();
        public ValueWithIntModifiers spellSaveDC = new ValueWithIntModifiers();
        public ValueWithIntModifiers spellsPerDay = new ValueWithIntModifiers();
        public ValueWithIntModifiers bonusSpells = new ValueWithIntModifiers();
    }
}
