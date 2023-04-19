using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Initiative
    {
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
        public int GetInitiative(CharacterSheet sheet)
        {
            return CharacterSheet.GetAbilityModifier(sheet, Ability.Dexterity) + miscModifiers.GetTotal(sheet);
        }
    }
}
