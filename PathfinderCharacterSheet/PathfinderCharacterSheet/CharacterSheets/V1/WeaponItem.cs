using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class WeaponItem : GearItem
    {
        public ValueWithIntModifiers attackBonus = new ValueWithIntModifiers();
        public string AttackBonus(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            var wab = attackBonus.GetTotal(sheet);
            var bonus = wab >= 0 ? "+" + wab : wab.ToString();
            bonus += " (" + sheet.GetAttackBonus(wab) + ")";
            return bonus;
        }
        public CriticalHit critical = new CriticalHit();
        public List<DiceRoll> damageRolls = new List<DiceRoll>();
        public string Damage(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            var wd = string.Empty;
            foreach (var roll in damageRolls)
            {
                var rollStr = roll.AsString(sheet);
                if (string.IsNullOrWhiteSpace(rollStr))
                    continue;
                if (!string.IsNullOrWhiteSpace(wd))
                    wd += " + ";
                wd += rollStr;
            }
            return wd;
        }
        public string DamageBonus(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            var wdb = damageBonus.GetTotal(sheet);
            var db = sheet.DamageBonus + wdb;
            var bonus = (wdb >= 0 ? "+" + wdb : wdb.ToString());
            bonus += " (" + (db >= 0 ? "+" + db : db.ToString()) + ")";
            return bonus;
        }
        public override string AsString(CharacterSheet sheet)
        {
            var weapon = string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
                weapon += name + ": ";
            weapon += AttackBonus(sheet);
            var c = critical.AsString(sheet);
            if (!string.IsNullOrWhiteSpace(c))
                weapon += ", " + c;
            var d = Damage(sheet) + " " + DamageBonus(sheet);
            if (!string.IsNullOrWhiteSpace(d))
                weapon += ", " + d;
            var r = range.GetTotal(sheet);
            if (r > 0)
            {
                weapon += ", " + Range(sheet);
                weapon += ", " + ammunition.GetTotal(sheet).ToString();
            }
            return weapon;
        }
        public ValueWithIntModifiers damageBonus = new ValueWithIntModifiers();
        public string type = null;
        public ValueWithIntModifiers range = new ValueWithIntModifiers();
        public string Range(CharacterSheet sheet)
        {
            return range.GetTotal(sheet) + " ft";
        }
        public ValueWithIntModifiers ammunition = new ValueWithIntModifiers();
        public string special = null;

        public override object Clone
        {
            get
            {
                var clone = new WeaponItem();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(WeaponItem other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (!attackBonus.Equals(other.attackBonus))
                return false;
            if (!critical.Equals(other.critical))
                return false;
            if (!CharacterSheet.IsEqual(damageRolls, other.damageRolls))
                return false;
            if (!damageBonus.Equals(other.damageBonus))
                return false;
            if (type != other.type)
                return false;
            if (!range.Equals(other.range))
                return false;
            if (!ammunition.Equals(other.ammunition))
                return false;
            if (special != other.special)
                return false;
            return true;
        }

        public WeaponItem Fill(WeaponItem source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            attackBonus = source.attackBonus.Clone as ValueWithIntModifiers;
            critical = source.critical.Clone as CriticalHit;
            damageRolls = new List<DiceRoll>();
            foreach (var roll in source.damageRolls)
                damageRolls.Add(roll.Clone as DiceRoll);
            damageBonus = source.damageBonus.Clone as ValueWithIntModifiers;
            type = source.type;
            range = source.range.Clone as ValueWithIntModifiers;
            ammunition = source.ammunition.Clone as ValueWithIntModifiers;
            special = source.special;
            return this;
        }
    }
}
