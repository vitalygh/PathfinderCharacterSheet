using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class LevelOfClass
    {
        public ValueWithIntModifiers level = new ValueWithIntModifiers();
        public int GetLevel(CharacterSheet sheet) { return level.GetTotal(sheet); }
        public string className = null;
        public string ClassName { get { return className; } }

        public virtual object Clone
        {
            get
            {
                var loc = new LevelOfClass();
                loc.Fill(this);
                return loc;
            }
        }

        public virtual LevelOfClass Fill(LevelOfClass source)
        {
            if (source == null)
                return this;
            level = source.level.Clone as ValueWithIntModifiers;
            className = source.className;
            return this;
        }

        public static List<LevelOfClass> CreateClone(List<LevelOfClass> loc)
        {
            if (loc == null)
                return null;
            var list = new List<LevelOfClass>();
            foreach (var l in loc)
                if (l != null)
                    list.Add(l.Clone as LevelOfClass);
                else
                    list.Add(l);
            return list;
        }

        public static int Total(CharacterSheet sheet, List<LevelOfClass> levelOfClass)
        {
            var level = 0;
            if (levelOfClass != null)
                foreach (var loc in levelOfClass)
                {
                    if (loc == null)
                        continue;
                    level += loc.GetLevel(sheet);
                }
            return level;
        }

        public static string AsString(CharacterSheet sheet, List<LevelOfClass> levelOfClass)
        {
            var level = string.Empty;
            var totalLevel = 0;
            if (levelOfClass != null)
                foreach (var loc in levelOfClass)
                {
                    if (loc == null)
                        continue;
                    if (level.Length > 0)
                        level += ", ";
                    var lvl = loc.GetLevel(sheet); ;
                    level += loc.ClassName + " (" + lvl.ToString() + ")";
                    totalLevel += lvl;
                }
            if (level.Length > 0)
                level = totalLevel.ToString() + ": " + level;
            return level;
        }

        public bool Equals(LevelOfClass other)
        {
            if (other == null)
                return false;
            if (!other.level.Equals(level))
                return false;
            if (other.ClassName != ClassName)
                return false;
            return true;
        }

        public static bool Equal(List<LevelOfClass> a, List<LevelOfClass> b)
        {
            if ((a == null) && (b == null))
                return true;
            if ((a == null) || (b == null))
                return false;
            var count = a.Count;
            if (count != b.Count)
                return false;
            for (var i = 0; i < count; i++)
            {
                var ai = a[i];
                var bi = b[i];
                if ((ai == null) && (bi == null))
                    continue;
                if ((ai == null) || (bi == null))
                    return false;
                if (!ai.Equals(bi))
                    return false;
            }
            return true;
        }
    }
}
