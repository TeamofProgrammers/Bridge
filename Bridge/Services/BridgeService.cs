using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ToP.Bridge.Extensions;
using ToP.Bridge.Extensions.Discord;
using ToP.Bridge.Helpers;
using ToP.Bridge.Model.Classes;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Model.Events;
using ToP.Bridge.Model.Events.Discord;
using ToP.Bridge.Model.Events.Irc;

namespace ToP.Bridge.Services
{
    public class BridgeService
    {
        private Dictionary<string, UserLink> UserLinks { get; set; }
        private IrcService IrcLink { get; set; }
        private DiscordService DiscordLink { get; set; }
        private BridgeConfig Config { get; set; }

        public BridgeService(IrcService ircLink, DiscordService discordLink, BridgeConfig config)
        {
            this.Config = config;
            this.IrcLink = ircLink;
            this.DiscordLink = discordLink;
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
            var link = Config.DiscordServer.ChannelMapping.FirstOrDefault(x => x.IRC.ToUpper() == IrcChannel.ToUpper());
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
            var link = FindIrcChannelLink(e.Message.Destination);
            if (null != link) {
                string parsedMessage = ParseIrcMessageForUsers(e.Message.Message);
                string message = e.Message.IsAction
                    ? $"_*{e.Message.SourceUser} {parsedMessage} *_"
                    : $"<{e.Message.SourceUser}> {parsedMessage}";
                _ = DiscordLink.SendMessage(Config.DiscordServer.GuildId, link.Discord, message.IrcToDiscordStrikeThrough().IrcToDiscordUnderline().IrcToDiscordItalics().IrcToDiscordBold());
            }
        }

        public void IrcPrivateMessage(object s, IrcMessageEventArgs e)
        {
            IrcLink.SendMessage(
                e.Message.Destination,
                $"{IrcMessageHelper.BoldText("Automated Message")}: {IrcMessageHelper.ItalicizeText("This user is a generated representation of a user connect to our Discord server. Private messages currently are no supported. Check back in the future!")}",
                e.Message.SourceUser);
        }

        /// <summary>
        /// Data Sanitization and String Parser
        /// Cleans up a discord message prior to being sent over the IRC link. 
        /// This can prevent an IRC injection attack... (Is that even a thing??)
        /// </summary>
        public string ParseDiscordMessage(string message, IReadOnlyCollection<SocketUser> users = null)
        {
            var parsed = message.StripLineBreaks();


            if(users?.Count > 0)
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
                parsed = parsed.StripWhitespace();

            return parsed.DiscordToIrcStrikeThrough().DiscordToIrcUnderline().DiscordToIrcBold().DiscordToIrcItalics();
        }

        

        public void DiscordChannelMessage(object s, DiscordMessageEventArgs e)
        {
            var query = Config.DiscordServer.ChannelMapping.FirstOrDefault(x => x.Discord == e.Message.Channel.Id);
            if (query != null)
            {
                var thisLink = FindUserLink(e.Message.Author.Username);
                var parsedMessage = ParseDiscordMessage(e.Message.Content, e.Message.MentionedUsers);
                var isAction = parsedMessage.StartsWith(DiscordMessageHelper.ActionControl) && parsedMessage.EndsWith(DiscordMessageHelper.ActionControl);
                var chunkSize = Config.IRCServer.MaxMessageSize;
                if (parsedMessage.Length <= chunkSize)
                {
                    if (isAction)
                        IrcLink.SendAction(thisLink.IrcUid, parsedMessage, query.IRC);
                    else
                        IrcLink.SendMessage(thisLink.IrcUid, parsedMessage, query.IRC);
                }
                else
                {
                    for(var i = 0; i < parsedMessage.Length; i += Config.IRCServer.MaxMessageSize)
                    {
                        if (i + Config.IRCServer.MaxMessageSize > parsedMessage.Length) chunkSize = parsedMessage.Length - i;
                        var iterationMessage = parsedMessage.Substring(i, chunkSize);

                        if (isAction)
                            IrcLink.SendAction(thisLink.IrcUid, iterationMessage, query.IRC);
                        else
                            IrcLink.SendMessage(thisLink.IrcUid, iterationMessage, query.IRC);
                    }
                }

                foreach (var attachment in e.Message.Attachments)
                {
                    IrcLink.SendMessage(thisLink.IrcUid, $"{attachment.Url}", query.IRC);
                }
            }
        }

        public void DiscordGuildConnected(object s, DiscordGuildConnectedEventArgs e)
        {
            if (e.Guild.Id == Config.DiscordServer.GuildId)
            { 
                foreach (var channel in e.Guild.Channels)
                {
                    var link = Config.DiscordServer.ChannelMapping.FirstOrDefault(x => x.Discord == channel.Id);
                    if (link != null)
                    {
                        var users = channel.Users;
                        foreach (var user in users.Where(x=> !x.Roles.Select(y=> y.Name).Intersect(Config.DiscordServer.IgnoredUserRoles).Any()))
                        {
                            JoinDiscordUserToIrcChannel(user, link);
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

        private void JoinDiscordUserToIrcChannel(SocketGuildUser user, Channel link)
        {
            UserLink thisLink;
            if (UserLinks.ContainsKey(user.Username))
            {
                thisLink = UserLinks[user.Username];
            }
            else
            {
                thisLink = new UserLink(Config.IRCServer.NicknameSuffix)
                {
                    BaseUserName = user.Username,
                    DiscordUserName = user.Username,
                    DiscordUserId = user.Id
                };
                thisLink.IrcUid = IrcLink.RegisterNick(thisLink.IrcUserName);
                UserLinks.Add(user.Username, thisLink);
            }

            IrcLink.JoinChannel(thisLink.IrcUserName, link.IRC);
        }

        private void PartDiscordUserFromIrcChannel(SocketGuildUser user, Channel link)
        {
            UserLink thisLink;
            if (UserLinks.ContainsKey(user.Username))
            {
                thisLink = UserLinks[user.Username];
                IrcLink.PartChannel(thisLink.IrcUserName, link.IRC);
                UserLinks.Remove(user.Username);
            }
        }

        private bool DiscordUserConsideredOnline(UserStatus status)
        {
            return Config.DiscordServer.OnlineUserStatuses.Contains(status);
        }

        public void DiscordUserUpdated(object s, DiscordUserUpdatedEventArgs e)
        {
            var bridgeUser = UserLinks[e.Current.Username];
            if (!DiscordUserConsideredOnline(e.Previous.Status) && DiscordUserConsideredOnline(e.Current.Status))
            {
                IrcLink.SetAway(bridgeUser.IrcUid, false);
            }
            else if(DiscordUserConsideredOnline(e.Previous.Status) && !DiscordUserConsideredOnline(e.Current.Status))
            {
                IrcLink.SetAway(bridgeUser.IrcUid, true);
            }

            // Join new channels
            foreach (var channel in e.NewChannels)
                IrcLink.JoinChannel(bridgeUser.IrcUserName,
                    Config.DiscordServer.ChannelMapping.FirstOrDefault(x => x.Discord == channel.Id)?.IRC);

            // Leave removed channels
            foreach (var channel in e.RemovedChannels)
                IrcLink.PartChannel(bridgeUser.IrcUserName,
                    Config.DiscordServer.ChannelMapping.FirstOrDefault(x => x.Discord == channel.Id)?.IRC);
        }

        public void DiscordUserJoined(object s, DiscordUserJoinLeaveEventArgs e)
        {
            var channels = e.GuildUser.Guild.Channels.Where(x => x.Users.Select(y => y.Id).Contains(e.GuildUser.Id))
                .Select(x => Config.DiscordServer.ChannelMapping.FirstOrDefault(y => y.Discord == x.Id));
            foreach (var channel in channels.Where(x=> x != null))
                JoinDiscordUserToIrcChannel(e.GuildUser, channel);
        }

        public void DiscordUserLeave(object s, DiscordUserJoinLeaveEventArgs e)
        {
            foreach (var channel in Config.DiscordServer.ChannelMapping)
                PartDiscordUserFromIrcChannel(e.GuildUser, channel);
        }

        public async void IrcServerDisconnect(object sender, EventArgs e)
        {
            foreach (var channel in Config.DiscordServer.ChannelMapping)
                await DiscordLink.SendMessage(Config.DiscordServer.GuildId, channel.Discord,
                    $"{DiscordMessageHelper.BoldControlCode}Bridge Down:{DiscordMessageHelper.BoldControlCode} Irc Connection Severed. Attempting to reconnect...");
        }

        public void UpdateIrcLink(IrcService ircLink)
        {
            this.IrcLink = ircLink;
        }
    }
}
