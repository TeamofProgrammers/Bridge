using System.Linq;

namespace BridgeMock_May2019
{
    class Glue
    {
        private BridgeService IrcLink;
        private DiscordService DiscordLink;
        private BridgeConfig Config;
        public Glue(BridgeService IrcLink, DiscordService DiscordLink, BridgeConfig Config)
        {
            this.Config = Config;
            this.IrcLink = IrcLink;
            this.DiscordLink = DiscordLink;
        }
        public void IrcChannelMessage(object s, IrcMessageEventArgs e)
        {
            //_EventLog("Channel Message Received");
            //_EventLog($"{e.ChannelMessage.User} {e.ChannelMessage.Channel} {e.ChannelMessage.Message}");
            var link = Config.Discord.ChannelLinks
                .Where(x => x.IrcChannelName.ToUpper() == e.ChannelMessage.Channel.ToUpper())
                .FirstOrDefault();
            if (null != link) { 
                string message = $"<{e.ChannelMessage.User}> {e.ChannelMessage.Message}";
                _ = DiscordLink.SendMessage(Config.Discord.GuildId, link.DiscordChannelId, message);
            }
        }
        public void DiscordChannelMessage(object s, DiscordMessageEventArgs e)
        {
            var query = Config.Discord.ChannelLinks.Where(x => x.DiscordChannelId == e.Message.Channel.Id).FirstOrDefault();
            if (query != null)
            {
                IrcLink.SendMessage(e.Message.Author.Username, e.Message.Content, query.IrcChannelName);
            }
        }

        public void DiscordGuildConnected(object s, DiscordGuildConnectedEventArgs e)
        {
            if (e.Guild.Id == Config.Discord.GuildId)
            {
                //var mchannel = guild.Channels.Where(x => x.Id == _Channel).First();
                foreach (var channel in e.Guild.Channels)
                {
                    var link = Config.Discord.ChannelLinks.Where(x => x.DiscordChannelId == channel.Id).FirstOrDefault();
                    if (link != null)
                    {
                        var users = channel.Users;
                        foreach (var user in users)
                        {
                            IrcLink.RegisterNick(user.Username);
                            IrcLink.JoinChannel(user.Username, link.IrcChannelName);
                        }
                    }
                }
            }
        }
    }
}
