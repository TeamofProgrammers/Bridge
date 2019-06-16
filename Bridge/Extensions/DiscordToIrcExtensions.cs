using System;
using System.Linq;
using ToP.Bridge.Helpers;

namespace ToP.Bridge.Extensions
{
    public static class DiscordToIrcExtensions
    {
        private static string _tempReplacement => "\u000b\u000b\u000b\u000b\u000b";
        private static string _tempReplacement2 => "\u000c\u000c\u000c\u000c\u000c";
        public static string DiscordToIrcBold(this string str, bool preserveLastOccurrence = true)
        {

            var value =  str.OccurrenceCount(DiscordMessageHelper.BoldControlCode) > 1 // Ensure there's a pair
                ? (preserveLastOccurrence ? str.ReplaceLastOccurrence(DiscordMessageHelper.BoldControlCode, _tempReplacement) : str).Replace(DiscordMessageHelper.BoldControlCode, IrcMessageHelper.BoldControlCode).Replace(_tempReplacement, DiscordMessageHelper.BoldControlCode)
                : str;
            return value;
        }

        public static string DiscordToIrcAction(this string str)
        {
            return str.ReplaceLastOccurrence(DiscordMessageHelper.ActionControl, "").ReplaceFirstOccurrence(DiscordMessageHelper.ActionControl,"");
        }

        public static string DiscordToIrcItalics(this string str)
        {
            if (str.OccurrenceCount(DiscordMessageHelper.ItalicsControlCode) < 2)
                return str; // Ensure there's a pair

            var discordBoldExists = str.OccurrenceCount(DiscordMessageHelper.BoldControlCode) > 0;

            return discordBoldExists
                ? // Ensures it doesn't matter what order we call bold and italics methods
                str.Replace(DiscordMessageHelper.BoldControlCode, _tempReplacement2)
                    .ReplaceLastOccurrence(DiscordMessageHelper.ItalicsControlCode, _tempReplacement)
                    .Replace(DiscordMessageHelper.ItalicsControlCode, IrcMessageHelper.ItalicsControlCode)
                    .Replace(_tempReplacement, DiscordMessageHelper.ItalicsControlCode)
                    .Replace(_tempReplacement2, DiscordMessageHelper.BoldControlCode)
                : str.Replace(DiscordMessageHelper.ItalicsControlCode, IrcMessageHelper.ItalicsControlCode);
        }

        public static string DiscordToIrcUnderline(this string str)
        {
            return str.OccurrenceCount(DiscordMessageHelper.UnderlineControlCode) > 1 // Ensure there's a pair
                ? str.ReplaceLastOccurrence(DiscordMessageHelper.UnderlineControlCode, _tempReplacement).Replace(DiscordMessageHelper.UnderlineControlCode, IrcMessageHelper.UnderlineControlCode).Replace(_tempReplacement, DiscordMessageHelper.UnderlineControlCode)
                : str;
        }

        public static string DiscordToIrcStrikeThrough(this string str)
        {
            return str.OccurrenceCount(DiscordMessageHelper.StrikeThroughControlCode) > 1 // Ensure there's a pair
                ? str.ReplaceLastOccurrence(DiscordMessageHelper.StrikeThroughControlCode, _tempReplacement).Replace(DiscordMessageHelper.StrikeThroughControlCode, IrcMessageHelper.StrikeThroughControlCode).Replace(_tempReplacement, DiscordMessageHelper.StrikeThroughControlCode)
                : str;
        }

    }
}