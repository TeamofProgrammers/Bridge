using System;
using Discord.WebSocket;

namespace BridgeMock_May2019
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

    public class DiscordGuildConnectedEventArgs : EventArgs
    {
        private readonly SocketGuild guild;

        public DiscordGuildConnectedEventArgs(SocketGuild guild)
        {
            this.guild = guild;
        }
        public SocketGuild Guild { get { return guild; } }
    }

    public class DiscordUserUpdatedEventArgs : EventArgs
    {
        private readonly SocketGuildUser previous, current;

        public DiscordUserUpdatedEventArgs(SocketGuildUser previous, SocketGuildUser current)
        {
            this.previous = previous;
            this.current = current;
        }
        public SocketGuildUser Previous {  get { return previous; } }
        public SocketGuildUser Current {  get { return current; } }
    }
}
