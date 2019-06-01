using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace BridgeMock_May2019
{
    public class DiscordLinkConfig
    {
        public string Token;
        public ulong GuildId;
        public List<ChannelLink> ChannelLinks;
        public bool ReadConfig()
        {
            ChannelLinks = new List<ChannelLink>();
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            DirectoryInfo path = new FileInfo(location.AbsolutePath).Directory;
            FileInfo[] config = path.GetFiles("config.xml");
            if (config.Length != 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(config[0].FullName);
                XmlNode disc = doc.DocumentElement.SelectSingleNode("DiscordServer");
                Token = disc["Token"].InnerText;
                GuildId = ulong.Parse(disc["GuildID"].InnerText);

                foreach (XmlNode node in disc["ChannelMapping"].ChildNodes)
                {
                    ChannelLink link = new ChannelLink();
                    link.DiscordChannelId = ulong.Parse(node["Discord"].InnerText);
                    link.IrcChannelName = node["IRC"].InnerText;
                    ChannelLinks.Add(link);
                }


                return true;
            }
            return false;
        }
    }
}
