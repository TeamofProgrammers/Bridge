using System;
using System.Threading.Tasks;
using static Crayon.Output;

namespace ToP.Bridge.Arguments
{
    public class RehashArgument : IArgument
    {
        public async Task Process(Main process, Action<string> logger, string input)
        {
            logger($"Command not fully functional. {input.Split(' ')[0]}");
            foreach (var guild in process?.DiscordLink?.ActiveGuilds)
            {
                logger(Green($"Removing bridge spam from: {guild.Name}"));
                process?.DiscordLink?.Cleanup(guild);
            }

            //TODO: Setup rehash of config
        }
    }
}