using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeMock_May2019
{
    public class ChannelMessageEvent
    {
        // Example:
        // :shiftybit PRIVMSG #top :test
        public string User { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }

    }
    public class ChannelMessageEventArgs : EventArgs
    {
        private readonly ChannelMessageEvent _channelMessage;
        public ChannelMessageEventArgs(ChannelMessageEvent message)
        {
            _channelMessage = message;
        }

        public ChannelMessageEvent ChannelMessage {  get { return _channelMessage; } }
    }
}
