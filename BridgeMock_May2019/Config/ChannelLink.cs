using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeMock_May2019
{
    public class ChannelMapping : List<Channel> { }
    public class Channel
    {
        /// <summary>
        /// DiscordChannelId
        /// </summary>
        public ulong Discord { get; set; }
        /// <summary>
        /// IrcChannelName
        /// </summary>
        public string IRC { get; set; }
    }
}
