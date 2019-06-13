namespace ToP.Bridge.Model.Config
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
        /// Suffix added to the end of every nickname on IRC
        /// </summary>
        public string NicknameSuffix { get; set; }
        /// <summary>
        /// Largest message length supported by Irc Server.
        /// </summary>
        public int MaxMessageSize { get; set; } 
        /// <summary>
        /// Do we squeeze whitespace down prior to sending? (Condense multiple spaces to single space)
        /// </summary>
        public bool SqueezeWhiteSpace { get; set; } 
    }

}
