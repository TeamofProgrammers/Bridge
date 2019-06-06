namespace ToP.Bridge.Model.Classes
{
    public class UserLink
    {
        public string BaseUserName { get; set; }
        public ulong DiscordUserId { get; set; }
        public string DiscordUserName { get; set; }
        public string IrcUid { get; set; }
        public string Suffix { get; private set; }
        public string IrcUserName
        {
            get
            {
                return GetIrcUserName(BaseUserName);
            }
        }
        private string GetIrcUserName(string userName)
        {
            return $"{userName}{Suffix}".Replace(' ', '_');
        }

        public UserLink(string suffix)
        {
            Suffix = suffix;
        }
    }
}