using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Modifier<T>: IContextValue<T>, IEquatable<Modifier<T>>, IPrototype<Modifier<T>>
    {
        public bool active = true;
        public string name = null;
        public T value = default;
        public virtual T GetValue(CharacterSheet context)
        {
            return value;
        }

        public virtual Modifier<T> Clone
        {
            get
            {
                var modifier = new Modifier<T>();
                modifier.Fill(this);
                return modifier;
            }
        }

        public virtual Modifier<T> Fill(Modifier<T> source)
        {
            if (source == null)
                return this;
            active = source.active;
            name = source.name;
            value = source.value;
            return this;
        }

        public bool Equals(Modifier<T> other)
        {
            if (other == null)
                return false;
            if (active != other.active)
                return false;
            if (name != other.name)
                return false;
            if (!value.Equals(other.value))
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
            return Equals(other as Modifier<T>);
        }

        public static bool operator ==(Modifier<T> first, Modifier<T> second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
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
            hash = (hash * 7) + (name is null ? 0 : name.GetHashCode());
            hash = (hash * 7) + (!ReferenceEquals(null, value) ? value.GetHashCode() : 0);
            return hash;
        }
    }
}
