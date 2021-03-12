using System;
using System.Globalization;
using System.Resources;

namespace Carfamsoft.ModelToView.Shared
{
    /// <summary>
    /// Provides extension methods for various types.
    /// </summary>
    public static class TypeExtensions
    {
        private const string DateFormat = "yyyy-MM-dd"; // Compatible with HTML date inputs

        /// <summary>
        /// Generates a globally unique identifier for the specified type.
        /// </summary>
        /// <param name="type">The type whose name will be prefixed to the identifier.</param>
        /// <param name="camelCase">true to use camel-casing, otherwise false.</param>
        /// <returns></returns>
        public static string GenerateId(this Type type, bool camelCase = false) => type.Name.GenerateId(camelCase);

        /// <summary>
        /// Generates a globally unique identifier for the specified string.
        /// </summary>
        /// <param name="name">The name used to prefix the identifier.</param>
        /// <param name="camelCase">true to use camel-casing, otherwise false.</param>
        /// <returns></returns>
        public static string GenerateId(this string name, bool camelCase = false) => $"{(camelCase ? name.ToCamelCase() : name)}_{Guid.NewGuid().GetHashCode():x}";

        /// <summary>
        /// Determines whether the type of the specified object is numeric.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns></returns>
        public static bool IsNumeric(this object obj) => true == obj?.GetType().IsNumeric();

        /// <summary>
        /// Determines whether the specified type is numeric.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns></returns>
        public static bool IsNumeric(this Type type)
        {
            if (type is null) return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether <paramref name="propertyType"/> is a boolean 
        /// and <paramref name="elementType"/> is 'checkbox'.
        /// </summary>
        /// <param name="propertyType">The type of the property to check.</param>
        /// <param name="elementType">The HTML input type to check.</param>
        /// <returns></returns>
        public static bool SupportsCheckbox(this Type propertyType, string elementType)
        {
            return propertyType == typeof(bool) && (elementType == "checkbox" || string.IsNullOrWhiteSpace(elementType));
        }

        /// <summary>
        /// Determines whether the target type is numeric.
        /// </summary>
        /// <param name="targetType">The target type to check.</param>
        /// <returns></returns>
        public static bool SupportsInputNumber(this Type targetType)
        {
            return targetType == typeof(int) ||
                    targetType == typeof(long) ||
                    targetType == typeof(short) ||
                    targetType == typeof(float) ||
                    targetType == typeof(double) ||
                    targetType == typeof(decimal);
        }

        /// <summary>
        /// Determines whether the specified type is a <see cref="string"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsString(this Type t) => t == typeof(string);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="byte"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsByte(this Type t) => t == typeof(byte);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsSByte(this Type t) => t == typeof(sbyte);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="char"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsChar(this Type t) => t == typeof(char);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="short"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsInt16(this Type t) => t == typeof(short);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="ushort"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsUInt16(this Type t) => t == typeof(ushort);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="int"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsInt32(this Type t) => t == typeof(int);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="uint"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsUInt32(this Type t) => t == typeof(uint);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="long"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsInt64(this Type t) => t == typeof(long);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="ulong"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsUInt64(this Type t) => t == typeof(ulong);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="float"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsSingle(this Type t) => t == typeof(float);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="double"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsDouble(this Type t) => t == typeof(double);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="decimal"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsDecimal(this Type t) => t == typeof(decimal);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="bool"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsBoolean(this Type t) => t == typeof(bool);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="DateTime"/> or <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsDate(this Type t) => t.IsDateTime() || t.IsDateTimeOffset();

        /// <summary>
        /// Determnines whether the specified type is a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsDateTime(this Type t) => t == typeof(DateTime);

        /// <summary>
        /// Determnines whether the specified type is a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the types match, otherwise false.</returns>
        public static bool IsDateTimeOffset(this Type t) => t == typeof(DateTimeOffset);

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific
        /// format to its System.Byte equivalent.
        /// </summary>
        /// <param name="t">A <see cref="byte"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style"></param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        /// When this method returns, contains the 8-bit unsigned integer value equivalent to the 
        /// number contained in s if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseByte(this Type t, string value, NumberStyles style, IFormatProvider provider, out byte result)
        {
            result = default;
            return t.IsByte() && byte.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style 
        /// and culture-specific format to its <see cref="sbyte"/> equivalent, and returns 
        /// a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="t">A <see cref="sbyte"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the permitted 
        /// format of <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        /// When this method returns, contains the 8-bit unsigned integer value equivalent to the number 
        /// contained in <paramref name="value"/> if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseSByte(this Type t, string value, NumberStyles style, IFormatProvider provider, out sbyte result)
        {
            result = default;
            return t.IsSByte() && sbyte.TryParse(value, style, provider, out result);
        }

        /// <summary>
        ///  Converts the value of the specified string to its equivalent Unicode character.
        /// </summary>
        /// <param name="t">A <see cref="char"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains a Unicode character equivalent to the sole
        /// character in <paramref name="value"/>, if the conversion succeeded, or an undefined 
        /// value if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseChar(this Type t, string value, out char result)
        {
            result = default;
            return t.IsChar() && char.TryParse(value, out result);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style 
        /// and culture-specific format to its 16-bit signed integer equivalent.
        /// </summary>
        /// <param name="t">A <see cref="short"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        /// When this method returns, contains the 16-bit signed integer value equivalent to the number 
        /// contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseInt16(this Type t, string value, NumberStyles style, IFormatProvider provider, out short result)
        {
            result = default;
            return t.IsInt16() && short.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style 
        /// and culture-specific format to its 16-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="t">A <see cref="ushort"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the 16-bit unsigned integer value equivalent to the number
        ///  contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseUInt16(this Type t, string value, NumberStyles style, IFormatProvider provider, out ushort result)
        {
            result = default;
            return t.IsUInt16() && ushort.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style 
        /// and culture-specific format to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="t">A <see cref="int"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the 32-bit signed integer value equivalent to the number
        ///  contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseInt32(this Type t, string value, NumberStyles style, IFormatProvider provider, out int result)
        {
            result = default;
            return t.IsInt32() && int.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style 
        /// and culture-specific format to its 32-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="t">A <see cref="uint"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the 32-bit unsigned integer value equivalent to the number
        ///  contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseUInt32(this Type t, string value, NumberStyles style, IFormatProvider provider, out uint result)
        {
            result = default;
            return t.IsUInt32() && uint.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style 
        /// and culture-specific format to its 64-bit signed integer equivalent.
        /// </summary>
        /// <param name="t">A <see cref="long"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the 64-bit signed integer value equivalent to the number
        ///  contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseInt64(this Type t, string value, NumberStyles style, IFormatProvider provider, out long result)
        {
            result = default;
            return t.IsInt64() && long.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the string representation of a number in a specified style 
        /// and culture-specific format to its 64-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="t">A <see cref="ulong"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the 64-bit unsigned integer value equivalent to the number
        ///  contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseUInt64(this Type t, string value, NumberStyles style, IFormatProvider provider, out ulong result)
        {
            result = default;
            return t.IsUInt64() && ulong.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific
        /// format to its single-precision floating-point number equivalent.
        /// </summary>
        /// <param name="t">A <see cref="float"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the single-precision floating-point number equivalent 
        ///  to the numeric value or symbol contained in <paramref name="value"/>, if the conversion succeeded,
        ///  or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseSingle(this Type t, string value, NumberStyles style, IFormatProvider provider, out float result)
        {
            result = default;
            return t.IsSingle() && float.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific
        /// format to its double-precision floating-point number equivalent.
        /// </summary>
        /// <param name="t">A <see cref="double"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the double-precision floating-point number equivalent 
        ///  to the numeric value or symbol contained in <paramref name="value"/>, if the conversion succeeded,
        ///  or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseDouble(this Type t, string value, NumberStyles style, IFormatProvider provider, out double result)
        {
            result = default;
            return t.IsDouble() && double.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="decimal"/> 
        /// equivalent using the specified style and culture-specific format.
        /// </summary>
        /// <param name="t">A <see cref="decimal"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style 
        /// elements that can be present in <paramref name="value"/>.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="value"/>.
        /// If <paramref name="provider"/> is null, the thread current culture is used.
        /// </param>
        /// <param name="result">
        ///  When this method returns, contains the decimal number equivalent to the numeric 
        ///  value or symbol contained in <paramref name="value"/>, if the conversion succeeded,
        ///  or zero if the conversion failed.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseDecimal(this Type t, string value, NumberStyles style, IFormatProvider provider, out decimal result)
        {
            result = default;
            return t.IsDecimal() && decimal.TryParse(value, style, provider, out result);
        }

        /// <summary>
        /// Tries to convert the specified string representation of a logical value to its
        /// <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="t">A <see cref="bool"/> type.</param>
        /// <param name="value">A string containing a number to convert.</param>
        /// <param name="result">
        /// When this method returns, if the conversion succeeded, contains true if value is equal to
        /// <see cref="bool.TrueString"/> or false if value is equal to <see cref="bool.FalseString"/>.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseBoolean(this Type t, string value, out bool result)
        {
            result = default;
            return t.IsBoolean() && bool.TryParse(value, out result);
        }

        /// <summary>
        /// Attempts to convert a value to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="t">A <see cref="DateTime"/> type.</param>
        /// <param name="value">A string containing the value to convert.</param>
        /// <param name="format">The format string to use in conversion.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use for conversion.</param>
        /// <param name="result">The converted value.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseDateTime(this Type t, string value, string format, CultureInfo culture, out DateTime result)
        {
            result = default;
            return t.IsDateTime() && BindConverter.TryConvertToDateTime(value, culture, format, out result);
        }

        /// <summary>
        /// Attempts to convert a value to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="t">The type of the value to convert.</param>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format string to use in conversion.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use for conversion.</param>
        /// <param name="result">The converted value.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryParseDateTimeOffset(this Type t, string value, string format, CultureInfo culture, out DateTimeOffset result)
        {
            result = default;
            return t.IsDateTimeOffset() && BindConverter.TryConvertToDateTimeOffset(value, culture, format, out result);
        }

        /// <summary>
        /// Returns a localized string for a property name.
        /// </summary>
        /// <param name="resourceManager">
        /// A resource manager that provides convenient access to culture-specific resources.
        /// </param>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">
        /// An object that represents the culture for which the resource is localized.
        /// </param>
        /// <returns>
        /// The value of the resource localized for the specified culture, 
        /// or null if name cannot be found in a resource set.
        /// </returns>
        public static string GetDisplayString(this ResourceManager resourceManager, string name, CultureInfo culture = null)
        {
            if (resourceManager != null && name.IsNotWhiteSpace())
            {
                try
                {
                    var result = resourceManager.GetString(name, culture);
                    if (result.IsWhiteSpace()) result = name.TitleCaseWords();
                    return result;
                }
                catch
                {
                }
            }
            return name.TitleCaseWords();
        }
    }
}
