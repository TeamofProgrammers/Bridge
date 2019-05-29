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
    class ChannelLink
    {
        public ulong DiscordChannelId { get; set; }
        public string IrcChannelName { get; set; }
    }
    class DiscordService
    {
        private BridgeService _bridge;
        private readonly DiscordSocketClient _client;
        private string _Token;
        private ulong _GuildID;
        private List<ChannelLink> _ChannelLinks;
        public DiscordService(BridgeService bridge)
        {
            _ChannelLinks = new List<ChannelLink>();
            ReadConfig();
            _bridge = bridge;
            _client = new DiscordSocketClient();
            _client.MessageReceived += MessageReceivedAsync;
            _client.GuildAvailable += GuildAvailableAsync;
        }

        public async Task GuildAvailableAsync(SocketGuild guild)
        {
            if (guild.Id == _GuildID)
            {
                //var mchannel = guild.Channels.Where(x => x.Id == _Channel).First();
                foreach (var channel in guild.Channels)
                {
                    var link = _ChannelLinks.Where(x => x.DiscordChannelId == channel.Id).FirstOrDefault();
                    if (link != null)
                    {
                        var users = channel.Users;
                        foreach (var user in users)
                        {
                            _bridge.RegisterNick(user.Username);
                            _bridge.JoinChannel(user.Username, link.IrcChannelName);
                        }
                    }
                }
            }
        }
        public async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;
            var query = _ChannelLinks.Where(x => x.DiscordChannelId == message.Channel.Id).FirstOrDefault();
            if (query != null) 
                _bridge.SendMessage(message.Author.Username, message.Content, query.IrcChannelName);
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
                
                foreach(XmlNode node in disc["ChannelMapping"].ChildNodes)
                {
                    ChannelLink link = new ChannelLink();
                    link.DiscordChannelId = ulong.Parse(node["Discord"].InnerText);
                    link.IrcChannelName = node["IRC"].InnerText;
                    _ChannelLinks.Add(link);
                }


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
