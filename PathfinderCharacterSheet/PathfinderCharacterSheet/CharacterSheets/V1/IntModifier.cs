using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class IntModifier : Modifier<int>
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

        public override int GetValue(CharacterSheet sheet)
        {
            if ((sourceItemUID != CharacterSheet.InvalidUID) && (sheet != null))
            {
                var item = sheet.GetItemByUID(sourceItemUID);
                if (item == null)
                    return 0;
                if (!item.active && mustBeActive)
                    return 0;
            }
            var totalValue = value;

            if ((sheet != null) && (SourceAbility != Ability.None))
            {
                var ab = sheet.GetAbilityModifier(SourceAbility);
                if (abilityMultiplier != null)
                    ab = abilityMultiplier.Apply(ab);
                totalValue += ab;
            }

            if (multiplyToLevel)
            {
                if (!string.IsNullOrWhiteSpace(className))
                {
                    var level = sheet.GetLevelOfClass(className);
                    if (level != null)
                    {
                        var lv = level.GetTotal(sheet);
                        if (levelMultiplier != null)
                            lv = levelMultiplier.Apply(lv);
                        totalValue *= lv;
                    }
                }
                else
                {
                    var lv = sheet.TotalLevel;
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

        public override object Clone
        {
            get
            {
                var clone = new IntModifier();
                clone.Fill(this);
                return clone;
            }
        }

        public static List<IntModifier> CreateClone(List<IntModifier> mods)
        {
            if (mods == null)
                return null;
            var list = new List<IntModifier>();
            foreach (var m in mods)
                if (m != null)
                    list.Add(m.Clone as IntModifier);
                else
                    list.Add(m);
            return list;
        }

        public virtual object Fill(IntModifier source)
        {
            if (source == null)
                return this;
            base.Fill(source);

            sourceAbility = source.sourceAbility;
            abilityMultiplier = source.abilityMultiplier.Clone as IntMultiplier;

            sourceItemUID = source.sourceItemUID;
            mustBeActive = source.mustBeActive;

            multiplyToLevel = source.multiplyToLevel;
            className = source.className;
            levelMultiplier = source.levelMultiplier.Clone as IntMultiplier;

            autoNaming = source.autoNaming;

            return this;
        }

        public bool Equals(IntModifier other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (other.sourceAbility != sourceAbility)
                return false;
            if (!other.abilityMultiplier.Equals(abilityMultiplier))
                return false;
            if (other.sourceItemUID != sourceItemUID)
                return false;
            if (other.mustBeActive != mustBeActive)
                return false;
            if (other.multiplyToLevel != multiplyToLevel)
                return false;
            if (!other.levelMultiplier.Equals(levelMultiplier))
                return false;
            if (other.className != className)
                return false;
            if (other.autoNaming != autoNaming)
                return false;
            return true;
        }
    }
}
