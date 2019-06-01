using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace BridgeMock_May2019
{
    class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private List<ChannelLink> _ChannelLinks;
        static Action<string> _EventLog;
        public event EventHandler<DiscordMessageEventArgs> OnChannelMessage;
        public event EventHandler<DiscordGuildConnectedEventArgs> OnGuildConnected;
        private DiscordLinkConfig Config;
        public DiscordService(Action<string> EventLog, DiscordLinkConfig Config)
        {
            _EventLog = EventLog;
            _ChannelLinks = new List<ChannelLink>();
            _client = new DiscordSocketClient();
            _client.MessageReceived += MessageReceivedAsync;
            _client.GuildAvailable += GuildAvailableAsync;
            this.Config = Config;
        }

        private async Task GuildAvailableAsync(SocketGuild guild)
        {
            _EventLog("Guild has become available");
            EventHandler<DiscordGuildConnectedEventArgs> handler = OnGuildConnected;
            if (null != handler)
            {
                handler(this, new DiscordGuildConnectedEventArgs(guild));
            }
        }
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            EventHandler<DiscordMessageEventArgs> handler = OnChannelMessage;
            if (null != handler)
            {
                handler(this, new DiscordMessageEventArgs(message));
            }
        }

        public async Task SendMessage(ulong GuildId, ulong DiscordChannelId, string Message)
        {
            var guild = _client.GetGuild(GuildId);
            var channel = guild.GetTextChannel(DiscordChannelId);
            channel.SendMessageAsync(Message);
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
