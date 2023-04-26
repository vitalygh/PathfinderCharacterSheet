using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public interface IContextValue<T, Context>
    {
        T GetValue(Context context);
    }
}
