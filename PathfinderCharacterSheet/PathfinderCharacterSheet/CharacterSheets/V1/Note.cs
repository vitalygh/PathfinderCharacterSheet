﻿using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Note: ItemWithDescription, IEquatable<Note>
    {
        public override ItemWithDescription Clone
        {
            get
            {
                var clone = new Note();
                clone.Fill(this);
                return clone;
            }
        }

        public bool Equals(Note other)
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
