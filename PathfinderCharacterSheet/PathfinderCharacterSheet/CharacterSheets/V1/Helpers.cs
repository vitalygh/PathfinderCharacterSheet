using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public static class Helpers
    {
        public static bool SequenceEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
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

        public static T[] Populate<T>(this T[] array) where T: new()
        {
            return Populate(array, () => new T());
        }

        public static T[] Populate<T>(this T[] array, Func<T> provider)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = provider == null ? default(T) : provider();
            }
            return array;
        }
    }
}
