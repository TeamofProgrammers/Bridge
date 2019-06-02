using System.Linq;
using Discord;
using System.Collections.Generic;

namespace BridgeMock_May2019
{
    class UserLink
    {
        public string BaseUserName { get; set; }
        public ulong DiscordUserId { get; set; }
        public string DiscordUserName { get; set; }
        public string IrcUid { get; set; }
        public string IrcUserName
        {
            get
            {
                return GetIrcUserName(BaseUserName);
            }
        }
        public string GetIrcUserName(string userName)
        {
            return $"{userName}/discord".Replace(' ', '_');
        }
    }
    class Glue
    {
        private Dictionary<string, UserLink> UserLinks;
        private BridgeService IrcLink;
        private DiscordService DiscordLink;
        private BridgeConfig Config;
        public Glue(BridgeService IrcLink, DiscordService DiscordLink, BridgeConfig Config)
        {
            this.Config = Config;
            this.IrcLink = IrcLink;
            this.DiscordLink = DiscordLink;
            this.UserLinks = new Dictionary<string, UserLink>();
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
        /// <summary>
        /// Data Sanitization and String Parser
        /// Cleans up a discord message prior to being sent over the IRC link. 
        /// This can prevent an IRC injection attack... (Is that even a thing??)
        /// </summary>
        public string ParseDiscordMessage(string message)
        {
            var parsed = message.Replace("\r\n","");
            parsed = parsed.Replace("\n", " ");

            if (Config.Irc.SqueezeWhiteSpace)
                parsed = System.Text.RegularExpressions.Regex.Replace(parsed, @"\s+", " ");

            return parsed;
        }
        public void DiscordChannelMessage(object s, DiscordMessageEventArgs e)
        {
            var query = Config.Discord.ChannelLinks.Where(x => x.DiscordChannelId == e.Message.Channel.Id).FirstOrDefault();
            if (query != null)
            {
                if (!UserLinks.ContainsKey(e.Message.Author.Username))
                {
                    // I don't know if this will ever happen, but I would like to investigate what to do here if it ever does.
                    throw new System.InvalidOperationException("Discord<->Irc UserLink not established for message sender.");
                }
                var thisLink = UserLinks[e.Message.Author.Username];
                string parsedMessage = ParseDiscordMessage(e.Message.Content);

                int chunkSize = Config.Irc.MaxMessageSize;
                if (parsedMessage.Length <= chunkSize)
                {
                    IrcLink.SendMessage(thisLink.IrcUid, parsedMessage, query.IrcChannelName);
                }
                else
                {
                    for(int i = 0; i < parsedMessage.Length; i += Config.Irc.MaxMessageSize)
                    {

                        if (i + Config.Irc.MaxMessageSize > parsedMessage.Length) chunkSize = parsedMessage.Length - i;
                        IrcLink.SendMessage(thisLink.IrcUid, parsedMessage.Substring(i, chunkSize), query.IrcChannelName);
                    }
                }
            }
        }

        public void DiscordGuildConnected(object s, DiscordGuildConnectedEventArgs e)
        {
            if (e.Guild.Id == Config.Discord.GuildId)
            { 
                foreach (var channel in e.Guild.Channels)
                {
                    var link = Config.Discord.ChannelLinks.Where(x => x.DiscordChannelId == channel.Id).FirstOrDefault();
                    if (link != null)
                    {
                        var users = channel.Users;
                        foreach (var user in users)
                        {
                            UserLink thisLink;
                            if (UserLinks.ContainsKey(user.Username))
                            {
                                thisLink = UserLinks[user.Username];
                            }
                            else
                            {
                                thisLink = new UserLink();
                                thisLink.BaseUserName = user.Username;
                                thisLink.DiscordUserName = user.Username;
                                thisLink.DiscordUserId = user.Id;
                                thisLink.IrcUid = IrcLink.RegisterNick(thisLink.IrcUserName);
                                UserLinks.Add(user.Username, thisLink);
                            }
                            IrcLink.JoinChannel(thisLink.IrcUserName, link.IrcChannelName);                            
                        }
                    }
                }
                foreach(var user in e.Guild.Users)
                {
                    if (!DiscordUserConsideredOnline(user.Status) && UserLinks.ContainsKey(user.Username))
                    {
                        UserLink thisLink = UserLinks[user.Username];
                        IrcLink.SetAway(thisLink.IrcUid, true);
                    }
                }
            }
        }
        private bool DiscordUserConsideredOnline(UserStatus status)
        {
            if(status == UserStatus.AFK || status == UserStatus.Idle || status == UserStatus.Online)
            {
                return true;
            }
            return false;
        }
        public void DiscordUserUpdated(object s, DiscordUserUpdatedEventArgs e)
        {
            if(!DiscordUserConsideredOnline(e.Previous.Status) && DiscordUserConsideredOnline(e.Current.Status))
            {
                IrcLink.SetAway(UserLinks[e.Current.Username].IrcUid, false);
            }
            else if(DiscordUserConsideredOnline(e.Previous.Status) && !DiscordUserConsideredOnline(e.Current.Status))
            {
                IrcLink.SetAway(UserLinks[e.Current.Username].IrcUid, true);
            }
        }
    }
}
