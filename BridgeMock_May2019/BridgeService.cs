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
        private static Random r = new Random();
        public BridgeService(string ServerHost, int ServerPort, string ServerPassword)
        {
            this._ServerHost = ServerHost;
            this._ServerPort = ServerPort;
            this._ServerPassword = ServerPassword;
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
            string mstr = "";
            mstr += ":00B ";  // Server Identifier
            mstr += "UID "; // UID Command https://www.unrealircd.org/docs/Server_protocol:UID_command
            mstr += nick + " "; // nickname
            mstr += "0 "; // Hop Count
            mstr += "0 "; // timestamp when user came online
            mstr += nick + "_"+ r.Next(1000, 9999).ToString() + " " ; // username *HACK*
            mstr += "discordBot "; // hostname
            mstr += RandomString(9) + " "; // user UID. Currently 9 character random alphanumeric  *HACK*
            mstr += "0 "; // service stamp (i guess when recognized by services??)
            mstr += "+iw-x "; // user modes
            mstr += "* "; // VHOST. * if blank
            mstr += "BridgeCloak "; // this should be the cloak, but we can use this hacky thing for now
            mstr += "* "; // This is the ip address, apparently * works too based on tcpdump output i've seen
            mstr += ":" + nick; // this is the real name, can have spaces. used in last
            write(mstr);

        }
        public void JoinChannel(string nick, string channel)
        {
            // example
            // :darkscrypt JOIN #topdev
            string mstr = "";
            mstr += ":" + nick + " ";
            mstr += "JOIN ";
            mstr += channel;
            write(mstr);
        }
        public void MessageChannel(string nick, string message, string channel = "#TOP")
        {
            write(":" + nick + " PRIVMSG " + channel + " :" + message);
        }
        private void write(string line)
        {
            writer.WriteLine(line);
            writer.Flush();
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
