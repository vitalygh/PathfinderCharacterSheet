using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public interface IPlatformProxy
    {
        string PersistentDataPath { get; }
    }
}
