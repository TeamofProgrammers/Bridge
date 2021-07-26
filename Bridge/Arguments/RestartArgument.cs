using System;
using System.Threading.Tasks;
using static Crayon.Output;

namespace ToP.Bridge.Arguments
{
    public class RestartArgument : IArgument
    {
        public async Task Process(Main process, Action<string> logger, string input)
        {
            if (input.Split(' ').Length < 2)
            {
                logger("Restarting connections. This may take a few seconds..");
                await process?.Disconnect();
                await Task.Delay(5000);
                await process?.Connect();
            }
            else
            {
                switch (input.Split(' ')[1])
                {
                    case "irc":
                        logger("Restarting Irc Link. This may take a few seconds..");
                        await process?.IrcLink.Disconnect();
                        await Task.Delay(5000);
                        await process?.InitializeBridge();
                        break;
                    case "discord":
                        //logger("Feature unavailable.");
                        // TODO: Reconnect discord without running into nick collision on IRC side
                        await process?.DiscordLink.Disconnect();
                        await Task.Delay(5000);
                        await process?.InitializeBridge(initIrc: false);
                        break;
                    case "help":
                    default:
                        logger(Red(Bold("Syntax: restart <service>")));
                        logger("Available options:");
                        logger("\t\trestart discord");
                        logger("\t\trestart irc");
                        break;
                }
            }
        }
    }
}
