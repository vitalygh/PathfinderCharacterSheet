using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ItemWithDescription
    {
        public int uid = -1;
        public bool selected = false;
        public string name = null;
        public string description = null;

        public ItemWithDescription()
        {
            uid = CharacterSheetStorage.GetUID();
        }

        public virtual object Clone
        {
            get
            {
                var clone = new ItemWithDescription();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(ItemWithDescription other)
        {
            if (other == null)
                return false;
            if (uid != other.uid)
                return false;
            if (selected != other.selected)
                return false;
            if (name != other.name)
                return false;
            if (description != other.description)
                return false;
            return true;
        }

        public ItemWithDescription Fill(ItemWithDescription source)
        {
            if (source == null)
                return this;
            uid = source.uid;
            selected = source.selected;
            name = source.name;
            description = source.description;
            return this;
        }

        public virtual string AsString(CharacterSheet sheet)
        {
            return name;
        }
    }
}
