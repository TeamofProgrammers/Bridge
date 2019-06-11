using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordUserUpdatedEventArgs : EventArgs
    {
        public DiscordUserUpdatedEventArgs(SocketGuildUser previous, SocketGuildUser current)
        {
            this.Previous = previous;
            this.Current = current;
        }
        public SocketGuildUser Previous { get; }
        public SocketGuildUser Current { get; }
    }
}
