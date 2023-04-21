using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Helpers
    {
        public static bool IsEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            if ((first is null) && (second is null))
                return true;
            if ((first is null) || (second is null))
                return false;
            return System.Linq.Enumerable.SequenceEqual(first, second);
        }

        public static T GetEnumValue<T>(string text, T defaultValue) where T : struct
        {
            if (Enum.TryParse(text, out T result))
                return result;
            return defaultValue;
        }
    }
}
