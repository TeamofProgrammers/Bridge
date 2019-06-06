using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Model.Events;
using ToP.Bridge.Model.Events.Discord;

namespace ToP.Bridge.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private List<Channel> _channelLinks;
        static Action<string> EventLog;
        public event EventHandler<DiscordMessageEventArgs> OnChannelMessage;
        public event EventHandler<DiscordGuildConnectedEventArgs> OnGuildConnected;
        public event EventHandler<DiscordUserUpdatedEventArgs> OnUserUpdated;
        private DiscordLinkConfig Config;
        public DiscordService(Action<string> eventLog, DiscordLinkConfig config)
        {
            EventLog = eventLog;
            _channelLinks = new List<Channel>();
            _client = new DiscordSocketClient();
            _client.MessageReceived += MessageReceivedAsync;
            _client.GuildAvailable += GuildAvailableAsync;
            _client.GuildMemberUpdated += UserUpdatedAsync;
            this.Config = config;
        }

        private async Task UserUpdatedAsync(SocketGuildUser previous, SocketGuildUser current)
        {
            EventHandler<DiscordUserUpdatedEventArgs> handler = OnUserUpdated;
            handler?.Invoke(this, new DiscordUserUpdatedEventArgs(previous, current));
            EventLog($"{current.Username} Status Updated");
        }

        private async Task GuildAvailableAsync(SocketGuild guild)
        {
            EventLog("Guild has become available");
            EventHandler<DiscordGuildConnectedEventArgs> handler = OnGuildConnected;
            handler?.Invoke(this, new DiscordGuildConnectedEventArgs(guild));
        }
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            EventHandler<DiscordMessageEventArgs> handler = OnChannelMessage;
            handler?.Invoke(this, new DiscordMessageEventArgs(message));
        }

        public async Task SendMessage(ulong GuildId, ulong DiscordChannelId, string Message)
        {
            var guild = _client.GetGuild(GuildId);
            var channel = guild.GetTextChannel(DiscordChannelId);
            _ = channel.SendMessageAsync(Message);
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
