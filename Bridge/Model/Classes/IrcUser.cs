namespace ToP.Bridge.Model.Classes
{
    public class IrcUser
    {
        public IrcUser(string uid, string nick, string username)
        {
            UID = uid;
            Nick = nick;
            UserName = username;
        }
        public string UID { get; set; }
        public string Nick { get; set; }
        public string UserName { get; set; }
    }
}