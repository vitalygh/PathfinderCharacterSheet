using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public interface IApplicable<T>
    {
        T Apply(T value);
    }
}
