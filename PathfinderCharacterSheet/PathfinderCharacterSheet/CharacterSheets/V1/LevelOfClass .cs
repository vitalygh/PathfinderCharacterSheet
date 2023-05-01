using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class LevelOfClass: IContextValue<int, CharacterSheet>, IPrototype<LevelOfClass>, IEquatable<LevelOfClass>
    {
        public ValueWithIntModifiers level = new ValueWithIntModifiers();
        public int GetValue(CharacterSheet sheet) { return level.GetValue(sheet); }
        public string className = null;
        public string ClassName { get { return className; } }

        public virtual LevelOfClass Clone
        {
            get
            {
                var loc = new LevelOfClass();
                loc.Fill(this);
                return loc;
            }
        }

        public bool Equals(LevelOfClass other)
        {
            if (other == null)
                return false;
            if (level != other.level)
                return false;
            if (className != other.className)
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
            return Equals(other as LevelOfClass);
        }

        public static bool operator ==(LevelOfClass first, LevelOfClass second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(LevelOfClass first, LevelOfClass second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (level is null ? 0 : level.GetHashCode());
            hash = (hash * 7) + (className is null ? 0 : className.GetHashCode());
            return hash;
        }

        public virtual LevelOfClass Fill(LevelOfClass source)
        {
            if (source == null)
                return this;
            level = source.level?.Clone;
            className = source.className;
            return this;
        }
    }
}
