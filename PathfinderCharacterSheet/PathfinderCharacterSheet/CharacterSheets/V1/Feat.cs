using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Feat: ItemWithDescription, IEquatable<Feat>
    {
        public override ItemWithDescription Clone
        {
            get
            {
                var clone = new Feat();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(Feat other)
        {
            if (other == null)
                return false;
            if (!base.Equals(other))
                return false;
            return true;
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
