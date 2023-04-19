using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ArmorClass
    {
        public enum DexterityModifierSources
        {
            DependsOnACItems,
            Full,
            Custom,
        }

        public ValueWithIntModifiers armorBonus = new ValueWithIntModifiers();
        public bool itemsArmorBonus = true;
        public ValueWithIntModifiers shieldBonus = new ValueWithIntModifiers();
        public bool itemsShieldBonus = true;
        public string dexterityModifierSource = DexterityModifierSources.DependsOnACItems.ToString();
        public DexterityModifierSources DexterityModifierSource
        {
            get { return CharacterSheet.GetEnumValue(dexterityModifierSource, DexterityModifierSources.DependsOnACItems); }
            set { dexterityModifierSource = value.ToString(); }
        }
        public ValueWithIntModifiers dexterityModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers sizeModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers naturalArmor = new ValueWithIntModifiers();
        public ValueWithIntModifiers deflectionModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();

        public virtual object Clone
        {
            get
            {
                return new ArmorClass()
                {
                    armorBonus = armorBonus.Clone as ValueWithIntModifiers,
                    itemsArmorBonus = itemsArmorBonus,
                    shieldBonus = shieldBonus.Clone as ValueWithIntModifiers,
                    itemsShieldBonus = itemsShieldBonus,
                    dexterityModifierSource = dexterityModifierSource,
                    dexterityModifier = dexterityModifier.Clone as ValueWithIntModifiers,
                    sizeModifier = sizeModifier.Clone as ValueWithIntModifiers,
                    naturalArmor = naturalArmor.Clone as ValueWithIntModifiers,
                    deflectionModifier = deflectionModifier.Clone as ValueWithIntModifiers,
                    miscModifiers = miscModifiers.Clone as ValueWithIntModifiers,
                };
            }
        }

        public bool Equals(ArmorClass other)
        {
            if (itemsArmorBonus != other.itemsArmorBonus)
                return false;
            if (!armorBonus.Equals(other.armorBonus))
                return false;
            if (itemsShieldBonus != other.itemsShieldBonus)
                return false;
            if (!shieldBonus.Equals(other.shieldBonus))
                return false;
            if (dexterityModifierSource != other.dexterityModifierSource)
                return false;
            if (!dexterityModifier.Equals(other.dexterityModifier))
                return false;
            if (!sizeModifier.Equals(other.sizeModifier))
                return false;
            if (!naturalArmor.Equals(other.naturalArmor))
                return false;
            if (!deflectionModifier.Equals(other.deflectionModifier))
                return false;
            if (!miscModifiers.Equals(other.miscModifiers))
                return false;
            return true;
        }

        private int GetBaseArmorClass(CharacterSheet sheet)
        {
            return 10 + sizeModifier.GetTotal(sheet) + deflectionModifier.GetTotal(sheet) + miscModifiers.GetTotal(sheet);
        }

        public int GetArmorBonus(CharacterSheet sheet, ArmorClassItem.ArmorTypes type)
        {
            var ac = 0;
            foreach (var item in sheet.armorClassItems)
            {
                if (item == null)
                    continue;
                if (!item.active)
                    continue;
                if (item.ArmorType != type)
                    continue;
                ac += item.armorBonus.GetTotal(sheet);
            }
            return ac;
        }

        public int GetArmorBonus(CharacterSheet sheet)
        {
            if (!itemsArmorBonus)
                return armorBonus.GetTotal(sheet);
            return GetArmorBonus(sheet, ArmorClassItem.ArmorTypes.Armor);
        }

        public int GetShieldBonus(CharacterSheet sheet)
        {
            if (!itemsShieldBonus)
                return armorBonus.GetTotal(sheet);
            return GetArmorBonus(sheet, ArmorClassItem.ArmorTypes.Shield);
        }

        private ValueWithIntModifiers GetDexBonusLimit(CharacterSheet sheet)
        {
            ValueWithIntModifiers minValue = null;
            var value = 0;
            foreach (var item in sheet.armorClassItems)
            {
                if (item == null)
                    continue;
                if (!item.active)
                    continue;
                if (!item.limitMaxDexBonus)
                    continue;
                var itemValue = item.maxDexBonus.GetTotal(sheet);
                if ((minValue == null) || (value > itemValue))
                {
                    minValue = item.maxDexBonus;
                    value = itemValue;
                }
            }
            return minValue;
        }

        public int GetDexterityModifier(CharacterSheet sheet)
        {
            var full = CharacterSheet.GetAbilityModifier(sheet, Ability.Dexterity);
            switch (DexterityModifierSource)
            {
                case DexterityModifierSources.DependsOnACItems:
                    var limit = GetDexBonusLimit(sheet);
                    if (limit == null)
                        return full;
                    var maxValue = limit.GetTotal(sheet);
                    if (full < maxValue)
                        return full;
                    return maxValue;
                case DexterityModifierSources.Full:
                    return full;
                case DexterityModifierSources.Custom:
                    return dexterityModifier.GetTotal(sheet);
                default:
                    return full;
            }
        }

        public int GetTotal(CharacterSheet sheet)
        {
            return GetBaseArmorClass(sheet) + GetDexterityModifier(sheet) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.GetTotal(sheet);
        }

        public int GetTouch(CharacterSheet sheet)
        {
            return GetBaseArmorClass(sheet) + GetDexterityModifier(sheet);
        }

        public int GetFlatFooted(CharacterSheet sheet)
        {
            return GetBaseArmorClass(sheet) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.GetTotal(sheet);
        }
    }
}
