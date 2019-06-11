namespace ToP.Bridge.Helpers
{
    public static class DiscordMessageHelper
    {
        public static string ActionControl =>  "_";
        
        public static string BoldControlCode => "**";
        public static string ItalicsControlCode => "*";
        public static string StrikeThroughControlCode => "~~";
        public static string UnderlineControlCode => "__";

        public static string BoldText(string input) => $"{BoldControlCode}{input}{BoldControlCode}";
        public static string ItalicizeText(string input) => $"{ItalicsControlCode}{input}{ItalicsControlCode}";
        public static string UnderlineText(string input) => $"{UnderlineControlCode}{input}{UnderlineControlCode}";
    }
}