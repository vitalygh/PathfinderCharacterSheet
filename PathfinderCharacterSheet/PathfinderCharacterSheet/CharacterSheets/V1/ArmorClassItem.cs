using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ArmorClassItem : GearItem, IEquatable<GearItem>
    {
        public ValueWithIntModifiers armorBonus = new ValueWithIntModifiers();
        public string ArmorBonus(CharacterSheet sheet)
        {
            var ab = armorBonus.GetValue(sheet);
            return ab >= 0 ? "+" + ab : ab.ToString();
        }
        public string armorType = ArmorTypes.Other.ToString();
        public ArmorTypes ArmorType
        {
            get { return CharacterSheet.GetEnumValue(armorType, ArmorTypes.Other); }
            set { armorType = value.ToString(); }
        }
        public bool limitMaxDexBonus = false;
        public ValueWithIntModifiers maxDexBonus = new ValueWithIntModifiers();
        public string MaxDexBonus(CharacterSheet sheet)
        {
            if (!limitMaxDexBonus)
                return "-";
            var mdb = maxDexBonus.GetValue(sheet);
            return mdb >= 0 ? "+" + mdb : mdb.ToString();
        }
        public ValueWithIntModifiers checkPenalty = new ValueWithIntModifiers();
        public string CheckPenalty(CharacterSheet sheet)
        {
            return checkPenalty.GetValue(sheet).ToString();
        }
        public ValueWithIntModifiers spellFailure = new ValueWithIntModifiers();
        public string SpellFailure(CharacterSheet sheet)
        {
            return spellFailure.GetValue(sheet) + "%";
        }
        public string properties = null;
        public override string AsString(CharacterSheet sheet)
        {
            var armor = string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
                armor += name + " ";
            armor += "(" + armorType;
            //if (active)
            //    armor += ", active";
            armor += "): ";
            armor += ArmorBonus(sheet);
            if (limitMaxDexBonus)
                armor += ", " + MaxDexBonus(sheet);
            armor += ", " + CheckPenalty(sheet);
            armor += ", " + SpellFailure(sheet);
            if (!string.IsNullOrWhiteSpace(properties))
                armor += ", " + properties;
            return armor;
        }

        public override ItemWithDescription Clone
        {
            get
            {
                var clone = new ArmorClassItem();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(ArmorClassItem other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (armorBonus != other.armorBonus)
                return false;
            if (armorType != other.armorType)
                return false;
            if (limitMaxDexBonus != other.limitMaxDexBonus)
                return false;
            if (maxDexBonus != other.maxDexBonus)
                return false;
            if (checkPenalty != other.checkPenalty)
                return false;
            if (spellFailure != other.spellFailure)
                return false;
            if (properties != other.properties)
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
            return Equals(other as ArmorClassItem);
        }

        public static bool operator ==(ArmorClassItem first, ArmorClassItem second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(ArmorClassItem first, ArmorClassItem second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + selected.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, armorBonus) ? armorBonus.GetHashCode() : 0);
            hash = (hash * 7) + armorType.GetHashCode();
            hash = (hash * 7) + limitMaxDexBonus.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, maxDexBonus) ? maxDexBonus.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, checkPenalty) ? checkPenalty.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, spellFailure) ? spellFailure.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, properties) ? properties.GetHashCode() : 0);
            return hash;
        }

        public ArmorClassItem Fill(ArmorClassItem source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            selected = source.selected;
            armorBonus = source.armorBonus?.Clone;
            armorType = source.armorType;
            limitMaxDexBonus = source.limitMaxDexBonus;
            maxDexBonus = source.maxDexBonus?.Clone;
            checkPenalty = source.checkPenalty?.Clone;
            spellFailure = source.spellFailure?.Clone;
            properties = source.properties;
            return this;
        }
    }
}
