using System;
using System.Collections.Generic;
using System.Linq;

namespace CinderellaGirlsCardViewer
{
    public static class EnumEx
    {
        public static T ToEnum<T>(this string name)
        {
            return EnumExInner<T>.ToEnum(name);
        }

        private static class EnumExInner<T>
        {
            private static readonly Dictionary<string, T> Enums
                = ((T[])Enum.GetValues(typeof(T)))
                    .ToDictionary(type => type.ToString().ToLower());

            public static T ToEnum(string name)
            {
                var result = Enums.FirstOrDefault(pair => name.IndexOf(pair.Key, StringComparison.OrdinalIgnoreCase) >= 0);
                return result.Key != null ? result.Value : default(T);
            }
        }
    }
}
