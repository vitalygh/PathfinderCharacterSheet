using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ItemWithUseLimit : ItemWithDescription
    {
        public bool hasUseLimit = false;
        public ValueWithIntModifiers useLimit = new ValueWithIntModifiers();
        public ValueWithIntModifiers dailyUseLimit = new ValueWithIntModifiers();

        public override string AsString(CharacterSheet sheet)
        {
            var text = name;

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
                var clone = new ItemWithUseLimit();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(ItemWithUseLimit other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            if (hasUseLimit != other.hasUseLimit)
                return false;
            if (!useLimit.Equals(other.useLimit))
                return false;
            if (!dailyUseLimit.Equals(other.dailyUseLimit))
                return false;
            return true;
        }

        public ItemWithUseLimit Fill(ItemWithUseLimit source)
        {
            if (source == null)
                return this;
            base.Fill(source);
            hasUseLimit = source.hasUseLimit;
            useLimit = source.useLimit.Clone as ValueWithIntModifiers;
            dailyUseLimit = source.dailyUseLimit.Clone as ValueWithIntModifiers;
            return this;
        }
    }
}
