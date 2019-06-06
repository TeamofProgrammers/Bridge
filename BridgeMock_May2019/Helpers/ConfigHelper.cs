using System.IO;
using System.Xml.Serialization;
using ToP.Bridge.Model.Config;

namespace ToP.Bridge.Helpers
{
    public static class ConfigHelper
    {
        public static BridgeConfig LoadConfig(string location = "Config/config.xml")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BridgeConfig));
            object obj = null;

            using (TextReader reader = new StringReader(File.ReadAllText(location)))
            {
                obj = serializer.Deserialize(reader);
            }
            return (BridgeConfig)obj;
        }
    }
}
