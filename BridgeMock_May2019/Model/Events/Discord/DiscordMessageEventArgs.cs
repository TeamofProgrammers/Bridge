using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordMessageEventArgs : EventArgs
    {
        private readonly SocketMessage _channelMessage;
        public DiscordMessageEventArgs(SocketMessage message)
        {
            _channelMessage = message;
        }

        public SocketMessage Message {  get { return _channelMessage; } }
    }
}