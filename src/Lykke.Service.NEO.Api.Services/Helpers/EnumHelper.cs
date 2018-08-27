using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Lykke.Service.NEO.Api.Services.Helpers
{
    /// <summary>
    ///     This class implements helper methods for enumeration
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        ///     Returns a strongly typed enumeration field for given enum type
        /// </summary>
        public static T GetEnumType<T>(int value)
        {
            return (T) Enum.Parse(typeof (T), value.ToString());
        }

        /// <summary>
        ///     Returns a strongly typed enumeration field for given enum type
        /// </summary>
        public static T GetEnumType<T>(this string value)
        {
            return (T) Enum.Parse(typeof (T), value);
        }

        /// <summary>
        ///     Returns a strongly typed enumeration field for given enum type
        /// </summary>
        public static TEnum GetEnumTypeTry<TEnum>(this string value, TEnum defaultType) where TEnum : struct
        {
            TEnum resultInputType;
            return Enum.TryParse(value, true, out resultInputType) ? resultInputType : defaultType;
        }

        /// <summary>
        ///     Returns an integer value corresponding to the given enumeration field for a given enum type.
        /// </summary>
        public static int GetEnumValue<T>(T enumField)
        {
            return Convert.ToInt32(enumField);
        }

        /// <summary>
        ///     Reads the description attribute for the given enum field and returns its string representation
        /// </summary>
        public static string GetEnumFieldDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        ///     Gets list of enum fields for the given enum type
        /// </summary>
        public static List<string> GetFieldsFromEnum<T>()
        {
            return Enum.GetNames(typeof (T)).ToList();
        }

        /// <summary>
        ///     Gets list of enum fields for the given enum type
        /// </summary>
        public static List<T> GetAllFromEnum<T>()
        {
            var strings = GetFieldsFromEnum<T>();
            return strings.Select(GetEnumType<T>).ToList();
        }

        /// <summary>
        ///     Returns an enum filed for the given enum description
        /// </summary>
        public static T GetEnumTypeFromDescription<T>(string desc)
        {
            var enumType = typeof (T);
            var names = Enum.GetNames(enumType);
            foreach (var name in names)
            {
                if (GetEnumFieldDescription((Enum) Enum.Parse(enumType, name)).Equals(desc))
                {
                    return (T) Enum.Parse(enumType, name);
                }
            }
            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }

        /// <summary>
        ///     Convert enum to enum
        /// </summary>
        public static T EnumToEnum<T, TU>(TU enumArg)
        {
            if (!typeof (T).IsEnum) throw new Exception("This method only takes enumerations.");
            if (!typeof (TU).IsEnum) throw new Exception("This method only takes enumerations.");
            try
            {
                return (T) Enum.ToObject(typeof (T), enumArg);
            }
            catch
            {
                throw new Exception
                    (string.Format("Error converting enumeration {0} value {1} to enumeration {2} ",
                        enumArg,
                        typeof (TU),
                        typeof (T)));
            }
        }
    }
}
