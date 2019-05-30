using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;

namespace BridgeMock_May2019
{
    static class BridgeConfig
    {
        public static string UplinkHost { get; set; }
        public static int UplinkPort { get; set; }
        public static string UplinkPassword { get; set; }
        public static string ServerIdentifier { get; set; }
        public static string ServerName { get; set; }
        public static string ServerDescription { get; set; }

        public static bool ReadConfig()
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
                return true;
            }
            return false;
        }
    }
}
