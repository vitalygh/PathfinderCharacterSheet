using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SavingThrow: IPrototype<SavingThrow>, IEquatable<SavingThrow>
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
            if (abilityModifierSource != other.abilityModifierSource)
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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, abilityModifierSource) ? abilityModifierSource.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, baseSave) ? baseSave.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, magicModifier) ? magicModifier.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, miscModifier) ? miscModifier.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, tempModifier) ? tempModifier.GetHashCode() : 0);
            return hash;
        }

        public virtual SavingThrow Fill(SavingThrow source)
        {
            if (source == null)
                return this;
            abilityModifierSource = source.abilityModifierSource;
            baseSave = source.baseSave?.Clone;
            magicModifier = source.magicModifier?.Clone;
            miscModifier = source.miscModifier?.Clone;
            tempModifier = source.tempModifier?.Clone;
            return this;
        }
    }
}
