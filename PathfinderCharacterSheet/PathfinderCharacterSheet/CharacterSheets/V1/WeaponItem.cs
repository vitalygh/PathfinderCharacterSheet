using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class WeaponItem : GearItem, IEquatable<WeaponItem>
    {
        public ValueWithIntModifiers attackBonus = new ValueWithIntModifiers();
        public string AttackBonus(CharacterSheet sheet)
        {
            if (sheet == null)
                return null;
            var wab = attackBonus.GetValue(sheet);
            var bonus = wab >= 0 ? "+" + wab : wab.ToString();
            bonus += " (" + sheet.GetAttackBonus(wab) + ")";
            return bonus;
        }
        public CriticalHit critical = new CriticalHit();
        public DiceRollList damageRolls = new DiceRollList();
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
            var wdb = damageBonus.GetValue(sheet);
            var db = sheet.DamageBonus + wdb;
            var bonus = wdb >= 0 ? "+" + wdb : wdb.ToString();
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
            var r = range.GetValue(sheet);
            if (r > 0)
            {
                weapon += ", " + Range(sheet);
                weapon += ", " + ammunition.GetValue(sheet).ToString();
            }
            return weapon;
        }
        public ValueWithIntModifiers damageBonus = new ValueWithIntModifiers();
        public string type = null;
        public ValueWithIntModifiers range = new ValueWithIntModifiers();
        public string Range(CharacterSheet sheet)
        {
            return range.GetValue(sheet) + " ft";
        }
        public ValueWithIntModifiers ammunition = new ValueWithIntModifiers();
        public string special = null;

        public override ItemWithDescription Clone
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
            if (attackBonus != other.attackBonus)
                return false;
            if (critical != other.critical)
                return false;
            if (damageRolls != other.damageRolls)
                return false;
            if (damageBonus != other.damageBonus)
                return false;
            if (type != other.type)
                return false;
            if (range != other.range)
                return false;
            if (ammunition != other.ammunition)
                return false;
            if (special != other.special)
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
            return Equals(other as WeaponItem);
        }

        public static bool operator ==(WeaponItem first, WeaponItem second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(WeaponItem first, WeaponItem second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, attackBonus) ? attackBonus.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, critical) ? critical.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, damageRolls) ? damageRolls.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, damageBonus) ? damageBonus.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, type) ? type.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, range) ? range.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, ammunition) ? ammunition.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, special) ? special.GetHashCode() : 0);
            return hash;
        }

        public WeaponItem Fill(WeaponItem source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            attackBonus = source.attackBonus?.Clone;
            critical = source.critical?.Clone;
            damageRolls = source.damageRolls?.Clone;
            damageBonus = source.damageBonus?.Clone;
            type = source.type;
            range = source.range?.Clone;
            ammunition = source.ammunition?.Clone;
            special = source.special;
            return this;
        }
    }
}
