using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class LevelOfClassList : List<LevelOfClass>, IContextValue<int>, IEquatable<LevelOfClassList>, IPrototype<LevelOfClassList>
    {
        public virtual int GetValue(CharacterSheet context)
        {
            int value = 0;
            foreach (var level in this)
                value += level.GetValue(context);
            return value;
        }

        public virtual LevelOfClassList Clone
        {
            get
            {
                var levels = new LevelOfClassList();
                levels.Fill(this);
                return levels;
            }
        }

        public virtual LevelOfClassList Fill(LevelOfClassList source)
        {
            if (source == null)
                return this;
            Clear();
            foreach (var level in source)
                Add(level?.Clone);
            return this;
        }

        public bool Equals(LevelOfClassList other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return System.Linq.Enumerable.SequenceEqual(this, other);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as LevelOfClassList);
        }

        public static bool operator ==(LevelOfClassList first, LevelOfClassList second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(LevelOfClassList first, LevelOfClassList second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string AsString(CharacterSheet sheet)
        {
            var level = string.Empty;
            var totalLevel = 0;
            foreach (var loc in this)
            {
                if (loc == null)
                    continue;
                if (level.Length > 0)
                    level += ", ";
                var lvl = loc.GetValue(sheet); ;
                level += loc.ClassName + " (" + lvl.ToString() + ")";
                totalLevel += lvl;
            }
            if (level.Length > 0)
                level = totalLevel.ToString() + ": " + level;
            return level;
        }
    }
}
