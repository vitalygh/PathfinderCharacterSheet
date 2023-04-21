using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public interface IPrototype<T>
    {
        T Clone { get; }
        T Fill(T other);
    }
}
