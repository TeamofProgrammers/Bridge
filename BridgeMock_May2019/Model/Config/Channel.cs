namespace ToP.Bridge.Model.Config
{
    public class Channel
    {
        /// <summary>
        /// DiscordChannelId ie. 000000000000000000
        /// </summary>
        public ulong Discord { get; set; }
        /// <summary>
        /// IRC ChannelName ie. #channel
        /// </summary>
        public string IRC { get; set; }
    }
}
