using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntModifier : Modifier<int>, IEquatable<IntModifier>
    {
        public string sourceAbility = Ability.None.ToString();
        public Ability SourceAbility
        {
            get { return Helpers.GetEnumValue(sourceAbility, Ability.None); }
            set { sourceAbility = value.ToString(); }
        }
        public IntMultiplier abilityMultiplier = new IntMultiplier();

        public int sourceItemUID = CharacterSheet.InvalidUID;
        public bool mustBeActive = true;

        public bool multiplyToLevel = false;
        public string className = null;
        public IntMultiplier levelMultiplier = new IntMultiplier();

        public bool autoNaming = true;

        public override int GetValue(CharacterSheet context)
        {
            var totalValue = base.GetValue(context);
            if (context == null)
                return totalValue;

            if (sourceItemUID != CharacterSheet.InvalidUID)
            {
                var item = context.GetItemByUID(sourceItemUID);
                if (item == null)
                    return 0;
                if (!item.active && mustBeActive)
                    return 0;
            }

            if (SourceAbility != Ability.None)
            {
                var ab = context.GetAbilityModifier(SourceAbility);
                if (abilityMultiplier != null)
                    ab = abilityMultiplier.Apply(ab);
                totalValue += ab;
            }

            if (multiplyToLevel)
            {
                if (!string.IsNullOrWhiteSpace(className))
                {
                    var level = context.GetLevelOfClass(className);
                    if (level != null)
                    {
                        var lv = level.GetValue(context);
                        if (levelMultiplier != null)
                            lv = levelMultiplier.Apply(lv);
                        totalValue *= lv;
                    }
                }
                else
                {
                    var lv = context.TotalLevel;
                    if (levelMultiplier != null)
                        lv = levelMultiplier.Apply(lv);
                    totalValue *= lv;
                }
            }
            return totalValue;
        }

        public string AsString(CharacterSheet sheet)
        {
            if (!autoNaming)
                return name;
            var text = new StringBuilder();
            if (SourceAbility != Ability.None)
            {
                var ab = sourceAbility;
                if (abilityMultiplier != null)
                    ab = abilityMultiplier.AsString(ab);
                text.Append(ab);
                if (value != 0)
                    text.Insert(0, " + ").Insert(0, value).Insert(0, "(").Append(")");
            }
            if (multiplyToLevel)
            {
                if (string.IsNullOrWhiteSpace(text.ToString()) && (value != 1))
                    text.Append(value);
                var lv = string.IsNullOrWhiteSpace(className) ? "Total Level" : "Level Of "
                    + (string.IsNullOrWhiteSpace(className) ? "Unnamed Class" : className);
                if (levelMultiplier != null)
                    lv = levelMultiplier.AsString(lv);
                if (!string.IsNullOrWhiteSpace(text.ToString()))
                    text.Append(" * ");
                text.Append(lv);
            }
            if (!string.IsNullOrWhiteSpace(name))
                if (string.IsNullOrWhiteSpace(text.ToString()))
                    text.Append(name);
                else
                    text.Insert(0, " (").Insert(0, name).Append(")");
            if (sourceItemUID != CharacterSheet.InvalidUID)
            {
                var item = sheet.GetItemByUID(sourceItemUID);
                if (item == null)
                    text.Insert(0, "[item missing] ");
                else if (!mustBeActive)
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

            sourceAbility = source.sourceAbility;
            abilityMultiplier = source.abilityMultiplier?.Clone;
            sourceItemUID = source.sourceItemUID;
            mustBeActive = source.mustBeActive;

            multiplyToLevel = source.multiplyToLevel;
            className = source.className;
            levelMultiplier = source.levelMultiplier?.Clone;

            autoNaming = source.autoNaming;

            return this;
        }

        public bool Equals(IntModifier other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (sourceAbility != other.sourceAbility)
                return false;
            if (abilityMultiplier != other.abilityMultiplier)
                return false;
            if (abilityMultiplier != other.abilityMultiplier)
                return false;
            if (sourceItemUID != other.sourceItemUID)
                return false;
            if (mustBeActive != other.mustBeActive)
                return false;
            if (multiplyToLevel != other.multiplyToLevel)
                return false;
            if (levelMultiplier != other.levelMultiplier)
                return false;
            if (className != other.className)
                return false;
            if (autoNaming != other.autoNaming)
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
            hash = (hash * 7) + sourceAbility.GetHashCode();
            hash = (hash * 7) + (abilityMultiplier is null ? 0 : abilityMultiplier.GetHashCode());
            hash = (hash * 7) + sourceItemUID.GetHashCode();
            hash = (hash * 7) + mustBeActive.GetHashCode();
            hash = (hash * 7) + multiplyToLevel.GetHashCode();
            hash = (hash * 7) + (className is null ? 0 : className.GetHashCode());
            hash = (hash * 7) + (levelMultiplier is null ? 0 : levelMultiplier.GetHashCode());
            hash = (hash * 7) + autoNaming.GetHashCode();
            return hash;
        }
    }
}
