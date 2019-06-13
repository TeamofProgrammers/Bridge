using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToP.Bridge.Extensions
{
    public static class StringExtensions
    {
        public static string StripLineBreaks(this string str)
        {
            return str.Replace("\r\n", "").Replace("\n", " ");
        }

        public static string StripWhitespace(this string str)
        {
            return Regex.Replace(str, @"\s+", " ");
        }

        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            var place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            return source.Remove(place, find.Length).Insert(place, replace);
        }
        
        public static int OccurrenceCount(this string str, string find)
        {
            return str.Split(new string[] {find}, StringSplitOptions.None).Length - 1;
        }
    }
}