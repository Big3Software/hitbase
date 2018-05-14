using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Big3.Hitbase.Miscellaneous
{
    public static class StringExtensions
    {
        public static string Left(this string text, int length)
        {
            if (text == null)
                return null;

            return (text.Length < length) ? text : text.Substring(0, length);
        }

        public static string Right(this string text, int length)
        {
            if (text == null)
                return null;

            return (text.Length < length) ? text : text.Substring(text.Length - length, length);
        }

        public static string Mid(this string text, int start, int end)
        {
            if (text == null)
                return null;

            return (text.Length < end + 1) ? text.Substring(start, text.Length - 1) : text.Substring(start, end);
        }

        public static string Mid(this string text, int start)
        {
            if (text == null)
                return null;

            return (text.Length < start + 1) ? String.Empty : text.Substring(start, text.Length - start);
        }

        public static bool IsNumeric(this string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!Char.IsNumber(text[i]))
                    return false;
            }

            return true;
        }
    }
}
