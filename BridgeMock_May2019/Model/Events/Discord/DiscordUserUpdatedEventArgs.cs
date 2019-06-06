using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
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
