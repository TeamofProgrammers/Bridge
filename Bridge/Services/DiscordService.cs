using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ToP.Bridge.Extensions.Discord;
using ToP.Bridge.Model.Config;
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
        public event EventHandler<DiscordUserJoinLeaveEventArgs> OnUserLeave;
        public event EventHandler<DiscordUserJoinLeaveEventArgs> OnUserJoin;
        public Dictionary<ulong, List<SocketChannel>> UserChannels { get; set; }

        public List<SocketGuild> ActiveGuilds => _client?.Guilds.ToList();
        private DiscordLinkConfig Config;
        public DiscordService(Action<string> eventLog, DiscordLinkConfig config)
        {
            UserChannels = new Dictionary<ulong, List<SocketChannel>>();
            EventLog = eventLog;
            _channelLinks = new List<Channel>();
            _client = new DiscordSocketClient();
            _client.MessageReceived += MessageReceivedAsync;
            _client.GuildAvailable += GuildAvailableAsync;
            _client.GuildMemberUpdated += UserUpdatedAsync;
            _client.ChannelUpdated += ChannelUpdatedAsync;
            _client.UserLeft += UserLeftGuild;
            _client.UserJoined += UserJoinedGuild;
            _client.LoggedOut += LoggedOutAsync;
            this.Config = config;
        }


        public int UserCount
        {
            get
            {
                return UserChannels.SelectMany(x => x.Value.SelectMany(y=> y.Users.SelectMany(z=> z.Username))).Distinct().ToList().Count;
            }
        }

        private async Task LoggedOutAsync()
        {
            await Task.Delay(5000);
            await MainAsync();
        }

        private async Task ChannelUpdatedAsync(SocketChannel previous, SocketChannel current)
        {
            
        }

        private async Task UserJoinedGuild(SocketGuildUser user)
        {
            if (user.Guild.Id == Config.GuildId)
            {
                AddUserChannels(user);
                OnUserJoin?.Invoke(this, new DiscordUserJoinLeaveEventArgs(user));
                EventLog($"{user.Username} has joined the guild");
            }
        }
        private async Task UserLeftGuild(SocketGuildUser user)
        {
            if (user.Guild.Id == Config.GuildId)
            {
                OnUserLeave?.Invoke(this, new DiscordUserJoinLeaveEventArgs(user));
                EventLog($"{user.Username} has left guild");
            }
        }
        private async Task UserUpdatedAsync(SocketGuildUser previousUser, SocketGuildUser currentUser)
        {
            if (previousUser.Guild.Id == Config.GuildId)
            {
                var channels = UserChannels[currentUser.Id].ToList();
                var removedChannels = currentUser.GetRemovedChannels(channels).ToList();
                var newChannels = currentUser.GetNewChannels(channels).ToList();
                var args = new DiscordUserUpdatedEventArgs(previousUser, currentUser, newChannels, removedChannels);

                OnUserUpdated?.Invoke(this, args);

                if (args.NewChannels.Any() || args.RemovedChannels.Any())
                    AddUserChannels(currentUser);

                EventLog($"{currentUser.Username} Status Updated: {currentUser.Status}");
            }
        }

        private async Task GuildAvailableAsync(SocketGuild guild)
        {
            if (guild.Id == Config.GuildId)
            {
                EventLog("Guild has become available");
                foreach (var socketGuildUser in guild.Users)
                {
                    AddUserChannels(socketGuildUser);
                }
                
                OnGuildConnected?.Invoke(this, new DiscordGuildConnectedEventArgs(guild));
                await Cleanup(guild);
            }
        }

        private void AddUserChannels(SocketGuildUser socketGuildUser)
        {
            if (socketGuildUser.Guild.Id == Config.GuildId)
            {
                if (UserChannels.Keys.Any(x => x == socketGuildUser.Id))
                {
                    var channels = UserChannels[socketGuildUser.Id].ToList();
                    var removedChannels = socketGuildUser.GetRemovedChannels(channels).ToList();
                    var newChannels = socketGuildUser.GetNewChannels(channels).ToList();
                    UserChannels[socketGuildUser.Id].AddRange(newChannels);
                    UserChannels[socketGuildUser.Id].RemoveAll(x=> removedChannels.Any(y=> y.Id == x.Id));
                }
                else
                    UserChannels.Add(socketGuildUser.Id, socketGuildUser.GetChannels().ToList());
            }
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

        public async Task Cleanup(SocketGuild guild)
        {
            // Cleanup Disconnect Messages
            foreach (var chanId in Config.ChannelMapping.Select(x => x.Discord))
            {
                var expireDate = DateTime.Today.AddDays(-13);
                var chan = guild.TextChannels.FirstOrDefault(x => x.Id == chanId);
                var messages = await _client.GetGuild(guild.Id).GetTextChannel(chanId).GetMessagesAsync(100).FlattenAsync();
                // TODO: Standardize disconnect message so it can be referenced for identification
                foreach (var deleteMessage in messages.Where(x => x.Content.Contains("Irc Connection Severed") && x.CreatedAt >= expireDate))
                { 
                    await chan.DeleteMessageAsync(deleteMessage);
                    await Task.Delay(1000);
                }
            }
        }

        public async Task Disconnect()
        {
            await _client.StopAsync();
        }
    }
}
