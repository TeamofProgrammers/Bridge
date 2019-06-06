using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordGuildConnectedEventArgs : EventArgs
    {
        private readonly SocketGuild guild;

        public DiscordGuildConnectedEventArgs(SocketGuild guild)
        {
            this.guild = guild;
        }
        public SocketGuild Guild { get { return guild; } }
    }
}