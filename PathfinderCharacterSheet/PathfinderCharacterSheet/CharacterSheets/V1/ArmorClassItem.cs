using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ArmorClassItem : GearItem
    {
        public enum ArmorTypes
        {
            Armor,
            Shield,
            Other,
        }

        public ValueWithIntModifiers armorBonus = new ValueWithIntModifiers();
        public string ArmorBonus(CharacterSheet sheet)
        {
            var ab = armorBonus.GetTotal(sheet);
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
            var mdb = maxDexBonus.GetTotal(sheet);
            return mdb >= 0 ? "+" + mdb : mdb.ToString();
        }
        public ValueWithIntModifiers checkPenalty = new ValueWithIntModifiers();
        public string CheckPenalty(CharacterSheet sheet)
        {
            return checkPenalty.GetTotal(sheet).ToString();
        }
        public ValueWithIntModifiers spellFailure = new ValueWithIntModifiers();
        public string SpellFailure(CharacterSheet sheet)
        {
            return spellFailure.GetTotal(sheet) + "%";
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

        public override object Clone
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
            if (!armorBonus.Equals(other.armorBonus))
                return false;
            if (armorType != other.armorType)
                return false;
            if (limitMaxDexBonus != other.limitMaxDexBonus)
                return false;
            if (!maxDexBonus.Equals(other.maxDexBonus))
                return false;
            if (!checkPenalty.Equals(other.checkPenalty))
                return false;
            if (!spellFailure.Equals(other.spellFailure))
                return false;
            if (properties != other.properties)
                return false;
            return true;
        }

        public ArmorClassItem Fill(ArmorClassItem source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            selected = source.selected;
            armorBonus = source.armorBonus.Clone as ValueWithIntModifiers;
            armorType = source.armorType;
            limitMaxDexBonus = source.limitMaxDexBonus;
            maxDexBonus = source.maxDexBonus.Clone as ValueWithIntModifiers;
            checkPenalty = source.checkPenalty.Clone as ValueWithIntModifiers;
            spellFailure = source.spellFailure.Clone as ValueWithIntModifiers;
            properties = source.properties;
            return this;
        }
    }
}
