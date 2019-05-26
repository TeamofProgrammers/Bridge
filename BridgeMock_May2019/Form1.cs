using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BridgeMock_May2019
{
    public partial class Form1 : Form
    {
        private BridgeService bridge;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry("192.168.1.6");
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bridge = new BridgeService("192.168.1.6", 7001, "GoldenRetriever");
            bridge.StartBridge();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            bridge.MessageChannel(txtUserName.Text, txtMessage.Text);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            bridge.RegisterNick(txtUserName.Text);
        }

        private void TxtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                bridge.RegisterNick(txtUserName.Text);
            }
        }

        private void BtnJoin_Click(object sender, EventArgs e)
        {
            bridge.JoinChannel(txtUserName.Text, txtChannel.Text);
        }

        private void TxtChannel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bridge.JoinChannel(txtUserName.Text, txtChannel.Text);
            }
        }
    }
}
