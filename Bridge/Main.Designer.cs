namespace ToP.Bridge
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabIrc = new System.Windows.Forms.TabPage();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnAway = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnChangeNick = new System.Windows.Forms.Button();
            this.txtChangeNick = new System.Windows.Forms.TextBox();
            this.btnAction = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.tabDiscord = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabIrc.SuspendLayout();
            this.tabDiscord.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(843, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearLogToolStripMenuItem,
            this.exportLogToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.logToolStripMenuItem.Text = "Log";
            // 
            // clearLogToolStripMenuItem
            // 
            this.clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            this.clearLogToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.clearLogToolStripMenuItem.Text = "Clear Log";
            this.clearLogToolStripMenuItem.Click += new System.EventHandler(this.ClearLogToolStripMenuItem_Click);
            // 
            // exportLogToolStripMenuItem
            // 
            this.exportLogToolStripMenuItem.Name = "exportLogToolStripMenuItem";
            this.exportLogToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.exportLogToolStripMenuItem.Text = "Export Log";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabIrc);
            this.tabControl1.Controls.Add(this.tabDiscord);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(843, 539);
            this.tabControl1.TabIndex = 16;
            // 
            // tabIrc
            // 
            this.tabIrc.Controls.Add(this.btnDisconnect);
            this.tabIrc.Controls.Add(this.btnBack);
            this.tabIrc.Controls.Add(this.btnAway);
            this.tabIrc.Controls.Add(this.richTextBox1);
            this.tabIrc.Controls.Add(this.btnChangeNick);
            this.tabIrc.Controls.Add(this.txtChangeNick);
            this.tabIrc.Controls.Add(this.btnAction);
            this.tabIrc.Controls.Add(this.btnJoin);
            this.tabIrc.Controls.Add(this.btnRegister);
            this.tabIrc.Controls.Add(this.btnSend);
            this.tabIrc.Controls.Add(this.label5);
            this.tabIrc.Controls.Add(this.label4);
            this.tabIrc.Controls.Add(this.label3);
            this.tabIrc.Controls.Add(this.label2);
            this.tabIrc.Controls.Add(this.label1);
            this.tabIrc.Controls.Add(this.txtMessage);
            this.tabIrc.Controls.Add(this.txtChannel);
            this.tabIrc.Controls.Add(this.txtUserName);
            this.tabIrc.Location = new System.Drawing.Point(4, 22);
            this.tabIrc.Name = "tabIrc";
            this.tabIrc.Padding = new System.Windows.Forms.Padding(3);
            this.tabIrc.Size = new System.Drawing.Size(835, 513);
            this.tabIrc.TabIndex = 0;
            this.tabIrc.Text = "IRC Controls";
            this.tabIrc.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(668, 55);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 32;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // btnAway
            // 
            this.btnAway.Location = new System.Drawing.Point(586, 55);
            this.btnAway.Name = "btnAway";
            this.btnAway.Size = new System.Drawing.Size(75, 23);
            this.btnAway.TabIndex = 31;
            this.btnAway.Text = "Away";
            this.btnAway.UseVisualStyleBackColor = true;
            this.btnAway.Click += new System.EventHandler(this.BtnAway_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(10, 138);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(819, 358);
            this.richTextBox1.TabIndex = 30;
            this.richTextBox1.Text = "";
            // 
            // btnChangeNick
            // 
            this.btnChangeNick.Location = new System.Drawing.Point(489, 55);
            this.btnChangeNick.Name = "btnChangeNick";
            this.btnChangeNick.Size = new System.Drawing.Size(90, 23);
            this.btnChangeNick.TabIndex = 29;
            this.btnChangeNick.Text = "Change Nick";
            this.btnChangeNick.UseVisualStyleBackColor = true;
            this.btnChangeNick.Click += new System.EventHandler(this.BtnChangeNick_Click);
            // 
            // txtChangeNick
            // 
            this.txtChangeNick.Location = new System.Drawing.Point(345, 57);
            this.txtChangeNick.Name = "txtChangeNick";
            this.txtChangeNick.Size = new System.Drawing.Size(137, 20);
            this.txtChangeNick.TabIndex = 28;
            // 
            // btnAction
            // 
            this.btnAction.Location = new System.Drawing.Point(488, 109);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(75, 23);
            this.btnAction.TabIndex = 27;
            this.btnAction.Text = "Action!";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.BtnAction_Click);
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(249, 81);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(75, 23);
            this.btnJoin.TabIndex = 26;
            this.btnJoin.Text = "Join";
            this.btnJoin.UseVisualStyleBackColor = true;
            this.btnJoin.Click += new System.EventHandler(this.BtnJoin_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(249, 55);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnRegister.TabIndex = 25;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(407, 109);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 24;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.BtnSend_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Message";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Channel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Makes a shitty Form";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "The Angry Programmer";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(142, 109);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(258, 20);
            this.txtMessage.TabIndex = 18;
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtMessage_KeyDown);
            // 
            // txtChannel
            // 
            this.txtChannel.Location = new System.Drawing.Point(142, 83);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(100, 20);
            this.txtChannel.TabIndex = 17;
            this.txtChannel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtChannel_KeyDown);
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(142, 57);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(100, 20);
            this.txtUserName.TabIndex = 16;
            this.txtUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtUserName_KeyDown);
            // 
            // tabDiscord
            // 
            this.tabDiscord.Controls.Add(this.label7);
            this.tabDiscord.Controls.Add(this.label6);
            this.tabDiscord.Controls.Add(this.richTextBox2);
            this.tabDiscord.Location = new System.Drawing.Point(4, 22);
            this.tabDiscord.Name = "tabDiscord";
            this.tabDiscord.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiscord.Size = new System.Drawing.Size(818, 513);
            this.tabDiscord.TabIndex = 1;
            this.tabDiscord.Text = "Discord Controls";
            this.tabDiscord.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(91, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "To be desired.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "I leave more";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(6, 175);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(806, 331);
            this.richTextBox2.TabIndex = 0;
            this.richTextBox2.Text = "";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(748, 55);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 33;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.BtnDisconnect_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 567);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "What a Save!";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabIrc.ResumeLayout(false);
            this.tabIrc.PerformLayout();
            this.tabDiscord.ResumeLayout(false);
            this.tabDiscord.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportLogToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabIrc;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnChangeNick;
        private System.Windows.Forms.TextBox txtChangeNick;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TabPage tabDiscord;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAway;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnDisconnect;
    }
}

