using System.Linq;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Diagnostics;

namespace BridgeMock_May2019
{
    class UserLink
    {
        public string BaseUserName { get; set; }
        public ulong DiscordUserId { get; set; }
        public string DiscordUserName { get; set; }
        public string IrcUid { get; set; }
        public string Suffix { get; private set; }
        public string IrcUserName
        {
            get
            {
                return GetIrcUserName(BaseUserName);
            }
        }
        private string GetIrcUserName(string userName)
        {
            return $"{userName}{Suffix}".Replace(' ', '_');
        }

        public UserLink(string suffix)
        {
            Suffix = suffix;
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

        public UserLink FindUserLink(string username)
        {
            if (!UserLinks.ContainsKey(username))
            {
                // I don't know if this will ever happen, but I would like to investigate what to do here if it ever does.
                throw new System.InvalidOperationException("Discord<->Irc UserLink not established for message sender");
            }
            var thisLink = UserLinks[username];
            return thisLink;
        }
        public Channel FindIrcChannelLink(string IrcChannel)
        {
            var link = Config.DiscordServer.ChannelMapping.Where(x => x.IRC.ToUpper() == IrcChannel.ToUpper()).FirstOrDefault();
            return link;
        }
        public string ParseIrcMessageForUsers(string message)
        {
            foreach (KeyValuePair<string, UserLink> user in this.UserLinks.ToList())
            {
                string highlightToken = $"<@{user.Value.DiscordUserId}>";
                message = message.Replace(user.Value.IrcUserName, highlightToken);
            }
            return message;
        }
        public void IrcChannelMessage(object s, IrcMessageEventArgs e)
        {
            //_EventLog("Channel Message Received");
            //_EventLog($"{e.ChannelMessage.User} {e.ChannelMessage.Channel} {e.ChannelMessage.Message}");
            var link = FindIrcChannelLink(e.ChannelMessage.Channel);
            if (null != link) {
                string parsedMessage = ParseIrcMessageForUsers(e.ChannelMessage.Message);
                string message = $"<{e.ChannelMessage.User}> {parsedMessage}";
                _ = DiscordLink.SendMessage(Config.DiscordServer.GuildId, link.Discord, message);
            }
        }
        /// <summary>
        /// Data Sanitization and String Parser
        /// Cleans up a discord message prior to being sent over the IRC link. 
        /// This can prevent an IRC injection attack... (Is that even a thing??)
        /// </summary>
        public string ParseDiscordMessage(string message, IReadOnlyCollection<SocketUser> users = null)
        {
            var parsed = message.Replace("\r\n","");
            parsed = parsed.Replace("\n", " ");
            char bold = (char)2;  // CloudyOne: Fix me please. You are my only hope. Convert **text** to bold stuff in irc.
            if(users.Count > 0)
            {
                foreach(var user in users)
                {
                    string replacement = $"<@{user.Id}>";
                    var mentionedLink = FindUserLink(user.Username);
                    parsed = parsed.Replace(replacement, mentionedLink.IrcUserName);
                }
            }
            // :shiftybit PRIVMSG #top :asdf test  normal text
            if (Config.IRCServer.SqueezeWhiteSpace)
                parsed = System.Text.RegularExpressions.Regex.Replace(parsed, @"\s+", " ");

            return parsed;
        }
        public void DiscordChannelMessage(object s, DiscordMessageEventArgs e)
        {
            var query = Config.DiscordServer.ChannelMapping.Where(x => x.Discord == e.Message.Channel.Id).FirstOrDefault();
            if (query != null)
            {
                var thisLink = FindUserLink(e.Message.Author.Username);
                string parsedMessage = ParseDiscordMessage(e.Message.Content, e.Message.MentionedUsers);

                int chunkSize = Config.IRCServer.MaxMessageSize;
                if (parsedMessage.Length <= chunkSize)
                {
                    IrcLink.SendMessage(thisLink.IrcUid, parsedMessage, query.IRC);
                }
                else
                {
                    for(int i = 0; i < parsedMessage.Length; i += Config.IRCServer.MaxMessageSize)
                    {
                        if (i + Config.IRCServer.MaxMessageSize > parsedMessage.Length) chunkSize = parsedMessage.Length - i;
                        IrcLink.SendMessage(thisLink.IrcUid, parsedMessage.Substring(i, chunkSize), query.IRC);
                    }
                }
            }
        }

        public void DiscordGuildConnected(object s, DiscordGuildConnectedEventArgs e)
        {
            if (e.Guild.Id == Config.DiscordServer.GuildId)
            { 
                foreach (var channel in e.Guild.Channels)
                {
                    var link = Config.DiscordServer.ChannelMapping.Where(x => x.Discord == channel.Id).FirstOrDefault();
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
                                thisLink = new UserLink(Config.IRCServer.NicknameSuffix);
                                thisLink.BaseUserName = user.Username;
                                thisLink.DiscordUserName = user.Username;
                                thisLink.DiscordUserId = user.Id;
                                thisLink.IrcUid = IrcLink.RegisterNick(thisLink.IrcUserName);
                                UserLinks.Add(user.Username, thisLink);
                            }
                            IrcLink.JoinChannel(thisLink.IrcUserName, link.IRC);                            
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
            if(status == UserStatus.AFK || status == UserStatus.Idle || status == UserStatus.Online) // TODO: XML Config
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
