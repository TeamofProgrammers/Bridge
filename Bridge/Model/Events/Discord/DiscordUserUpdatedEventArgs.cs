using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordUserUpdatedEventArgs : EventArgs
    {
        public DiscordUserUpdatedEventArgs(SocketGuildUser previous, SocketGuildUser current, List<SocketChannel> newChannels, List<SocketChannel> removedChannels)
        {
            this.Previous = previous;
            this.Current = current;
            NewChannels = newChannels;
            RemovedChannels = removedChannels;
        }
        public SocketGuildUser Previous { get; }
        public SocketGuildUser Current { get; }
        public List<SocketChannel> NewChannels { get; }
        public List<SocketChannel> RemovedChannels { get; }
    }
}
