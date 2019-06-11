using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using ToP.Bridge.Helpers;
using ToP.Bridge.Model.Classes;
using ToP.Bridge.Model.Config;
using ToP.Bridge.Model.Events;
using ToP.Bridge.Model.Events.Irc;

namespace ToP.Bridge.Services
{
    public class IrcService
    {
        private static Action<string> InputLog;
        private static Action<string> OutputLog;
        private static Action<string> EventLog;

        private TcpClient tcpClient;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        private List<IrcUser> IrcUsers;
        private IrcLinkConfig Config;
        private static Random r = new Random();

        public event EventHandler<IrcMessageEventArgs> OnChannelMessage;
        public event EventHandler<IrcMessageEventArgs> OnPrivateMessage;

        public IrcService(Action<string> inputLog, Action<string> outputLog, Action<string> eventLog, IrcLinkConfig Config)
        {
            InputLog = inputLog;
            OutputLog = outputLog;
            EventLog = eventLog;
            IrcUsers = new List<IrcUser>();
            this.Config = Config;
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
            var query = IrcUsers.Where(n => n.Nick.ToLower().Equals(nick.ToLower())).ToList();
            if(query.Count == 0)
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

                var bridgeUser = new IrcUser {UID = uid, UserName = username, Nick = nick};
                IrcUsers.Add(bridgeUser);
                return uid;
            }
            else
            {
                EventLog($"Error: {nick} is already registered ");
                return null;
            }

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
                EventLog($"Error: {oldNick} is not a valid user");
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
        public void SendMessage(string nick, string message, string channel)
        {
            Write($":{nick} PRIVMSG {channel} :{message}");
        }
        public void SendAction(string nick, string action, string channel)
        {
            Write($":{nick} PRIVMSG {channel} :{IrcMessageHelper.ActionControlCode}ACTION {action}{IrcMessageHelper.ActionControlCode}");
        }
        public void SetAway(string nick, bool away)
        {
            Write(away ? $":{nick} AWAY discord user away" : $":{nick} AWAY");
        }
        private void Write(string line)
        {
            try
            {
                writer.WriteLine(line);
                writer.Flush();
                OutputLog(line);
            } 
            catch (System.IO.IOException) {
                EventLog($"Error: Disconnected from {Config.UplinkHost}");
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

            (message.IsPrivate ? OnPrivateMessage : OnChannelMessage)?.Invoke(this, new IrcMessageEventArgs(message));
            
        }
        private void BridgeMain()
        {
            var input = string.Empty;
            while ((input = reader.ReadLine()) != null)
            {
                InputLog(input);
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
            EventLog($"BridgeMain() Terminated. Connection to {Config.UplinkHost} has been lost. ");
        }
        public void StartBridge()
        {
            CheckConnection();
            Write($"PASS {Config.UplinkPassword}");
            Write("PROTOCTL NICKv2 VHP NICKIP UMODE2 SJOIN SJOIN2 SJ3 NOQUIT TKLEXT ESVID MLOCK");
            Write($"PROTOCTL EAUTH={Config.ServerName}");
            Write($"PROTOCTL SID={Config.ServerIdentifier}");
            Write($"SERVER {Config.ServerName} 1 :{Config.ServerDescription}");
            Write($":{Config.ServerIdentifier} EOS");
            BridgeMain();
        }

        public void CheckConnection()
        {
            if (tcpClient == null)
            {
                tcpClient = new TcpClient(Config.UplinkHost, Config.UplinkPort);
                ns = tcpClient.GetStream();
                reader = new StreamReader(ns);
                writer = new StreamWriter(ns);
            }
        }

    }
}
