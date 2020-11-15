using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace ToP.Bridge.Model.Config
{
    public class DiscordLinkConfig
    {
        public string Token { get; set; }
        public ulong GuildId { get; set; }
        public ChannelMapping ChannelMapping { get; set; }
        public string OnlineStatuses { get; set; }
        public string IgnoredRoles { get; set; }

        public List<UserStatus> OnlineUserStatuses
        {
            get
            {
                try
                {
                    var statuses = OnlineStatuses?.Split(',').ToList();
                    var onlineStatuses = new List<UserStatus>();
                    if (statuses != null)
                    foreach (var status in statuses)
                    {
                        UserStatus userStatus;
                        var success = Enum.TryParse(status, out userStatus);
                        if (success)
                            onlineStatuses.Add(userStatus);
                    }

                    return onlineStatuses;
                }
                catch
                {
                    return new List<UserStatus>{ UserStatus.AFK, UserStatus.Idle, UserStatus.Online };
                }
            }
        }

        public List<string> IgnoredUserRoles
        {
            get
            {
                try
                {
                    return IgnoredRoles == null ? new List<string>() : IgnoredRoles.Split(',').ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
    }
}
