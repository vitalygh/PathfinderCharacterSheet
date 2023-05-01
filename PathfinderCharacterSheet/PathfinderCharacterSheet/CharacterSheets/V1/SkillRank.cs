#define SAVE_DELTA
using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SkillRank: IContextValue<int, CharacterSheet>, IPrototype<SkillRank>, IEquatable<SkillRank>
    {
        public string name = Skill.None.ToString();
        public bool hasSubject = false;
        public string subject = null;
        public bool classSkill = false;
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

        public ValueWithIntModifiers rank = new ValueWithIntModifiers();
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
        public bool armorPenalty = false;
        public bool trainedOnly = false;
        public bool custom = false;

        public SkillRank()
        {
        }

        public SkillRank(Skill name, Ability abilityModifierSource, bool trainedOnly = false, bool hasSubject = false, bool custom = false) :
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

        public virtual SkillRank Clone
        {
            get
            {
                var clone = new SkillRank();
                clone.Fill(this);
                return clone;
            }
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
            if (AbilityModifierSource != other.AbilityModifierSource)
                return false;
            if (rank != other.rank)
                return false;
            if (miscModifiers != other.miscModifiers)
                return false;
            if (armorPenalty != other.armorPenalty)
                return false;
            if (trainedOnly != other.trainedOnly)
                return false;
            if (custom != other.custom)
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
            return Equals(other as SkillRank);
        }

        public static bool operator ==(SkillRank first, SkillRank second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(SkillRank first, SkillRank second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (name is null ? 0 : name.GetHashCode());
            hash = (hash * 7) + hasSubject.GetHashCode();
            hash = (hash * 7) + (subject is null ? 0 : subject.GetHashCode());
            hash = (hash * 7) + classSkill.GetHashCode();
            hash = (hash * 7) + AbilityModifierSource.GetHashCode();
            hash = (hash * 7) + (rank is null ? 0 : rank.GetHashCode());
            hash = (hash * 7) + (miscModifiers is null ? 0 : miscModifiers.GetHashCode());
            hash = (hash * 7) + armorPenalty.GetHashCode();
            hash = (hash * 7) + trainedOnly.GetHashCode();
            hash = (hash * 7) + custom.GetHashCode();
            return hash;
        }

        public SkillRank Fill(SkillRank source)
        {
            if (source == null)
                return this;
            name = source.name;
            hasSubject = source.hasSubject;
            subject = source.subject;
            classSkill = source.classSkill;
            AbilityModifierSource = source.AbilityModifierSource;
            rank = source.rank?.Clone;
            miscModifiers = source.miscModifiers?.Clone;
            armorPenalty = source.armorPenalty;
            trainedOnly = source.trainedOnly;
            custom = source.custom;
            return this;
        }

        public int GetAbilityModifier(CharacterSheet sheet)
        {
            if (sheet == null)
                return 0;
            return sheet.GetAbilityModifier(AbilityModifierSource);
        }

        public int GetValue(CharacterSheet sheet)
        {
            var total = rank.GetValue(sheet);
            if (classSkill && (total > 0))
                total += 3;
            if (armorPenalty)
                total += sheet.CheckPenalty();
            total += GetAbilityModifier(sheet);
            total += miscModifiers.GetValue(sheet);
            return total;
        }
    }
}
