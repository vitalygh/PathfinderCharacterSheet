using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SavingThrow
    {
        public string abilityModifierSource = Ability.None.ToString();
        public Ability AbilityModifierSource
        {
            get { return CharacterSheet.GetEnumValue(abilityModifierSource, Ability.None); }
            set { abilityModifierSource = value.ToString(); }
        }
        public ValueWithIntModifiers baseSave = new ValueWithIntModifiers();
        public ValueWithIntModifiers magicModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers miscModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers tempModifier = new ValueWithIntModifiers();

        public SavingThrow()
        {

        }

        public SavingThrow(Ability abilityModifierSource)
        {
            this.abilityModifierSource = abilityModifierSource.ToString();
        }

        public int GetAbilityModifier(CharacterSheet sheet)
        {
            return CharacterSheet.GetAbilityModifier(sheet, AbilityModifierSource);
        }

        public int GetTotal(CharacterSheet sheet)
        {
            return baseSave.GetTotal(sheet) + GetAbilityModifier(sheet) + magicModifier.GetTotal(sheet) + miscModifier.GetTotal(sheet) + tempModifier.GetTotal(sheet);
        }

        public virtual object Clone
        {
            get
            {
                return new SavingThrow()
                {
                    abilityModifierSource = abilityModifierSource,
                    baseSave = baseSave.Clone as ValueWithIntModifiers,
                    magicModifier = magicModifier.Clone as ValueWithIntModifiers,
                    miscModifier = miscModifier.Clone as ValueWithIntModifiers,
                    tempModifier = tempModifier.Clone as ValueWithIntModifiers,
                };
            }
        }

        public bool Equals(SavingThrow other)
        {
            if (other == null)
                return false;
            if (abilityModifierSource != other.abilityModifierSource)
                return false;
            if (!baseSave.Equals(other.baseSave))
                return false;
            if (!magicModifier.Equals(other.magicModifier))
                return false;
            if (!miscModifier.Equals(other.miscModifier))
                return false;
            if (!tempModifier.Equals(other.tempModifier))
                return false;
            return true;
        }
    }
}
