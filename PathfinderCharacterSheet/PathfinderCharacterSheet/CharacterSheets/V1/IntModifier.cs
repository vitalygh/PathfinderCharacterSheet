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
            get { return CharacterSheet.GetEnumValue(sourceAbility, Ability.None); }
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

            if ((SourceAbility != Ability.None))
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
                return Name;
            var text = string.Empty;
            if (SourceAbility != Ability.None)
            {
                var ab = sourceAbility;
                if (abilityMultiplier != null)
                    ab = abilityMultiplier.AsString(ab);
                if (value != 0)
                    ab = "(" + value + " + " + ab + ")";
                text += ab;
            }
            if (multiplyToLevel)
            {
                if (string.IsNullOrWhiteSpace(text) && (value != 1))
                    text += value;
                var lv = string.IsNullOrWhiteSpace(className) ? "Total Level" : "Level Of "
                    + (string.IsNullOrWhiteSpace(className) ? "Unnamed Class" : className);
                if (levelMultiplier != null)
                    lv = levelMultiplier.AsString(lv);
                if (!string.IsNullOrWhiteSpace(text))
                    text += " * ";
                text += lv;
            }
            if (!string.IsNullOrWhiteSpace(Name))
                if (string.IsNullOrWhiteSpace(text))
                    text = Name;
                else
                    text = Name + " (" + text + ")";
            if (sourceItemUID != CharacterSheet.InvalidUID)
            {
                var item = sheet.GetItemByUID(sourceItemUID);
                if (item == null)
                    text = "[item missing] " + text;
                else if (!mustBeActive)
                    text = "[with " + item.name + "] " + text;
                else if (item.active)
                    text = "[with active " + item.name + "] " + text;
                else
                    text = "[need active " + item.name + "] " + text;
            }
            return text;
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

        public virtual object Fill(IntModifier source)
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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, abilityMultiplier) ? abilityMultiplier.GetHashCode() : 0);
            hash = (hash * 7) + sourceItemUID.GetHashCode();
            hash = (hash * 7) + mustBeActive.GetHashCode();
            hash = (hash * 7) + multiplyToLevel.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, className) ? className.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, levelMultiplier) ? levelMultiplier.GetHashCode() : 0);
            hash = (hash * 7) + autoNaming.GetHashCode();
            return hash;
        }
    }
}
