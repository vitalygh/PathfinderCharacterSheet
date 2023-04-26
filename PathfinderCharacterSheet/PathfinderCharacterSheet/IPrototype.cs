using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public interface IPrototype<T>
    {
        T Clone { get; }
        T Fill(T other);
    }
}
