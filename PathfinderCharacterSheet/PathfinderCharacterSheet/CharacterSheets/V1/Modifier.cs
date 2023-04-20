using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Modifier<T>: IContextValue<T>, IEquatable<Modifier<T>>, IPrototype<Modifier<T>>
    {
        public bool active = true;
        public virtual bool IsActive { get { return active; } }
        public string name = null;
        public virtual string Name { get { return name; } }
        public T value = default(T);
        public virtual T GetValue(CharacterSheet context) { return value; }

        public virtual Modifier<T> Clone
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

        public virtual Modifier<T> Fill(Modifier<T> source)
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
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.IsActive != IsActive)
                return false;
            if (other.Name != Name)
                return false;
            if (!other.value.Equals(value))
                return false;
            return true;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as Modifier<T>);
        }

        public static bool operator ==(Modifier<T> first, Modifier<T> second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(Modifier<T> first, Modifier<T> second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + active.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, name) ? name.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, value) ? value.GetHashCode() : 0);
            return hash;
        }
    }
}
