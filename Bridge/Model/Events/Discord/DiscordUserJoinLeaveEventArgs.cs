using System;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Events.Discord
{
    public class DiscordUserJoinLeaveEventArgs : EventArgs
    {
        public DiscordUserJoinLeaveEventArgs(SocketGuildUser guildUser)
        {
            this.GuildUser = guildUser;
        }
        public SocketGuildUser GuildUser { get; }
    }
}