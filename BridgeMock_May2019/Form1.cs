using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;


namespace BridgeMock_May2019
{
    public partial class Form1 : Form
    {
        private BridgeService bridge;
        delegate void TextBoxDelegate(string text);
        public Form1()
        {
            InitializeComponent();
        }

        public void InputLog(string text)
        {
            if (!richTextBox1.InvokeRequired) { 
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = System.Drawing.Color.DarkRed;
                richTextBox1.AppendText(text + "\r\n");
                richTextBox1.SelectionColor = System.Drawing.Color.Black;
           }
             else
            {
                TextBoxDelegate d = new TextBoxDelegate(InputLog);
                this.Invoke(d, new object[] { text });
            }
        }

        public void OutputLog(string text)
        {
            if (!richTextBox1.InvokeRequired)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = System.Drawing.Color.DarkBlue;
                richTextBox1.AppendText(text + "\r\n");
                richTextBox1.SelectionColor = System.Drawing.Color.Black;
            }
            else
            {
                TextBoxDelegate d = new TextBoxDelegate(OutputLog);
                this.Invoke(d, new object[] { text });
            }
        }
        public void EventLog(string text)
        {
            if (!richTextBox1.InvokeRequired)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = System.Drawing.Color.Black;
                richTextBox1.AppendText(text + "\r\n");
                richTextBox1.SelectionColor = System.Drawing.Color.Black;
            }
            else
            {
                TextBoxDelegate d = new TextBoxDelegate(EventLog);
                this.Invoke(d, new object[] { text });
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bridge = new BridgeService(InputLog, OutputLog, EventLog);
            DiscordService discordService = new DiscordService(bridge);
            discordService.MainAsync().GetAwaiter();
            
            bridge.StartBridge();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            bridge.SendMessage(txtUserName.Text, txtMessage.Text, txtChannel.Text);
            txtMessage.Clear();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            bridge.RegisterNick(txtUserName.Text);
        }

        private void TxtUserName_KeyDown(object sender, KeyEventArgs e) {
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

        private void BtnAction_Click(object sender, EventArgs e)
        {
            bridge.SendAction(txtUserName.Text, txtMessage.Text, txtChannel.Text);
            txtMessage.Clear();
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bridge.SendMessage(txtUserName.Text, txtMessage.Text, txtChannel.Text);
                txtMessage.Clear();
            }
        }

        private void BtnChangeNick_Click(object sender, EventArgs e)
        {
            bridge.ChangeNick(txtUserName.Text, txtChangeNick.Text);
        }
    }
}
