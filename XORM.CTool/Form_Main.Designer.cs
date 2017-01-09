namespace XORM.CTool
{
    partial class Form_Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SelAll_Btn = new System.Windows.Forms.Button();
            this.DBT_Btn = new System.Windows.Forms.Button();
            this.Conn_Btn = new System.Windows.Forms.Button();
            this.DataBase_Box = new System.Windows.Forms.TextBox();
            this.Server_Box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CONNMARK_Box = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.NameSpaceDAT_Box = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SelDATDir_Btn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DBTabList = new System.Windows.Forms.CheckedListBox();
            this.OutDATDir_Box = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DBT_Progress = new System.Windows.Forms.ProgressBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Msg_Box = new System.Windows.Forms.RichTextBox();
            this.Combo_Conn = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.CHK_READONLY = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Model_SelDirDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SelAll_Btn);
            this.groupBox1.Controls.Add(this.DBT_Btn);
            this.groupBox1.Controls.Add(this.Conn_Btn);
            this.groupBox1.Controls.Add(this.DataBase_Box);
            this.groupBox1.Controls.Add(this.Server_Box);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(373, 125);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库配置";
            // 
            // SelAll_Btn
            // 
            this.SelAll_Btn.Location = new System.Drawing.Point(142, 82);
            this.SelAll_Btn.Name = "SelAll_Btn";
            this.SelAll_Btn.Size = new System.Drawing.Size(102, 23);
            this.SelAll_Btn.TabIndex = 8;
            this.SelAll_Btn.Text = "全部选择";
            this.SelAll_Btn.UseVisualStyleBackColor = true;
            this.SelAll_Btn.Click += new System.EventHandler(this.SelAll_Btn_Click);
            // 
            // DBT_Btn
            // 
            this.DBT_Btn.Location = new System.Drawing.Point(265, 82);
            this.DBT_Btn.Name = "DBT_Btn";
            this.DBT_Btn.Size = new System.Drawing.Size(102, 23);
            this.DBT_Btn.TabIndex = 7;
            this.DBT_Btn.Text = "生成";
            this.DBT_Btn.UseVisualStyleBackColor = true;
            this.DBT_Btn.Click += new System.EventHandler(this.DBT_Btn_Click);
            // 
            // Conn_Btn
            // 
            this.Conn_Btn.Location = new System.Drawing.Point(13, 82);
            this.Conn_Btn.Name = "Conn_Btn";
            this.Conn_Btn.Size = new System.Drawing.Size(102, 23);
            this.Conn_Btn.TabIndex = 5;
            this.Conn_Btn.Text = "连接数据库";
            this.Conn_Btn.UseVisualStyleBackColor = true;
            this.Conn_Btn.Click += new System.EventHandler(this.Conn_Btn_Click);
            // 
            // DataBase_Box
            // 
            this.DataBase_Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DataBase_Box.Location = new System.Drawing.Point(56, 56);
            this.DataBase_Box.Name = "DataBase_Box";
            this.DataBase_Box.ReadOnly = true;
            this.DataBase_Box.Size = new System.Drawing.Size(311, 21);
            this.DataBase_Box.TabIndex = 1;
            // 
            // Server_Box
            // 
            this.Server_Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Server_Box.Location = new System.Drawing.Point(56, 29);
            this.Server_Box.Name = "Server_Box";
            this.Server_Box.ReadOnly = true;
            this.Server_Box.Size = new System.Drawing.Size(311, 21);
            this.Server_Box.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "数据库";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "主机";
            // 
            // CONNMARK_Box
            // 
            this.CONNMARK_Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CONNMARK_Box.Location = new System.Drawing.Point(139, 48);
            this.CONNMARK_Box.Name = "CONNMARK_Box";
            this.CONNMARK_Box.Size = new System.Drawing.Size(224, 21);
            this.CONNMARK_Box.TabIndex = 16;
            this.CONNMARK_Box.Text = "EBS";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(44, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 15;
            this.label11.Text = "链接字符串前缀";
            // 
            // NameSpaceDAT_Box
            // 
            this.NameSpaceDAT_Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameSpaceDAT_Box.Location = new System.Drawing.Point(139, 19);
            this.NameSpaceDAT_Box.Name = "NameSpaceDAT_Box";
            this.NameSpaceDAT_Box.Size = new System.Drawing.Size(224, 21);
            this.NameSpaceDAT_Box.TabIndex = 10;
            this.NameSpaceDAT_Box.Text = "EBS.Interface.Model";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 12);
            this.label8.TabIndex = 11;
            this.label8.Text = "(数据实体类)命名空间";
            // 
            // SelDATDir_Btn
            // 
            this.SelDATDir_Btn.Location = new System.Drawing.Point(369, 18);
            this.SelDATDir_Btn.Name = "SelDATDir_Btn";
            this.SelDATDir_Btn.Size = new System.Drawing.Size(83, 23);
            this.SelDATDir_Btn.TabIndex = 9;
            this.SelDATDir_Btn.Text = "输出目录...";
            this.SelDATDir_Btn.UseVisualStyleBackColor = true;
            this.SelDATDir_Btn.Click += new System.EventHandler(this.SelDATDir_Btn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.DBTabList);
            this.groupBox2.Location = new System.Drawing.Point(1, 172);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(373, 487);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据表列表";
            // 
            // DBTabList
            // 
            this.DBTabList.BackColor = System.Drawing.Color.LightCyan;
            this.DBTabList.CheckOnClick = true;
            this.DBTabList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DBTabList.FormattingEnabled = true;
            this.DBTabList.Location = new System.Drawing.Point(3, 17);
            this.DBTabList.Name = "DBTabList";
            this.DBTabList.Size = new System.Drawing.Size(367, 467);
            this.DBTabList.TabIndex = 8;
            // 
            // OutDATDir_Box
            // 
            this.OutDATDir_Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OutDATDir_Box.Location = new System.Drawing.Point(138, 75);
            this.OutDATDir_Box.Name = "OutDATDir_Box";
            this.OutDATDir_Box.ReadOnly = true;
            this.OutDATDir_Box.Size = new System.Drawing.Size(454, 21);
            this.OutDATDir_Box.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(125, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "(数据实体类)输出目录";
            // 
            // DBT_Progress
            // 
            this.DBT_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DBT_Progress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.DBT_Progress.Location = new System.Drawing.Point(382, 6);
            this.DBT_Progress.Name = "DBT_Progress";
            this.DBT_Progress.Size = new System.Drawing.Size(599, 23);
            this.DBT_Progress.Step = 1;
            this.DBT_Progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.DBT_Progress.TabIndex = 11;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.Msg_Box);
            this.groupBox4.Location = new System.Drawing.Point(380, 172);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(601, 487);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "操作消息";
            // 
            // Msg_Box
            // 
            this.Msg_Box.BackColor = System.Drawing.SystemColors.WindowText;
            this.Msg_Box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Msg_Box.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Msg_Box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Msg_Box.ForeColor = System.Drawing.Color.Red;
            this.Msg_Box.Location = new System.Drawing.Point(3, 17);
            this.Msg_Box.Name = "Msg_Box";
            this.Msg_Box.ReadOnly = true;
            this.Msg_Box.Size = new System.Drawing.Size(595, 467);
            this.Msg_Box.TabIndex = 11;
            this.Msg_Box.Text = "";
            // 
            // Combo_Conn
            // 
            this.Combo_Conn.FormattingEnabled = true;
            this.Combo_Conn.Location = new System.Drawing.Point(83, 9);
            this.Combo_Conn.Name = "Combo_Conn";
            this.Combo_Conn.Size = new System.Drawing.Size(121, 20);
            this.Combo_Conn.TabIndex = 4;
            this.Combo_Conn.SelectedIndexChanged += new System.EventHandler(this.Combo_Conn_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 5;
            this.label12.Text = "链接字符串";
            // 
            // CHK_READONLY
            // 
            this.CHK_READONLY.AutoSize = true;
            this.CHK_READONLY.Checked = true;
            this.CHK_READONLY.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_READONLY.Location = new System.Drawing.Point(260, 11);
            this.CHK_READONLY.Name = "CHK_READONLY";
            this.CHK_READONLY.Size = new System.Drawing.Size(108, 16);
            this.CHK_READONLY.TabIndex = 6;
            this.CHK_READONLY.Text = "只读串进行查询";
            this.CHK_READONLY.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.OutDATDir_Box);
            this.groupBox5.Controls.Add(this.CONNMARK_Box);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.SelDATDir_Btn);
            this.groupBox5.Controls.Add(this.NameSpaceDAT_Box);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(380, 41);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(599, 125);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "代码配置";
            // 
            // Model_SelDirDlg
            // 
            this.Model_SelDirDlg.Description = "请选择输出目录";
            // 
            // Form_Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.DBT_Progress);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.CHK_READONLY);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.Combo_Conn);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(1000, 700);
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "欢迎使用代码生成";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox DataBase_Box;
        private System.Windows.Forms.TextBox Server_Box;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DBT_Btn;
        private System.Windows.Forms.Button Conn_Btn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox DBTabList;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RichTextBox Msg_Box;
        private System.Windows.Forms.ProgressBar DBT_Progress;
        private System.Windows.Forms.Button SelAll_Btn;
        private System.Windows.Forms.TextBox NameSpaceDAT_Box;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button SelDATDir_Btn;
        private System.Windows.Forms.TextBox OutDATDir_Box;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox CONNMARK_Box;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox Combo_Conn;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox CHK_READONLY;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.FolderBrowserDialog Model_SelDirDlg;
    }
}