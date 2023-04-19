using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class ModifiersList<M, T, S> : List<M> where S : ISummable<T>, new() where M : Modifier<T>
    {
        public T GetTotal(CharacterSheet sheet) { return CharacterSheet.Sum<M, T, S>(sheet, this); }
        public virtual object Clone { get { return new ModifiersList<M, T, S>().Fill(this); } }
        public ModifiersList<M, T, S> Fill(ModifiersList<M, T, S> source)
        {
            Clear();
            foreach (var m in source)
                Add(m.Clone as M);
            return this;
        }
    }
}
