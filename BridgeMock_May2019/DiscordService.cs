using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Xml;
using System.Reflection;

namespace BridgeMock_May2019
{
    class DiscordService
    {
        private BridgeService _bridge;
        private readonly DiscordSocketClient _client;
        private string _Token;
        private ulong _GuildID;
        private ulong _Channel;
        public DiscordService(BridgeService bridge)
        {
            ReadConfig();
            _bridge = bridge;
            _client = new DiscordSocketClient();
            _client.MessageReceived += MessageReceivedAsync;
            //_MainGuild = _client.Guilds.Where(x => x.Id == _GuildID).First();
            _client.GuildAvailable += GuildAvailableAsync;
        }

        public async Task GuildAvailableAsync(SocketGuild guild)
        {
            if (guild.Id == _GuildID)
            {
                var mchannel = guild.Channels.Where(x => x.Id == _Channel).First();
                var users = mchannel.Users;
                foreach(var user in users)
                {
                    _bridge.RegisterNick(user.Username);
                    _bridge.JoinChannel(user.Username, "#bot-test");
                }
            }
        }
        public async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Channel.Name.ToLower() == "bot-test")
                _bridge.SendMessage(message.Author.Username, message.Content, "#bot-test");
        }

        public bool ReadConfig()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            DirectoryInfo path = new FileInfo(location.AbsolutePath).Directory;
            FileInfo[] config = path.GetFiles("config.xml");
            if (config.Length != 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(config[0].FullName);
                XmlNode disc = doc.DocumentElement.SelectSingleNode("DiscordServer");
                _Token = disc["Token"].InnerText;
                _GuildID = ulong.Parse(disc["GuildID"].InnerText);
                _Channel = 388908063014387714;
                return true;
            }
            return false;
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
