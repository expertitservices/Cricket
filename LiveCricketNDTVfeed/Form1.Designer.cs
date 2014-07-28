namespace LiveCricketNDTVfeed
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.lbProxy = new System.Windows.Forms.ListBox();
            this.tbProxies = new System.Windows.Forms.TextBox();
            this.lbProxies = new System.Windows.Forms.Label();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.lvTournament = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TeamScore = new System.Windows.Forms.Label();
            this.abort = new System.Windows.Forms.Button();
            this.scorecard = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lInning = new System.Windows.Forms.Label();
            this.gbBow = new System.Windows.Forms.GroupBox();
            this.lvBow = new System.Windows.Forms.ListView();
            this.cbIn = new System.Windows.Forms.ComboBox();
            this.gbBat = new System.Windows.Forms.GroupBox();
            this.lvBat = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lvFall = new System.Windows.Forms.ListView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.slbUKTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.restart = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbXML = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbBow.SuspendLayout();
            this.gbBat.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbProxy
            // 
            this.lbProxy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbProxy.FormattingEnabled = true;
            this.lbProxy.Location = new System.Drawing.Point(600, 522);
            this.lbProxy.Name = "lbProxy";
            this.lbProxy.Size = new System.Drawing.Size(195, 54);
            this.lbProxy.TabIndex = 183;
            // 
            // tbProxies
            // 
            this.tbProxies.BackColor = System.Drawing.Color.White;
            this.tbProxies.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbProxies.Location = new System.Drawing.Point(544, 523);
            this.tbProxies.Name = "tbProxies";
            this.tbProxies.ReadOnly = true;
            this.tbProxies.Size = new System.Drawing.Size(50, 20);
            this.tbProxies.TabIndex = 182;
            // 
            // lbProxies
            // 
            this.lbProxies.AutoSize = true;
            this.lbProxies.Location = new System.Drawing.Point(490, 526);
            this.lbProxies.Name = "lbProxies";
            this.lbProxies.Size = new System.Drawing.Size(47, 13);
            this.lbProxies.TabIndex = 181;
            this.lbProxies.Text = "Proxies :";
            // 
            // timerMain
            // 
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // lvTournament
            // 
            this.lvTournament.FullRowSelect = true;
            this.lvTournament.GridLines = true;
            this.lvTournament.Location = new System.Drawing.Point(5, 22);
            this.lvTournament.Name = "lvTournament";
            this.lvTournament.Size = new System.Drawing.Size(1072, 137);
            this.lvTournament.TabIndex = 184;
            this.lvTournament.UseCompatibleStateImageBehavior = false;
            this.lvTournament.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvTournament_MouseDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TeamScore);
            this.groupBox1.Controls.Add(this.abort);
            this.groupBox1.Controls.Add(this.scorecard);
            this.groupBox1.Controls.Add(this.lvTournament);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1081, 197);
            this.groupBox1.TabIndex = 186;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tournament";
            // 
            // TeamScore
            // 
            this.TeamScore.AutoSize = true;
            this.TeamScore.Location = new System.Drawing.Point(8, 170);
            this.TeamScore.Name = "TeamScore";
            this.TeamScore.Size = new System.Drawing.Size(0, 13);
            this.TeamScore.TabIndex = 188;
            // 
            // abort
            // 
            this.abort.Location = new System.Drawing.Point(968, 165);
            this.abort.Name = "abort";
            this.abort.Size = new System.Drawing.Size(107, 23);
            this.abort.TabIndex = 187;
            this.abort.Text = "Abort Task";
            this.abort.UseVisualStyleBackColor = true;
            this.abort.Click += new System.EventHandler(this.abort_Click);
            // 
            // scorecard
            // 
            this.scorecard.Location = new System.Drawing.Point(855, 165);
            this.scorecard.Name = "scorecard";
            this.scorecard.Size = new System.Drawing.Size(107, 23);
            this.scorecard.TabIndex = 185;
            this.scorecard.Text = "Show Scorecard";
            this.scorecard.UseVisualStyleBackColor = true;
            this.scorecard.Click += new System.EventHandler(this.scorecard_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lInning);
            this.groupBox2.Controls.Add(this.gbBow);
            this.groupBox2.Controls.Add(this.cbIn);
            this.groupBox2.Controls.Add(this.gbBat);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(6, 206);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1081, 311);
            this.groupBox2.TabIndex = 187;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scorecard";
            // 
            // lInning
            // 
            this.lInning.AutoSize = true;
            this.lInning.Location = new System.Drawing.Point(253, 32);
            this.lInning.Name = "lInning";
            this.lInning.Size = new System.Drawing.Size(0, 13);
            this.lInning.TabIndex = 192;
            // 
            // gbBow
            // 
            this.gbBow.Controls.Add(this.lvBow);
            this.gbBow.Location = new System.Drawing.Point(429, 60);
            this.gbBow.Name = "gbBow";
            this.gbBow.Size = new System.Drawing.Size(427, 250);
            this.gbBow.TabIndex = 190;
            this.gbBow.TabStop = false;
            this.gbBow.Text = "BOWLING";
            // 
            // lvBow
            // 
            this.lvBow.FullRowSelect = true;
            this.lvBow.GridLines = true;
            this.lvBow.Location = new System.Drawing.Point(5, 16);
            this.lvBow.Name = "lvBow";
            this.lvBow.Size = new System.Drawing.Size(418, 230);
            this.lvBow.TabIndex = 1;
            this.lvBow.UseCompatibleStateImageBehavior = false;
            // 
            // cbIn
            // 
            this.cbIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIn.FormattingEnabled = true;
            this.cbIn.Location = new System.Drawing.Point(94, 29);
            this.cbIn.Name = "cbIn";
            this.cbIn.Size = new System.Drawing.Size(142, 21);
            this.cbIn.TabIndex = 191;
            this.cbIn.SelectedIndexChanged += new System.EventHandler(this.cbIn_SelectedIndexChanged);
            // 
            // gbBat
            // 
            this.gbBat.Controls.Add(this.lvBat);
            this.gbBat.Location = new System.Drawing.Point(1, 60);
            this.gbBat.Name = "gbBat";
            this.gbBat.Size = new System.Drawing.Size(427, 250);
            this.gbBat.TabIndex = 189;
            this.gbBat.TabStop = false;
            this.gbBat.Text = "BATTING";
            // 
            // lvBat
            // 
            this.lvBat.FullRowSelect = true;
            this.lvBat.GridLines = true;
            this.lvBat.Location = new System.Drawing.Point(5, 16);
            this.lvBat.Name = "lvBat";
            this.lvBat.Size = new System.Drawing.Size(418, 230);
            this.lvBat.TabIndex = 2;
            this.lvBat.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 190;
            this.label1.Text = "Select Innings :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lvFall);
            this.groupBox3.Location = new System.Drawing.Point(857, 60);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(223, 250);
            this.groupBox3.TabIndex = 188;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "FALL OF WICKETS";
            // 
            // lvFall
            // 
            this.lvFall.FullRowSelect = true;
            this.lvFall.GridLines = true;
            this.lvFall.Location = new System.Drawing.Point(4, 16);
            this.lvFall.Name = "lvFall";
            this.lvFall.Size = new System.Drawing.Size(214, 230);
            this.lvFall.TabIndex = 1;
            this.lvFall.UseCompatibleStateImageBehavior = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slbUKTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 589);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1095, 22);
            this.statusStrip1.TabIndex = 188;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // slbUKTime
            // 
            this.slbUKTime.Name = "slbUKTime";
            this.slbUKTime.Size = new System.Drawing.Size(58, 17);
            this.slbUKTime.Text = "UK Time :";
            // 
            // restart
            // 
            this.restart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.restart.Location = new System.Drawing.Point(823, 532);
            this.restart.Name = "restart";
            this.restart.Size = new System.Drawing.Size(83, 23);
            this.restart.TabIndex = 189;
            this.restart.Text = "Restart";
            this.restart.UseVisualStyleBackColor = true;
            this.restart.Click += new System.EventHandler(this.restart_Click);
            // 
            // stop
            // 
            this.stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.stop.Location = new System.Drawing.Point(912, 532);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(83, 23);
            this.stop.TabIndex = 190;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // exit
            // 
            this.exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.exit.Location = new System.Drawing.Point(1001, 532);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(82, 23);
            this.exit.TabIndex = 191;
            this.exit.Text = "Exit";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(11, 559);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 196;
            this.label8.Text = "LogDir :";
            // 
            // tbLog
            // 
            this.tbLog.BackColor = System.Drawing.Color.White;
            this.tbLog.Location = new System.Drawing.Point(113, 556);
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.Size = new System.Drawing.Size(339, 20);
            this.tbLog.TabIndex = 197;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(11, 537);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 193;
            this.label2.Text = "XMLStoreDir :";
            // 
            // tbXML
            // 
            this.tbXML.BackColor = System.Drawing.Color.White;
            this.tbXML.Location = new System.Drawing.Point(113, 530);
            this.tbXML.Name = "tbXML";
            this.tbXML.ReadOnly = true;
            this.tbXML.Size = new System.Drawing.Size(339, 20);
            this.tbXML.TabIndex = 192;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1095, 611);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbProxy);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbXML);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.restart);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbProxies);
            this.Controls.Add(this.lbProxies);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Live Cricket NDTV Feed v1.1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbBow.ResumeLayout(false);
            this.gbBat.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbProxy;
        private System.Windows.Forms.TextBox tbProxies;
        private System.Windows.Forms.Label lbProxies;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.ListView lvTournament;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button abort;
        private System.Windows.Forms.Button scorecard;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView lvBat;
        private System.Windows.Forms.ListView lvFall;
        private System.Windows.Forms.ListView lvBow;
        private System.Windows.Forms.ComboBox cbIn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox gbBat;
        private System.Windows.Forms.GroupBox gbBow;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slbUKTime;
        private System.Windows.Forms.Label lInning;
        private System.Windows.Forms.Button restart;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbXML;
        private System.Windows.Forms.Label TeamScore;
    }
}

