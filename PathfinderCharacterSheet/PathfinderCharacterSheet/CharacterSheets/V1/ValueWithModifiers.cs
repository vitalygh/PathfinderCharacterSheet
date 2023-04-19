using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ValueWithModifiers<M, T, S> where S : ISummable<T>, new() where M : Modifier<T>
    {
        public T baseValue = default(T);
        public ModifiersList<M, T, S> modifiers = new ModifiersList<M, T, S>();

        public virtual T GetTotal(CharacterSheet sheet) { return CharacterSheet.Sum<T, S>(baseValue, modifiers.GetTotal(sheet)); }

        public virtual object Clone
        {
            get
            {
                var clone = new ValueWithModifiers<M, T, S>();
                clone.Fill(this);
                return clone;
            }
        }

        public ValueWithModifiers<M, T, S> Fill(ValueWithModifiers<M, T, S> source)
        {
            if (source == null)
                return this;
            baseValue = source.baseValue;
            modifiers = source.modifiers.Clone as ModifiersList<M, T, S>;
            return this;
        }

        public bool Equals(ValueWithModifiers<M, T, S> other)
        {
            if (other == null)
                return false;
            if (!other.baseValue.Equals(baseValue))
                return false;
            if (!CharacterSheet.IsEqual(other.modifiers, modifiers))
                return false;
            return true;
        }
    }
}
