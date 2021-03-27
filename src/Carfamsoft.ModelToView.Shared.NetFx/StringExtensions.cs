using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Carfamsoft.ModelToView.Shared
{
    /// <summary>
    /// Contains extension methods for an instance of the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Indicates whether the specified string is not null and not a System.String.Empty string after trimming.
        /// Functionally equivalent to the static method call !string.IsNullOrWhiteSpace(string).
        /// </summary>
        /// <param name="instance">The string to check.</param>
        /// <returns></returns>
        public static bool IsNotWhiteSpace(this string instance)
        {
            return instance != null && instance.Trim().Length > 0;
        }

        /// <summary>
        /// Indicates whether the specified string is null or a System.String.Empty string after trimming.
        /// Functionally equivalent to the static method call string.IsNullOrWhiteSpace(string).
        /// </summary>
        /// <param name="instance">The string to check.</param>
        /// <returns></returns>
        public static bool IsWhiteSpace(this string instance)
        {
            return instance == null || instance.Trim().Length == 0;
        }

        /// <summary>
        /// Removes and replaces with the given argument anything that is NOT in the following specified set of characters: 
        /// alpha-numeric (a-zA-Z_), dash (-), and period (.)
        /// </summary>
        /// <param name="value">The value from which to remove white space.</param>
        /// <param name="replacement">The replacement string for the characters to remove.</param>
        /// <returns></returns>
        public static string ReplaceWhiteSpace(this string value, string replacement = "-")
        {
            if (value == null)
            {
                return string.Empty;
            }

            // pattern to remove anything that is NOT in the following specified set of characters:
            //      alpha-numeric (a-zA-Z_), dash (-), and period (.)
            string pattern = @"[^\w-\.]";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            string tempName = r.Replace(value, " ");
            return string.Join(replacement, tempName.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Removes diacritics (accents) from the specified string.
        /// </summary>
        /// <param name="s">The string to alter.</param>
        public static string RemoveAccents(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            var chars = s.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Converts the specified string into words using its current title-casing.
        /// </summary>
        /// <param name="s">The string to split into words.</param>
        /// <returns>A string of one or more words.</returns>
        public static string TitleCaseWords(this string s)
        {
            if (s == null)
            {
                return null;
            }
            return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Converts the specified string to camel case.
        /// </summary>
        /// <param name="s">The string to convert to camcel case.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns></returns>
        public static string ToCamelCase(this string s, CultureInfo culture = null)
        {
            if (s == null) return null;
            if (s.Length == 0) return string.Empty;

            s = Regex.Replace(s, "([A-Z])([A-Z]+)($|[A-Z])", m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);

            var str = (culture != null
                ? char.ToLower(s[0], culture)
                : char.ToLowerInvariant(s[0])
                ).ToString();

            if (s.Length > 1) str += s.Substring(1);

            return str;
        }

        /// <summary>
        /// Determines whether this string and the specified content have the same value ignoring their case by using the <see cref="StringComparison.OrdinalIgnoreCase"/> comparer.
        /// </summary>
        /// <param name="value">The value to compare against the content.</param>
        /// <param name="content">The content to compare against this string value.</param>
        /// <returns>true if the value of the value parameter is the same as this string; otherwise, false.</returns>
        public static bool EqualNoCase(this string value, string content)
        {
            if (value != null)
            {
                return value.Equals(content, StringComparison.OrdinalIgnoreCase);
            }
            else if (content == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using a specified using the <see cref="StringComparer.OrdinalIgnoreCase"/> comparer.
        /// </summary>
        /// <param name="sequence">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
        public static bool ContainsNoCase(this string[] sequence, string value)
        {
            if (sequence != null)
            {
                return sequence.Contains(value, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes \t characters in the string and trim additional space and carriage returns.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Sanitize(this string text)
        {
            if (text == null)
            {
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder();
            var textArray = text.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in textArray.ToList())
            {
                sb.Append(item.Trim());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes all undesired characters in the given name and returns one that is safe and clean to use as a URL, and optionally converts it to lower case.
        /// </summary>
        /// <param name="name">The name to clean up.</param>
        /// <param name="toLower">true to convert the name to lower case; otherwise, leave it as is.</param>
        /// <returns></returns>
        public static string SanitizeName(this string name, bool toLower = true)
        {
            name = name.ReplaceWhiteSpace().RemoveAccents().Sanitize();
            string[] parts = name.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            name = string.Join("-", parts);
            if (toLower)
            {
                name = name.ToLower();
            }
            return name;
        }

        /// <summary>
        /// Parses the <paramref name="values"/> into a case-insensitive string dictionary.
        /// </summary>
        /// <param name="values">A vertical pipe-separated list of key/value pairs.</param>
        /// <returns></returns>
        public static IDictionary<string, string> ParseKeyValuePairs(this string values)
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(values))
            {
                var attrs = values.Split('|').Select(s =>
                {
                    var parts = s.Trim().Split('=');
                    
                    if (parts.Length == 2)
                        return new KeyValuePair<string, string>(parts[0], parts[1]);
                    else if (parts.Length == 1)
                        return new KeyValuePair<string, string>(parts[0], parts[0]);

                    throw new FormatException($"{nameof(values)} does not have the required key/value pairs format.");
                });

                foreach (var kvp in attrs)
                {
                    dic[kvp.Key] = kvp.Value;
                }
            }

            return dic;
        }
    }
}
