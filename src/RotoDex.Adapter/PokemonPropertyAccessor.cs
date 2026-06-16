using System;
using System.Reflection;

namespace RotoDex.Adapter
{
    public static class PokemonPropertyAccessor
    {
        public static bool TrySetProperty(IPokemon pokemon, string propertyName, string value)
        {
            var propInfo = typeof(IPokemon).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            if (propInfo == null || !propInfo.CanWrite)
                return false;

            try
            {
                object? convertedValue = ConvertValue(value, propInfo.PropertyType);
                if (convertedValue != null)
                {
                    propInfo.SetValue(pokemon, convertedValue);
                    return true;
                }
            }
            catch
            {
                // Ignore conversion or setter errors for batch operations
            }

            return false;
        }

        private static object? ConvertValue(string value, Type targetType)
        {
            if (targetType == typeof(string)) return value;
            if (targetType == typeof(int)) return int.Parse(value);
            if (targetType == typeof(uint)) return uint.Parse(value);
            if (targetType == typeof(ushort)) return ushort.Parse(value);
            if (targetType == typeof(short)) return short.Parse(value);
            if (targetType == typeof(byte)) return byte.Parse(value);
            if (targetType == typeof(bool)) return bool.Parse(value);

            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, true);

            return null;
        }
    }
}
