#define SAVE_DELTA
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SavingThrow: IPrototype<SavingThrow>, IEquatable<SavingThrow>
    {
        public const Ability DefaultAbilityModifierSource = Ability.None;
        public string abilityModifierSource
        {
            get =>
#if SAVE_DELTA
                DefaultAbilityModifierSource == AbilityModifierSource ? null :
#endif
                AbilityModifierSource.ToString();
            set => AbilityModifierSource = Helpers.GetEnumValue(value, DefaultAbilityModifierSource);
        }
        internal Ability AbilityModifierSource { get; set; } = DefaultAbilityModifierSource;

        public ValueWithIntModifiers baseSave = new ValueWithIntModifiers();
        public ValueWithIntModifiers magicModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers miscModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers tempModifier = new ValueWithIntModifiers();

        public SavingThrow()
        {
        
        }

        public SavingThrow(Ability abilityModifierSource)
        {
            AbilityModifierSource = abilityModifierSource;
        }

        public int GetAbilityModifier(CharacterSheet sheet)
        {
            if (sheet == null)
                return 0;
            return sheet.GetAbilityModifier(AbilityModifierSource);
        }

        public int GetTotal(CharacterSheet sheet)
        {
            return baseSave.GetValue(sheet) + GetAbilityModifier(sheet) + magicModifier.GetValue(sheet) + miscModifier.GetValue(sheet) + tempModifier.GetValue(sheet);
        }

        public virtual SavingThrow Clone
        {
            get
            {
                var savingThrow = new SavingThrow();
                savingThrow.Fill(this);
                return savingThrow;
            }
        }

        public bool Equals(SavingThrow other)
        {
            if (other == null)
                return false;
            if (AbilityModifierSource != other.AbilityModifierSource)
                return false;
            if (baseSave != other.baseSave)
                return false;
            if (magicModifier != other.magicModifier)
                return false;
            if (miscModifier != other.miscModifier)
                return false;
            if (tempModifier != other.tempModifier)
                return false;
            return true;
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as SavingThrow);
        }

        public static bool operator ==(SavingThrow first, SavingThrow second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(SavingThrow first, SavingThrow second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + AbilityModifierSource.GetHashCode();
            hash = (hash * 7) + (baseSave is null ? 0 : baseSave.GetHashCode());
            hash = (hash * 7) + (magicModifier is null ? 0 : magicModifier.GetHashCode());
            hash = (hash * 7) + (miscModifier is null ? 0 : miscModifier.GetHashCode());
            hash = (hash * 7) + (tempModifier is null ? 0 : tempModifier.GetHashCode());
            return hash;
        }

        public virtual SavingThrow Fill(SavingThrow source)
        {
            if (source == null)
                return this;
            AbilityModifierSource = source.AbilityModifierSource;
            baseSave = source.baseSave?.Clone;
            magicModifier = source.magicModifier?.Clone;
            miscModifier = source.miscModifier?.Clone;
            tempModifier = source.tempModifier?.Clone;
            return this;
        }
    }
}
