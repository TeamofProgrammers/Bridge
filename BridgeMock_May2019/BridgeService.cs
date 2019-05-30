using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;


namespace BridgeMock_May2019
{
    class BridgeUser
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
        private List<BridgeUser> _BridgeUsers;
        public event EventHandler<ChannelMessageEventArgs> OnChannelMessage;
        private static Random r = new Random();
        public BridgeService(Action<string> inputLog, Action<string> outputLog, Action<string> eventLog)
        {
            BridgeConfig.ReadConfig();
            InputLog = inputLog;
            OutputLog = outputLog;
            EventLog = eventLog;
            _BridgeUsers = new List<BridgeUser>();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[r.Next(s.Length)]).ToArray());
        }
        public void RegisterNick(string nick)
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
            var query = _BridgeUsers.Where(n => n.Nick.ToLower().Equals(nick.ToLower()));
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
                string mstr = $":{BridgeConfig.ServerIdentifier} UID {nick} {hopcount} {timestamp} {username} {hostname} {uid} {serviceStamp} {userMode} {vhost} {ipCloak} {ipAddress} :{nick}";

                write(mstr);
                var bridgeUser = new BridgeUser();
                bridgeUser.UID = uid;
                bridgeUser.UserName = username;
                bridgeUser.Nick = nick;
                _BridgeUsers.Add(bridgeUser);
            }
            else
            {
               EventLog($"Error: {nick} is already registered ");
            }

        }
        public void ChangeNick(string oldNick, string nick)
        {
            // Example
            //:00257TR0R NICK bitshift 1559074834
            //:00257TR0R NICK shiftybit 1559074836

            var query = _BridgeUsers.Where(n => n.Nick.ToLower().Equals(oldNick.ToLower()));
            if(query.ToList().Count == 0)
            {
                EventLog($"Error: {oldNick} is not a valid user");
                return;
            }
            BridgeUser user = query.First();
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
        private void write(string line)
        {
            writer.WriteLine(line);
            writer.Flush();
            OutputLog(line);
        }
        private void ChannelMessageReceived(string input)
        {
            string[] tokens = input.Split(' ');
            EventHandler<ChannelMessageEventArgs> handler = OnChannelMessage;
            if (null != handler)
            {
                ChannelMessageEvent channelMessage = new ChannelMessageEvent();
                channelMessage.User = tokens[0].Split(':')[1];
                channelMessage.Channel = tokens[2];
                string content = input.Split(new[] { ':' },3)[2];
                channelMessage.Message = content;
                handler(this, new ChannelMessageEventArgs(channelMessage));
            }
        }
        private void BridgeMain()
        {
            string input;
            while (true)
            {
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
            }
        }
        public void StartBridge()
        {
            tcpClient = new TcpClient(BridgeConfig.UplinkHost,BridgeConfig.UplinkPort);
            ns = tcpClient.GetStream();
            reader = new StreamReader(ns);
            writer = new StreamWriter(ns);
            write($"PASS {BridgeConfig.UplinkPassword}");
            write("PROTOCTL NICKv2 VHP NICKIP UMODE2 SJOIN SJOIN2 SJ3 NOQUIT TKLEXT ESVID MLOCK");
            write($"PROTOCTL EAUTH={BridgeConfig.ServerName}");
            write($"PROTOCTL SID={BridgeConfig.ServerIdentifier}");
            write($"SERVER {BridgeConfig.ServerName} 1 :{BridgeConfig.ServerDescription}");
            write($":{BridgeConfig.ServerIdentifier} EOS");
            BridgeMain();
        }
    }
}
