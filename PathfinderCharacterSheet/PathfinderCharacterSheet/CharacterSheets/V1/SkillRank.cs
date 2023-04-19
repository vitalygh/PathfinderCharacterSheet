using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SkillRank
    {
        public string name = Skills.None.ToString();
        public bool hasSubject = false;
        public string subject = null;
        public bool classSkill = false;
        public string abilityModifierSource = Ability.None.ToString();
        public Ability AbilityModifierSource
        {
            get { return CharacterSheet.GetEnumValue(abilityModifierSource, Ability.None); }
            set { abilityModifierSource = value.ToString(); }
        }
        public ValueWithIntModifiers rank = new ValueWithIntModifiers();
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
        public bool armorPenalty = false;
        public bool trainedOnly = false;
        public bool custom = false;

        public SkillRank()
        {
        }

        public SkillRank(Skills name, Ability abilityModifierSource, bool trainedOnly = false, bool hasSubject = false, bool custom = false) :
            this(name.ToString(), abilityModifierSource, trainedOnly, hasSubject, custom)
        {
        }

        public SkillRank(string name, Ability abilityModifierSource, bool trainedOnly = false, bool hasSubject = false, bool custom = false)
        {
            this.name = name;
            AbilityModifierSource = abilityModifierSource;
            switch (abilityModifierSource)
            {
                case Ability.Strength:
                case Ability.Dexterity:
                    armorPenalty = true;
                    break;
                default:
                    armorPenalty = false;
                    break;
            }
            this.trainedOnly = trainedOnly;
            this.hasSubject = hasSubject;
            this.custom = custom;
        }

        public string Name
        {
            get
            {
                var text = string.Empty;
                text += name;
                if (trainedOnly)
                    text += "*";
                if (hasSubject && !string.IsNullOrWhiteSpace(subject))
                    text += " (" + subject + ")";
                return text;
            }
        }

        public virtual object Clone
        {
            get
            {
                var clone = new SkillRank();
                clone.Fill(this);
                return clone;
            }
        }

        public SkillRank Fill(SkillRank source)
        {
            if (source == null)
                return this;
            name = source.name;
            hasSubject = source.hasSubject;
            subject = source.subject;
            classSkill = source.classSkill;
            abilityModifierSource = source.abilityModifierSource;
            rank = source.rank.Clone as ValueWithIntModifiers;
            miscModifiers = source.miscModifiers.Clone as ValueWithIntModifiers;
            armorPenalty = source.armorPenalty;
            trainedOnly = source.trainedOnly;
            custom = source.custom;
            return this;
        }

        public bool Equals(SkillRank other)
        {
            if (other == null)
                return false;
            if (name != other.name)
                return false;
            if (hasSubject != other.hasSubject)
                return false;
            if (subject != other.subject)
                return false;
            if (classSkill != other.classSkill)
                return false;
            if (abilityModifierSource != other.abilityModifierSource)
                return false;
            if (!rank.Equals(other.rank))
                return false;
            if (!miscModifiers.Equals(other.miscModifiers))
                return false;
            if (armorPenalty != other.armorPenalty)
                return false;
            if (trainedOnly != other.trainedOnly)
                return false;
            if (custom != other.custom)
                return false;
            return true;
        }

        public int GetAbilityModifier(CharacterSheet sheet)
        {
            return CharacterSheet.GetAbilityModifier(sheet, AbilityModifierSource);
        }

        public int GetTotal(CharacterSheet sheet)
        {
            var total = rank.GetTotal(sheet);
            if (classSkill && (total > 0))
                total += 3;
            if (armorPenalty)
                total += sheet.CheckPenalty();
            total += GetAbilityModifier(sheet);
            total += miscModifiers.GetTotal(sheet);
            return total;
        }
    }
}
