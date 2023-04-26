using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public class Settings: IEquatable<Settings>, IPrototype<Settings>
    {
        private bool saveChangesImmediately = false;
        public bool SaveChangesImmediately
        {
            get => saveChangesImmediately;
            set
            {
                if (saveChangesImmediately != value)
                    IsChanged = true;
                saveChangesImmediately = value;
            }
        }

        internal bool IsChanged { get; private set; }

        private static readonly string serializationName = "settings";
        public static readonly ISerializer<Settings> Serializer = new XmlFileSerializer<Settings>("settings");
        private static string serializationPath = null;

        public static Settings Load()
        {
            return Serializer.Load(serializationName);
        }

        public void Save()
        {
            serializationPath = Serializer.Save(serializationName, this, serializationPath);
            if (serializationPath != null)
                IsChanged = false;
        }

        public virtual Settings Clone
        {
            get
            {
                var clone = new Settings();
                clone.Fill(this);
                return clone;
            }
        }

        public virtual Settings Fill(Settings source)
        {
            if (source == null)
                return this;

            SaveChangesImmediately = source.SaveChangesImmediately;

            return this;
        }

        public bool Equals(Settings other)
        {
            if (other == null)
                return false;
            if (SaveChangesImmediately != other.SaveChangesImmediately)
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
            return Equals(other as Settings);
        }

        public static bool operator ==(Settings first, Settings second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(Settings first, Settings second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + SaveChangesImmediately.GetHashCode();
            return hash;
        }
    }
}
