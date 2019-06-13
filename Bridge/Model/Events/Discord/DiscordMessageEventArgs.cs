using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordMessageEventArgs : EventArgs
    {
        public DiscordMessageEventArgs(SocketMessage message)
        {
            Message = message;
        }

        public SocketMessage Message { get; }
    }
}