namespace ToP.Bridge.Helpers
{
    public static class IrcMessageHelper
    {
        public static string ActionControlCode => ((char) 1).ToString();
        
        public static string BoldControlCode => ((char) 2).ToString();
        public static string ItalicsControlCode => ((char) 29).ToString();
        public static string StrikeThroughControlCode => ((char)30).ToString();
        public static string UnderlineControlCode => ((char) 31).ToString();
        

        public static string BoldText(string input) => $"{BoldControlCode}{input}{BoldControlCode}";
        public static string ItalicizeText(string input) => $"{ItalicsControlCode}{input}{ItalicsControlCode}";
    }
}
