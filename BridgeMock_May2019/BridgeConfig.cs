using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace BridgeMock_May2019
{
    public class BridgeConfig
    {
        public IrcLinkConfig IRCServer;
        public DiscordLinkConfig DiscordServer;        
    }

}
