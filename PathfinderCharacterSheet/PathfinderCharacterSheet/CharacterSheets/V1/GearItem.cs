using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1 
{
    public class GearItem : ItemWithUseLimit
    {
        public bool active = false;
        public ValueWithIntModifiers amount = new ValueWithIntModifiers() { baseValue = 1, };
        public ValueWithIntModifiers weight = new ValueWithIntModifiers();

        public int TotalWeight(CharacterSheet sheet) { return amount.GetTotal(sheet) * weight.GetTotal(sheet); }
        public override string AsString(CharacterSheet sheet)
        {
            var text = string.Empty;
            var count = amount.GetTotal(sheet);
            if (count > 1)
                text += "(" + count + ")";

            if (!string.IsNullOrWhiteSpace(name))
                text += " ";
            text += name;

            if (hasUseLimit)
            {
                if (!string.IsNullOrWhiteSpace(text))
                    text += " ";
                var ul = useLimit.GetTotal(sheet);
                text += "[" + ul;
                var dul = dailyUseLimit.GetTotal(sheet);
                if (dul > 0)
                    text += " / " + dul;
                text += "]";
            }

            return text;
        }

        public override object Clone
        {
            get
            {
                var clone = new GearItem();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(GearItem other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (active != other.active)
                return false;
            if (!amount.Equals(other.amount))
                return false;
            if (!weight.Equals(other.weight))
                return false;
            return true;
        }

        public GearItem Fill(GearItem source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            active = source.active;
            amount = source.amount.Clone as ValueWithIntModifiers;
            weight = source.weight.Clone as ValueWithIntModifiers;
            return this;
        }
    }
}
