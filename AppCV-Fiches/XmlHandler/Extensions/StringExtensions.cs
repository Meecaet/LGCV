using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToUpper(str[0]) + str.Substring(1).ToLower();
            }
            return str;
        }

        public static string Sanitize(this string str)
        {
            return str?.Trim()?.Replace("-", "").Replace(",","");
        }
    }
}
