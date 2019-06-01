using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;

namespace BridgeMock_May2019
{
    public class BridgeConfig
    {
        public IrcLinkConfig Irc;
        public DiscordLinkConfig Discord;

        public BridgeConfig()
        {
            Irc = new IrcLinkConfig();
            Discord = new DiscordLinkConfig();
            Irc.ReadConfig();
            Discord.ReadConfig();
        }
    }

}
