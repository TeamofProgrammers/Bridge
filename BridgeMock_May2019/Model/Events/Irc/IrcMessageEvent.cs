namespace ToP.Bridge.Model.Events.Irc
{
    public class IrcMessageEvent
    {
        // Example:
        // :username PRIVMSG #channel :hello world
        public string User { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }

    }
}