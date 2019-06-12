using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using ToP.Bridge.Model.Events.Discord;

namespace ToP.Bridge.Extensions.Discord
{
    public static class DiscordUserExtensions
    {
        public static IEnumerable<SocketChannel> GetNewChannels(this SocketGuildUser user, IEnumerable<SocketChannel> previousChannels)
        {
            return user.GetChannels().ToList().Where(x => previousChannels.All(y => y.Id != x.Id));
        }

        public static IEnumerable<SocketChannel> GetRemovedChannels(this SocketGuildUser user, IEnumerable<SocketChannel> previousChannels)
        {
            var currentChannels = user.GetChannels().ToList();
            return previousChannels.Where(x => !currentChannels.Exists(y => y.Id == x.Id));
        }
        
        public static IEnumerable<SocketChannel> GetChannels(this SocketGuildUser user)
        {
            return user.Guild.TextChannels.Where(x => x.Users.Any(y => y.Id == user.Id));
        }

        public static IEnumerable<SocketRole> GetNewRoles(this DiscordUserUpdatedEventArgs args)
        {
            var previousRoles = args.Previous.Roles.ToList();
            return args.Current.Roles.Where(x => !previousRoles.Exists(y => y.Id == x.Id));
        }

        public static IEnumerable<SocketRole> GetRemovedRoles(this DiscordUserUpdatedEventArgs args)
        {
            var currentRoles = args.Current.Roles.ToList();
            return args.Previous.Roles.Where(x => !currentRoles.Exists(y => y.Id == x.Id));
        }
    }
}
