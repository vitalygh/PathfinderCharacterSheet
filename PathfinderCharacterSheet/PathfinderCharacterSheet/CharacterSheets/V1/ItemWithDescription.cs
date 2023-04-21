using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ItemWithDescription: IPrototype<ItemWithDescription>, IEquatable<ItemWithDescription>
    {
        public int uid = CharacterSheet.InvalidUID;
        public bool selected = false;
        public string name = null;
        public string description = null;

        public virtual ItemWithDescription Clone
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

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as ItemWithDescription);
        }

        public static bool operator ==(ItemWithDescription first, ItemWithDescription second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(ItemWithDescription first, ItemWithDescription second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + uid.GetHashCode();
            hash = (hash * 7) + selected.GetHashCode();
            hash = (hash * 7) + (name is null ? 0 : name.GetHashCode());
            hash = (hash * 7) + (description is null ? 0 : description.GetHashCode());
            return hash;
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
