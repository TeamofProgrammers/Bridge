using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordGuildConnectedEventArgs : EventArgs
    {
        public DiscordGuildConnectedEventArgs(SocketGuild guild)
        {
            this.Guild = guild;
        }
        public SocketGuild Guild { get; }
    }
}