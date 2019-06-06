using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace BridgeMock_May2019
{
    public class DiscordLinkConfig
    {
        public string Token { get; set; }
        public ulong GuildId { get; set; }
        public ChannelMapping ChannelMapping { get; set; }
    }
}
