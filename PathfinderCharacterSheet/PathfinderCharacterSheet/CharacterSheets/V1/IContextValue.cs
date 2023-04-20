﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public interface IContextValue<T>
    {
        T GetValue(CharacterSheet context);
    }
}
