namespace ToP.Bridge.Model.Events.Irc
{
    public class IrcMessageEvent
    {
        // Example:
        // :username PRIVMSG #channel :hello world
        public string SourceUser { get; set; }
        public string Destination { get; set; }
        public string Message { get; set; }
        public bool IsAction { get; set; }
        public bool IsPrivate { get; set; }
    }
}