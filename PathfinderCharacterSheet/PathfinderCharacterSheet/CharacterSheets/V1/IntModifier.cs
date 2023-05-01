#define SAVE_DELTA
using System;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntModifier : Modifier<int>, IEquatable<IntModifier>
    {
        public const Ability DefaultSourceAbility = Ability.None;
        public string sourceAbility
        {
            get =>
#if SAVE_DELTA
                DefaultSourceAbility == SourceAbility ? null :
#endif
                SourceAbility.ToString();
            set => SourceAbility = Helpers.GetEnumValue(value, DefaultSourceAbility);
        }
        internal Ability SourceAbility { get; set; } = DefaultSourceAbility;

        public IntMultiplier abilityMultiplier
        {
            get =>
#if SAVE_DELTA
                IntMultiplier.HasDefaultValue(AbilityMultiplier)? null :
#endif
                AbilityMultiplier;
            set => AbilityMultiplier = value;
        }
        internal IntMultiplier AbilityMultiplier { get; set; } = null;

        public const int DefaultSourceItemUID = CharacterSheet.InvalidUID;
        public string sourceItemUID
        {
            get =>
#if SAVE_DELTA
                DefaultSourceItemUID == SourceItemUID ? null :
#endif
                SourceItemUID.ToString();
            set => SourceItemUID = int.TryParse(value, out int outValue) ? outValue : DefaultSourceItemUID;
        }
        internal int SourceItemUID { get; set; } = DefaultSourceItemUID;

        public const bool DefaultMustBeActive = true;
        public string mustBeActive
        {
            get =>
#if SAVE_DELTA
                DefaultMustBeActive == MustBeActive ? null :
#endif
                MustBeActive.ToString();
            set => MustBeActive = bool.TryParse(value, out bool outValue) ? outValue : DefaultMustBeActive;
        }
        internal bool MustBeActive { get; set; } = DefaultMustBeActive;

        public const bool DefaultMultiplyToLevel = false;
        public string multiplyToLevel
        {
            get =>
#if SAVE_DELTA
                DefaultMultiplyToLevel == MultiplyToLevel ? null :
#endif
                MultiplyToLevel.ToString();
            set => MultiplyToLevel = bool.TryParse(value, out bool outValue) ? outValue : DefaultMultiplyToLevel;
        }
        internal bool MultiplyToLevel { get; set; } = DefaultMultiplyToLevel;

        public string className = null;
        public IntMultiplier levelMultiplier
        {
            get =>
#if SAVE_DELTA
                IntMultiplier.HasDefaultValue(LevelMultiplier) ? null :
#endif
                LevelMultiplier;
            set => LevelMultiplier = value;
        }
        internal IntMultiplier LevelMultiplier { get; set; } = null;

        public const bool DefaultAutoNaming = true;
        public string autoNaming
        {
            get =>
#if SAVE_DELTA
                DefaultAutoNaming == AutoNaming ? null :
#endif
                AutoNaming.ToString();
            set => AutoNaming = bool.TryParse(value, out bool boolValue) ? boolValue : DefaultAutoNaming;
        }
        internal bool AutoNaming { get; set; } = DefaultAutoNaming;

        public override int GetValue(CharacterSheet context)
        {
            var totalValue = base.GetValue(context);
            if (context == null)
                return totalValue;

            if (SourceItemUID != CharacterSheet.InvalidUID)
            {
                var item = context.GetItemByUID(SourceItemUID);
                if (item == null)
                    return 0;
                if (!item.active && (MustBeActive))
                    return 0;
            }

            if (SourceAbility != Ability.None)
            {
                var ab = context.GetAbilityModifier(SourceAbility);
                if (AbilityMultiplier != null)
                    ab = AbilityMultiplier.Apply(ab);
                totalValue += ab;
            }

            if (MultiplyToLevel)
            {
                if (!string.IsNullOrWhiteSpace(className))
                {
                    var level = context.GetLevelOfClass(className);
                    if (level != null)
                    {
                        var lv = level.GetValue(context);
                        if (LevelMultiplier != null)
                            lv = LevelMultiplier.Apply(lv);
                        totalValue *= lv;
                    }
                }
                else
                {
                    var lv = context.TotalLevel;
                    if (LevelMultiplier != null)
                        lv = LevelMultiplier.Apply(lv);
                    totalValue *= lv;
                }
            }
            return totalValue;
        }

        public string AsString(CharacterSheet sheet)
        {
            if (!AutoNaming)
                return name;
            var text = new StringBuilder();
            if (SourceAbility != Ability.None)
            {
                var ab = sourceAbility;
                if (AbilityMultiplier != null)
                    ab = AbilityMultiplier.AsString(ab);
                text.Append(ab);
                if (value != 0)
                    text.Insert(0, " + ").Insert(0, value).Insert(0, '(').Append(')');
            }
            if (MultiplyToLevel)
            {
                if (string.IsNullOrWhiteSpace(text.ToString()) && (value != 1))
                    text.Append(value);
                var lv = string.IsNullOrWhiteSpace(className) ? "Total Level" : "Level Of "
                    + (string.IsNullOrWhiteSpace(className) ? "Unnamed Class" : className);
                if (LevelMultiplier != null)
                    lv = LevelMultiplier.AsString(lv);
                if (!string.IsNullOrWhiteSpace(text.ToString()))
                    text.Append(" * ");
                text.Append(lv);
            }
            if (!string.IsNullOrWhiteSpace(name))
                if (string.IsNullOrWhiteSpace(text.ToString()))
                    text.Append(name);
                else
                    text.Insert(0, " (").Insert(0, name).Append(')');
            if (SourceItemUID != CharacterSheet.InvalidUID)
            {
                var item = sheet.GetItemByUID(SourceItemUID);
                if (item == null)
                    text.Insert(0, "[item missing] ");
                else if (!MustBeActive)
                    text.Insert(0, "] ").Insert(0, item.name).Insert(0, "[with ");
                else if (item.active)
                    text.Insert(0, "] ").Insert(0, item.name).Insert(0, "[with active ");
                else
                    text.Insert(0, "] ").Insert(0, item.name).Insert(0, "[need active ");
            }
            return text.ToString();
        }

        public override Modifier<int> Clone
        {
            get
            {
                var clone = new IntModifier();
                clone.Fill(this);
                return clone;
            }
        }

        public virtual IntModifier Fill(IntModifier source)
        {
            if (source == null)
                return this;
            base.Fill(source);

            SourceAbility = source.SourceAbility;
            AbilityMultiplier = source.AbilityMultiplier?.Clone;
            SourceItemUID = source.SourceItemUID;
            MustBeActive = source.MustBeActive;

            MultiplyToLevel = source.MultiplyToLevel;
            className = source.className;
            LevelMultiplier = source.LevelMultiplier?.Clone;

            AutoNaming = source.AutoNaming;

            return this;
        }

        public bool Equals(IntModifier other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (SourceAbility != other.SourceAbility)
                return false;
            if (AbilityMultiplier != other.AbilityMultiplier)
                return false;
            if (SourceItemUID != other.SourceItemUID)
                return false;
            if (MustBeActive != other.MustBeActive)
                return false;
            if (MultiplyToLevel != other.MultiplyToLevel)
                return false;
            if (LevelMultiplier != other.LevelMultiplier)
                return false;
            if (className != other.className)
                return false;
            if (AutoNaming != other.AutoNaming)
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
            return Equals(other as IntModifier);
        }

        public static bool operator ==(IntModifier first, IntModifier second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(IntModifier first, IntModifier second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + SourceAbility.GetHashCode();
            hash = (hash * 7) + (AbilityMultiplier is null ? 0 : AbilityMultiplier.GetHashCode());
            hash = (hash * 7) + SourceItemUID.GetHashCode();
            hash = (hash * 7) + MustBeActive.GetHashCode();
            hash = (hash * 7) + MultiplyToLevel.GetHashCode();
            hash = (hash * 7) + (className is null ? 0 : className.GetHashCode());
            hash = (hash * 7) + (LevelMultiplier is null ? 0 : LevelMultiplier.GetHashCode());
            hash = (hash * 7) + AutoNaming.GetHashCode();
            return hash;
        }
    }
}
