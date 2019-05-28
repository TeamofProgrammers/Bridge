using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace BridgeMock_May2019
{
    class BridgeService
    {
        private string _ServerHost;
        private int _ServerPort;
        private string _ServerPassword;
        private TcpClient tcpClient;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        static Action<string> InputLog;
        static Action<string> OutputLog;
        private string _ServerIdentifier;

        private static Random r = new Random();
        public BridgeService(string ServerHost, int ServerPort, string ServerPassword, Action<string> inputLog, Action<string> outputLog)
        {
            this._ServerHost = ServerHost;
            this._ServerPort = ServerPort;
            this._ServerPassword = ServerPassword;
            this._ServerIdentifier = "00B";
            InputLog = inputLog;
            OutputLog = outputLog;
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
            string mstr = $":{_ServerIdentifier} UID {nick} {hopcount} {timestamp} {username} {hostname} {uid} {serviceStamp} {userMode} {vhost} {ipCloak} {ipAddress} :{nick}";

            write(mstr);

        }
        public void JoinChannel(string nick, string channel)
        {
            // example
            // :darkscrypt JOIN #topdev
            string mstr = $":{nick} JOIN {channel}";
            write(mstr);
        }
        public void SendMessage(string nick, string message, string channel = "#TOP")
        {
            // example
            // :darkscrypt PRIVMSG #top : message here
            string mstr = $":{nick} PRIVMSG {channel} : {message}";
            write(mstr);
        }
        public void Action(string nick, string action, string channel = "#TOP")
        {
            // example  :shiftybit PRIVMSG #TOP :ACTION flips a table
            //string mstr = $":{nick} PRIVMSG {channel} :ACTION {action}";
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
        public void StartBridge()
        {
            tcpClient = new TcpClient(_ServerHost, _ServerPort);
            ns = tcpClient.GetStream();
            reader = new StreamReader(ns);
            writer = new StreamWriter(ns);
            write("PASS GoldenRetriever");
            write("PROTOCTL NICKv2 VHP NICKIP UMODE2 SJOIN SJOIN2 SJ3 NOQUIT TKLEXT ESVID MLOCK");
            write("PROTOCTL EAUTH=bridgeserv.teamofprogrammers.com");
            write("PROTOCTL SID=00B");
            write("SERVER bridgeserv.teamofprogrammers.com 1 :change me");
            write(":00B EOS");
            string input;
            while (true)
            {
                while((input = reader.ReadLine()) != null)
                {
                    InputLog(input);
                    string[] tokens = input.Split(' ');
                    if(tokens[0].ToUpper() == "PING")
                    {
                        write("PONG " + tokens[1]);
                    }

                    switch (tokens[1])
                    {
                        case "001":
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }
}
