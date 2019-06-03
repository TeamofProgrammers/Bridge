using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace BridgeMock_May2019
{
    public class IrcLinkConfig
    {
        public string UplinkHost { get; set; }
        public int UplinkPort { get; set; }
        public string UplinkPassword { get; set; }
        public string ServerIdentifier { get; set; }
        public string ServerName { get; set; }
        public string ServerDescription { get; set; }
        /// <summary>
        /// Largest message length supported by Irc Server.
        /// </summary>
        public int MaxMessageSize { get; set; } // TODO: Add to config.xml
        /// <summary>
        /// Do we squeeze whitespace down prior to sending? (Condense multiple spaces to single space)
        /// </summary>
        public bool SqueezeWhiteSpace { get; set; } // TODO: Add to config.xml
        public bool ReadConfig()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            DirectoryInfo path = new FileInfo(location.AbsolutePath).Directory;
            FileInfo[] config = path.GetFiles("config.xml");
            if (config.Length != 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(config[0].FullName);
                XmlNode irc = doc.DocumentElement.SelectSingleNode("IRCServer");
                UplinkHost = irc["UplinkHost"].InnerText;
                UplinkPort = int.Parse(irc["UplinkPort"].InnerText);
                UplinkPassword = irc["UplinkPassword"].InnerText;
                ServerIdentifier = irc["ServerIdentifier"].InnerText;
                ServerName = irc["ServerName"].InnerText;
                ServerDescription = irc["ServerDescription"].InnerText;
                MaxMessageSize = 350; //TODO: Config.Xml
                SqueezeWhiteSpace = true;  //TODO: Config.xml
                return true;
            }
            return false;
        }
    }

}
