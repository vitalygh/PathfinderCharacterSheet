using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Modifier<T>
    {
        public bool active = true;
        public virtual bool IsActive { get { return active; } }
        public string name = null;
        public virtual string Name { get { return name; } }
        public T value = default(T);
        public virtual T GetValue(CharacterSheet sheet) { return value; }

        public virtual object Clone
        {
            get
            {
                return new Modifier<T>()
                {
                    active = active,
                    name = name,
                    value = value,
                };
            }
        }

        public static List<Modifier<T>> CreateClone(List<Modifier<T>> mods)
        {
            if (mods == null)
                return null;
            var list = new List<Modifier<T>>();
            foreach (var m in mods)
                if (m != null)
                    list.Add(m.Clone as Modifier<T>);
                else
                    list.Add(m);
            return list;
        }

        public Modifier<T> Fill(Modifier<T> source)
        {
            if (source == null)
                return this;
            active = source.IsActive;
            name = source.Name;
            value = source.value;
            return this;
        }

        public bool Equals(Modifier<T> other)
        {
            if (other == null)
                return false;
            if (other.IsActive != IsActive)
                return false;
            if (other.Name != Name)
                return false;
            if (!other.value.Equals(value))
                return false;
            return true;
        }

        public static T Sum<S>(CharacterSheet sheet, List<Modifier<T>> modifiers) where S : ISummable<T>, new()
        {
            return CharacterSheet.Sum<Modifier<T>, T, S>(sheet, modifiers);
        }
    }
}
