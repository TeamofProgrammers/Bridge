using System;

namespace ToP.Bridge.Model.Events.Irc
{
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
