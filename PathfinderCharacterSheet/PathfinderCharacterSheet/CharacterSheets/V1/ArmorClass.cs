using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ArmorClass: IPrototype<ArmorClass>, IEquatable<ArmorClass>
    {
        public ValueWithIntModifiers armorBonus = new ValueWithIntModifiers();
        public bool itemsArmorBonus = true;
        public ValueWithIntModifiers shieldBonus = new ValueWithIntModifiers();
        public bool itemsShieldBonus = true;
        public const DexterityModifierSource DefaultDexterityModifierSource = DexterityModifierSource.DependsOnACItems;
        public string dexterityModifierSource = DefaultDexterityModifierSource.ToString();
        internal DexterityModifierSource DexterityModifierSource
        {
            get { return Helpers.GetEnumValue(dexterityModifierSource, DefaultDexterityModifierSource); }
            set { dexterityModifierSource = value.ToString(); }
        }
        public ValueWithIntModifiers dexterityModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers sizeModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers naturalArmor = new ValueWithIntModifiers();
        public ValueWithIntModifiers deflectionModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();

        public virtual ArmorClass Clone
        {
            get
            {
                var armorClass = new ArmorClass();
                armorClass.Fill(this);
                return armorClass;
            }
        }

        public virtual ArmorClass Fill(ArmorClass other)
        {
            if (other == null)
                return this;
            armorBonus = other.armorBonus?.Clone;
            itemsArmorBonus = other.itemsArmorBonus;
            shieldBonus = other.shieldBonus?.Clone;
            itemsShieldBonus = other.itemsShieldBonus;
            dexterityModifierSource = other.dexterityModifierSource;
            dexterityModifier = other.dexterityModifier?.Clone;
            sizeModifier = other.sizeModifier?.Clone;
            naturalArmor = other.naturalArmor?.Clone;
            deflectionModifier = other.deflectionModifier?.Clone;
            miscModifiers = other.miscModifiers?.Clone;
            return this;
        }

        public bool Equals(ArmorClass other)
        {
            if (other == null)
                return false;
            if (itemsArmorBonus != other.itemsArmorBonus)
                return false;
            if (armorBonus != other.armorBonus)
                return false;
            if (itemsShieldBonus != other.itemsShieldBonus)
                return false;
            if (shieldBonus != other.shieldBonus)
                return false;
            if (dexterityModifierSource != other.dexterityModifierSource)
                return false;
            if (dexterityModifier != other.dexterityModifier)
                return false;
            if (sizeModifier != other.sizeModifier)
                return false;
            if (naturalArmor != other.naturalArmor)
                return false;
            if (deflectionModifier != other.deflectionModifier)
                return false;
            if (miscModifiers != other.miscModifiers)
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
            return Equals(other as ArmorClass);
        }

        public static bool operator ==(ArmorClass first, ArmorClass second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(ArmorClass first, ArmorClass second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (armorBonus is null ? 0 : armorBonus.GetHashCode());
            hash = (hash * 7) + itemsArmorBonus.GetHashCode();
            hash = (hash * 7) + (shieldBonus is null ? 0 : shieldBonus.GetHashCode());
            hash = (hash * 7) + itemsShieldBonus.GetHashCode();
            hash = (hash * 7) + (dexterityModifierSource is null ? 0 : dexterityModifierSource.GetHashCode());
            hash = (hash * 7) + (dexterityModifier is null ? 0 : dexterityModifier.GetHashCode());
            hash = (hash * 7) + (sizeModifier is null ? 0 : sizeModifier.GetHashCode());
            hash = (hash * 7) + (naturalArmor is null ? 0 : naturalArmor.GetHashCode());
            hash = (hash * 7) + (deflectionModifier is null ? 0 : deflectionModifier.GetHashCode());
            hash = (hash * 7) + (miscModifiers is null ? 0 : miscModifiers.GetHashCode());
            return hash;
        }

        private int GetBaseArmorClass(CharacterSheet sheet)
        {
            return 10 + sizeModifier.GetValue(sheet) + deflectionModifier.GetValue(sheet) + miscModifiers.GetValue(sheet);
        }

        public int GetArmorBonus(CharacterSheet sheet, ArmorType type)
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
                ac += item.armorBonus.GetValue(sheet);
            }
            return ac;
        }

        public int GetArmorBonus(CharacterSheet sheet)
        {
            if (!itemsArmorBonus)
                return armorBonus.GetValue(sheet);
            return GetArmorBonus(sheet, ArmorType.Armor);
        }

        public int GetShieldBonus(CharacterSheet sheet)
        {
            if (!itemsShieldBonus)
                return armorBonus.GetValue(sheet);
            return GetArmorBonus(sheet, ArmorType.Shield);
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
                var itemValue = item.maxDexBonus.GetValue(sheet);
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
            if (sheet == null)
                return 0;
            var full = sheet.GetAbilityModifier(Ability.Dexterity);
            switch (DexterityModifierSource)
            {
                case DexterityModifierSource.DependsOnACItems:
                    var limit = GetDexBonusLimit(sheet);
                    if (limit == null)
                        return full;
                    var maxValue = limit.GetValue(sheet);
                    if (full < maxValue)
                        return full;
                    return maxValue;
                case DexterityModifierSource.Full:
                    return full;
                case DexterityModifierSource.Custom:
                    return dexterityModifier.GetValue(sheet);
                default:
                    return full;
            }
        }

        public int GetTotal(CharacterSheet sheet)
        {
            return GetBaseArmorClass(sheet) + GetDexterityModifier(sheet) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.GetValue(sheet);
        }

        public int GetTouch(CharacterSheet sheet)
        {
            return GetBaseArmorClass(sheet) + GetDexterityModifier(sheet);
        }

        public int GetFlatFooted(CharacterSheet sheet)
        {
            return GetBaseArmorClass(sheet) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.GetValue(sheet);
        }
    }
}
