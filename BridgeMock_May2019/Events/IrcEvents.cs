using System;

namespace BridgeMock_May2019
{
    public class IrcMessageEvent
    {
        // Example:
        // :shiftybit PRIVMSG #top :test
        public string User { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }

    }
    public class IrcMessageEventArgs : EventArgs
    {
        private readonly IrcMessageEvent _channelMessage;
        public IrcMessageEventArgs(IrcMessageEvent message)
        {
            _channelMessage = message;
        }

        public IrcMessageEvent ChannelMessage {  get { return _channelMessage; } }
    }
}
