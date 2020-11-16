using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ToP.Bridge.Helpers;
using ToP.Bridge.Model.Classes;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Model.Events.Irc;

namespace ToP.Bridge.Services
{
    public class IrcService
    {
        private static Action<string> IrcLog;

        private TcpClient tcpClient;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        private List<IrcUser> IrcUsers;
        private IrcLinkConfig Config;
        private static Random r = new Random();

        public event EventHandler<IrcMessageEventArgs> OnChannelMessage;
        public event EventHandler<IrcMessageEventArgs> OnPrivateMessage;
        public event EventHandler<EventArgs> OnServerDisconnect;

        public IrcService(Action<string> ircLog, IrcLinkConfig Config)
        {
            IrcLog = ircLog;
            IrcUsers = new List<IrcUser>();
            this.Config = Config;
        }

        public int UserCount
        {
            get
            {
                return IrcUsers.Select(x => x.UserName).Distinct().ToList().Count;
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[r.Next(s.Length)]).ToArray());
        }
        public string RegisterNick(string nick)
        {
            // example
            // :00B UID darkscrypt_0 0 0 darkscrypt_3583 discord 00B780369 0 +iw-x * BRIDGE * :darkscrypt
            // 

            //Format : Space seperated parameters
            // :ServerIdentifier  
            // UID    // UID Command https://www.unrealircd.org/docs/Server_protocol:UID_command
            // nickname
            // int Hop Count
            // int timestamp when user came online. (need to determine format of this, is it epoch?)
            // username *HACK*
            // hostname
            // userUID  // Currently 9 character random alphanumeric  *HACK*
            // services timestamp // im guessing this is when user gets +r from nickserv.
            // userModes //  "+iwx"
            // VHOST. * if blank
            // CloakedIP // right now i'm just sending a random string but it should look like 
            // Real IP address // This is the ip address, you can use *
            // : realname   // RealName appears after the colon, because this can have spaces in it.
            nick = nick.Trim();
            var user = IrcUsers.FirstOrDefault(n => n.Nick.ToLower().Equals(nick.ToLower()));
            if(user == null)
            {
                var hopcount = 0;
                var timestamp = 0;
                var username = nick + "_" + r.Next(1000, 9999).ToString(); // *HACK*
                var hostname = "discordBot";
                var uid = RandomString(9);
                var serviceStamp = 0;
                var userMode = "+iwx";
                var vhost = "*";
                var ipCloak = "bcloaked"; // bridge cloak
                var ipAddress = "*";

                Write($":{Config.ServerIdentifier} UID {nick} {hopcount} {timestamp} {username} {hostname} {uid} {serviceStamp} {userMode} {vhost} {ipCloak} {ipAddress} :{nick}");

                user = new IrcUser(uid, username, nick);
                IrcUsers.Add(user);
                
            }
            return user.UID;
            

        }
        public IrcUser GetIrcUser(string nick)
        {
            var query = IrcUsers.Where(n => n.Nick.ToLower().Equals(nick.ToLower())).ToList();
            return query.Count != 0 ? query.FirstOrDefault() : null;
        }
        public void DisconnectUser(string nick)
        {
            // Example
            //:002G7Y912 QUIT :Quit: Leaving
            IrcUser user = GetIrcUser(nick);
            if(null != user)
            {
                Write($":{user.UID} QUIT :Quit: Discord User Left");
                IrcUsers.Remove(user);
            }

        }
        public void ChangeNick(string oldNick, string nick)
        {
            var query = IrcUsers.Where(n => n.Nick.ToLower().Equals(oldNick.ToLower())).ToList();
            if(query.Count == 0)
            {
                IrcLog($"Error: {oldNick} is not a valid user");
                return;
            }
            var user = query.FirstOrDefault();
            if (user != null)
            {
                Write($":{user.UID} NICK {nick} 0");
                user.Nick = nick;
            }
        }

        public void JoinChannel(string nick, string channel)
        {
            Write($":{nick} JOIN {channel}");
        }

        public void PartChannel(string nick, string channel)
        {
            Write($":{nick} PART {channel}");
        }

        public void SendMessage(string nick, string message, string channel)
        {
            Write($":{nick} PRIVMSG {channel} :{message}", channel.Contains("#"));
        }
        public void SendAction(string nick, string action, string channel)
        {
            Write($":{nick} PRIVMSG {channel} :{IrcMessageHelper.ActionControlCode}ACTION {action}{IrcMessageHelper.ActionControlCode}", channel.Contains("#"));
        }
        public void SetAway(string nick, bool away)
        {
            Write(away ? $":{nick} AWAY discord user away" : $":{nick} AWAY");
            
        }
        private void Write(string line, bool console = true)
        {
            try
            {
                writer.WriteLine(line);
                writer.Flush();
                if (console)
                    IrcLog(line);
            } 
            catch (System.IO.IOException) {
                IrcLog($"Error: Disconnected from {Config.UplinkHost}");
                OnServerDisconnect?.Invoke(this, new EventArgs());
            }
            
        }
        
        private void ChannelMessageReceived(string input)
        {
            var tokens = input.Split(' ');
            var message = new IrcMessageEvent
            {
                SourceUser = tokens[0].Split(':')[1],
                IsPrivate = !tokens[2].StartsWith("#"),
                Destination = tokens[2],
                IsAction = tokens[3] == $":{IrcMessageHelper.ActionControlCode}ACTION"
            };
            var content = message.IsAction
                ? input.Replace($"{IrcMessageHelper.ActionControlCode}ACTION ", string.Empty).Replace($"{IrcMessageHelper.ActionControlCode}", string.Empty).Split(new[] {':'}, 3)[2]
                : input.Split(new[] {':'}, 3)[2];
            message.Message = content;
            if (!message.IsPrivate) IrcLog(input);
            (message.IsPrivate ? OnPrivateMessage : OnChannelMessage)?.Invoke(this, new IrcMessageEventArgs(message));
            
        }
        private void BridgeMain()
        {
            var input = string.Empty;
            try
            {
                while ((input = reader.ReadLine()) != null)
                {
                    //IrcLog(input);
                    var tokens = input.Split(' ');
                    if (tokens[0].ToUpper() == "PING")
                    {
                        Write("PONG " + tokens[1]);
                    }
                    else
                        switch (tokens[1].ToUpper())
                        {
                            case "PRIVMSG":
                                ChannelMessageReceived(input);
                                break;
                            default:
                                break;
                        }
                }
            }
            catch (Exception err)
            {
                
            }

            IrcLog($"BridgeMain() Terminated. Connection to {Config.UplinkHost} has been lost. ");
        }
        public void StartBridge()
        {
            CheckConnection();
            Write($"PASS {Config.UplinkPassword}", false);
            Write("PROTOCTL NICKv2 VHP NICKIP UMODE2 SJOIN SJOIN2 SJ3 NOQUIT TKLEXT ESVID MLOCK", false);
            Write($"PROTOCTL EAUTH={Config.ServerName}", false);
            Write($"PROTOCTL SID={Config.ServerIdentifier}", false);
            Write($"SERVER {Config.ServerName} 1 :{Config.ServerDescription}");
            Write($":{Config.ServerIdentifier} EOS", false);
            BridgeMain();
        }

        public void CheckConnection()
        {
            if (tcpClient == null)
            {
                tcpClient = new TcpClient(Config.UplinkHost, Config.UplinkPort);
                ns = tcpClient?.GetStream();
                reader = new StreamReader(ns);
                writer = new StreamWriter(ns);
            }
        }

        public async Task Disconnect()
        {
            await Task.Run(() =>
            {
                tcpClient?.Close();
                reader?.Close();
                writer?.Close();
                tcpClient = null;
            });
        }

    }
}
