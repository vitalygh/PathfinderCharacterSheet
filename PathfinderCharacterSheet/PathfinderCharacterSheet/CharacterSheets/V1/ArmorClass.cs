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

        public virtual ArmorClass Clone
        {
            get
            {
                return new ArmorClass()
                {
                    armorBonus = armorBonus?.Clone,
                    itemsArmorBonus = itemsArmorBonus,
                    shieldBonus = shieldBonus?.Clone,
                    itemsShieldBonus = itemsShieldBonus,
                    dexterityModifierSource = dexterityModifierSource,
                    dexterityModifier = dexterityModifier?.Clone,
                    sizeModifier = sizeModifier?.Clone,
                    naturalArmor = naturalArmor?.Clone,
                    deflectionModifier = deflectionModifier?.Clone,
                    miscModifiers = miscModifiers?.Clone,
                };
            }
        }

        public bool Equals(ArmorClass other)
        {
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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, armorBonus) ? armorBonus.GetHashCode() : 0);
            hash = (hash * 7) + itemsArmorBonus.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, shieldBonus) ? shieldBonus.GetHashCode() : 0);
            hash = (hash * 7) + itemsShieldBonus.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, dexterityModifierSource) ? dexterityModifierSource.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, dexterityModifier) ? dexterityModifier.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, sizeModifier) ? sizeModifier.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, naturalArmor) ? naturalArmor.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, deflectionModifier) ? deflectionModifier.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, miscModifiers) ? miscModifiers.GetHashCode() : 0);
            return hash;
        }

        private int GetBaseArmorClass(CharacterSheet sheet)
        {
            return 10 + sizeModifier.GetValue(sheet) + deflectionModifier.GetValue(sheet) + miscModifiers.GetValue(sheet);
        }

        public int GetArmorBonus(CharacterSheet sheet, ArmorTypes type)
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
            return GetArmorBonus(sheet, ArmorTypes.Armor);
        }

        public int GetShieldBonus(CharacterSheet sheet)
        {
            if (!itemsShieldBonus)
                return armorBonus.GetValue(sheet);
            return GetArmorBonus(sheet, ArmorTypes.Shield);
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
            var full = CharacterSheet.GetAbilityModifier(sheet, Ability.Dexterity);
            switch (DexterityModifierSource)
            {
                case DexterityModifierSources.DependsOnACItems:
                    var limit = GetDexBonusLimit(sheet);
                    if (limit == null)
                        return full;
                    var maxValue = limit.GetValue(sheet);
                    if (full < maxValue)
                        return full;
                    return maxValue;
                case DexterityModifierSources.Full:
                    return full;
                case DexterityModifierSources.Custom:
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
