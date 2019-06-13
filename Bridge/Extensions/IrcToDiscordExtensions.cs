using System;
using ToP.Bridge.Helpers;

namespace ToP.Bridge.Extensions
{
    public static class IrcToDiscordExtensions
    {

        public static string IrcToDiscordBold(this string str, bool keepAllOccurrences = false)
        {
            return str.Replace(IrcMessageHelper.BoldControlCode, DiscordMessageHelper.BoldControlCode, keepAllOccurrences);
        }

        public static string IrcToDiscordItalics(this string str, bool keepAllOccurrences = false)
        {
            return str.Replace(IrcMessageHelper.ItalicsControlCode, DiscordMessageHelper.ItalicsControlCode, keepAllOccurrences);
        }

        public static string IrcToDiscordUnderline(this string str, bool keepAllOccurrences = false)
        {
            return str.Replace(IrcMessageHelper.UnderlineControlCode, DiscordMessageHelper.UnderlineControlCode, keepAllOccurrences);
        }

        public static string IrcToDiscordStrikeThrough(this string str, bool keepAllOccurrences = false)
        {
            return str.Replace(IrcMessageHelper.StrikeThroughControlCode, DiscordMessageHelper.StrikeThroughControlCode, keepAllOccurrences);
        }

        private static string Replace(this string str, string from, string to, bool keepAllOccurrences)
        {
            return str.OccurrenceCount(from).IsEven() || keepAllOccurrences
                ? str.Replace(from, to)
                : str.ReplaceLastOccurrence(from, string.Empty).Replace(from, to);
        }
    }
}