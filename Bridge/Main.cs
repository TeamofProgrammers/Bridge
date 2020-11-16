using System;
using System.Linq;
using System.Threading.Tasks;
using ToP.Bridge.Helpers;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Services;

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
            }

            // Start the Async Processing
            if (initDiscord)
                DiscordLink.MainAsync().GetAwaiter();
            if (initIrc)
                IrcLink.StartBridge(); // Sort of Async.. Fix this later
        }

        public string UserStatus => $"Users Connected: IRC({IrcLink.UserCount})  Discord({DiscordLink.UserCount})";
        public string ChannelStatus => $"Channels Synced: {config.DiscordServer.ChannelMapping.Count}";

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
