using System;
using System.Linq;
using System.Threading.Tasks;
using ToP.Bridge.Helpers;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Services;
using static Crayon.Output;

namespace ToP.Bridge
{
    public partial class Main
    {
        public IrcService IrcLink { get; set; }
        public DiscordService DiscordLink { get; set; }
        private BridgeService glue { get; set; }
        private static Action<string> Logger;
        private BridgeConfig config;


        public Main(Action<Main> context, Action<string> logger)
        {
            context(this);
            Logger = logger;
        }

        public async Task InitializeBridge(bool initIrc = true, bool initDiscord = true)
        {
            config = ConfigHelper.LoadConfig();
            if (initIrc)
                IrcLink = new IrcService(Logger, config.IRCServer);
            if (initDiscord)
                DiscordLink = new DiscordService(Logger, config.DiscordServer);
            glue = new BridgeService(IrcLink, DiscordLink, config);

            if (initDiscord)
            {
                DiscordLink.OnChannelMessage += glue.DiscordChannelMessage;
                DiscordLink.OnGuildConnected += glue.DiscordGuildConnected;
                DiscordLink.OnUserUpdated += glue.DiscordUserUpdated;
                DiscordLink.OnUserJoin += glue.DiscordUserJoined;
                DiscordLink.OnUserLeave += glue.DiscordUserLeave;
            }

            if (initIrc)
            {
                IrcLink.OnChannelMessage += glue.IrcChannelMessage;
                IrcLink.OnPrivateMessage += glue.IrcPrivateMessage;
                IrcLink.OnServerDisconnect += glue.IrcServerDisconnect;
                IrcLink.OnServerConnect += glue.IrcServerConnect;
            }

            // Start the Async Processing
            if (initDiscord)
                DiscordLink.MainAsync().GetAwaiter();
            if (initIrc)
                IrcLink.StartBridge(); // Sort of Async.. Fix this later
        }

        public string UserStatus => $"\n{Red("Users Connected:")}\n\t\tIRC({IrcLink.UserCount})\n\t\tDiscord({DiscordLink.UserCount})";
        public string ChannelStatus => $"{Red("Channels Synced:")} { string.Join("",config.DiscordServer.ChannelMapping.Select(x=> $"\n\t\t{Blue(x.IRC)} <--> {Green(x.Discord.ToString())} - StatusChannel: {x.StatusChannel}").ToArray())}";

        public async Task Disconnect()
        {
            await IrcLink?.Disconnect();
            await DiscordLink?.Disconnect();
        }

        public async Task Connect()
        {
            await InitializeBridge();
        }

    }
}
