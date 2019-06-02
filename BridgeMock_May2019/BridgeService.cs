using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;


namespace BridgeMock_May2019
{
    class IrcUser
    {
        public string UID { get; set; }
        public string Nick { get; set; }
        public string UserName { get; set; }
    }
    class BridgeService
    {
        private TcpClient tcpClient;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        static Action<string> InputLog;
        static Action<string> OutputLog;
        static Action<string> EventLog;
        private List<IrcUser> IrcUsers;
        private IrcLinkConfig Config;
        public event EventHandler<IrcMessageEventArgs> OnChannelMessage;
        private static Random r = new Random();
        public BridgeService(Action<string> inputLog, Action<string> outputLog, Action<string> eventLog, IrcLinkConfig Config)
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
            var query = IrcUsers.Where(n => n.Nick.ToLower().Equals(nick.ToLower()));
            if(query.ToList().Count == 0)
            {
                int hopcount = 0;
                int timestamp = 0;
                string username = nick + "_" + r.Next(1000, 9999).ToString(); // *HACK*
                string hostname = "discordBot";
                string uid = RandomString(9); // *MAJOR HACK* Need to track this
                int serviceStamp = 0;
                string userMode = "+iwx";
                string vhost = "*";
                string ipCloak = "bcloaked"; // bridge cloak
                string ipAddress = "*";
                string mstr = $":{Config.ServerIdentifier} UID {nick} {hopcount} {timestamp} {username} {hostname} {uid} {serviceStamp} {userMode} {vhost} {ipCloak} {ipAddress} :{nick}";

                write(mstr);
                var bridgeUser = new IrcUser();
                bridgeUser.UID = uid;
                bridgeUser.UserName = username;
                bridgeUser.Nick = nick;
                IrcUsers.Add(bridgeUser);
                return uid;
            }
            else
            {
                EventLog($"Error: {nick} is already registered ");
                return null;
            }

        }
        public void ChangeNick(string oldNick, string nick)
        {
            // Example
            //:00257TR0R NICK bitshift 1559074834
            //:00257TR0R NICK shiftybit 1559074836

            var query = IrcUsers.Where(n => n.Nick.ToLower().Equals(oldNick.ToLower()));
            if(query.ToList().Count == 0)
            {
                EventLog($"Error: {oldNick} is not a valid user");
                return;
            }
            IrcUser user = query.First();
            string mstr = $":{user.UID} NICK {nick} 0";
            write(mstr);
            user.Nick = nick;

        }
        public void JoinChannel(string nick, string channel)
        {
            // example
            // :darkscrypt JOIN #topdev
            string mstr = $":{nick} JOIN {channel}";
            write(mstr);
        }
        public void SendMessage(string nick, string message, string channel)
        {
            // example
            // :darkscrypt PRIVMSG #top :Hello World!
            string mstr = $":{nick} PRIVMSG {channel} :{message}";
            write(mstr);
        }
        public void SendAction(string nick, string action, string channel)
        {
            // example  :shiftybit PRIVMSG #TOP :ACTION flips a table
            // string mstr = $":{nick} PRIVMSG {channel} :ACTION {action}";
            char c1 = (char)1; // control char 1
            string mstr = $":{nick} PRIVMSG {channel} :{c1}ACTION {action}{c1}"; 
            write(mstr);
        }
        public void SetAway(string nick, bool away)
        {
            string mstr;
            if (away)
            {
                mstr = $":{nick} AWAY discord user away";
            }
            else
            {
                mstr = $":{nick} AWAY";
            }
            write(mstr);
        }
        private void write(string line)
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
            string[] tokens = input.Split(' ');
            EventHandler<IrcMessageEventArgs> handler = OnChannelMessage;
            if (null != handler)
            {
                IrcMessageEvent channelMessage = new IrcMessageEvent();
                channelMessage.User = tokens[0].Split(':')[1];
                channelMessage.Channel = tokens[2];
                string content = input.Split(new[] { ':' },3)[2];
                channelMessage.Message = content;
                handler(this, new IrcMessageEventArgs(channelMessage));
            }
        }
        private void BridgeMain()
        {
            string input;
            while ((input = reader.ReadLine()) != null)
            {
                InputLog(input);
                string[] tokens = input.Split(' ');
                if (tokens[0].ToUpper() == "PING")
                {
                    write("PONG " + tokens[1]);
                }
                switch (tokens[1].ToUpper())
                {
                    case "PRIVMSG":
                        // Example: 
                        // :shiftybit PRIVMSG #top :test
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
            tcpClient = new TcpClient(Config.UplinkHost,Config.UplinkPort);
            ns = tcpClient.GetStream();
            reader = new StreamReader(ns);
            writer = new StreamWriter(ns);
            write($"PASS {Config.UplinkPassword}");
            write("PROTOCTL NICKv2 VHP NICKIP UMODE2 SJOIN SJOIN2 SJ3 NOQUIT TKLEXT ESVID MLOCK");
            write($"PROTOCTL EAUTH={Config.ServerName}");
            write($"PROTOCTL SID={Config.ServerIdentifier}");
            write($"SERVER {Config.ServerName} 1 :{Config.ServerDescription}");
            write($":{Config.ServerIdentifier} EOS");
            BridgeMain();
        }
    }
}
