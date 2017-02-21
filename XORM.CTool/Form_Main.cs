using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace XORM.CTool
{
    public partial class Form_Main : Form
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string DBConnString = string.Empty;
        /// <summary>
        /// 数据库类型
        /// </summary>
        private string DBType = string.Empty;
        public Form_Main()
        {
            InitializeComponent();

            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings.Count > 0)
            {
                for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                {
                    this.Combo_Conn.Items.Add(ConfigurationManager.ConnectionStrings[i].Name);
                }
            }

            ToolTip TT = new ToolTip();
            TT.SetToolTip(Server_Box, "服务器地址,如:192.168.0.1,1433(如数据库端口未修改,则不用写,1433)");
            TT.SetToolTip(DataBase_Box, "数据库用户名");
            TT.SetToolTip(DataBase_Box, "数据库名");
        }
        /// <summary>
        /// 连接字符串,并获取表列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Conn_Btn_Click(object sender, EventArgs e)
        {
            string GetTabListCmdTxt = "";
            if (this.DBType == "SQL Server")
            {
                GetTabListCmdTxt =
@"select t.id,t.name,d.[value] desctxt from sysobjects t left outer join sys.extended_properties d 
on t.id=d.major_id and d.minor_id=0
where t.xtype='U' order by t.name asc";

                SqlConnection MyConn = new SqlConnection(DBConnString);
                try
                {
                    if (MyConn.State == ConnectionState.Closed)
                    {
                        MyConn.Open();
                    }
                }
                catch
                {
                    ShowMsg("数据库连接失败");
                    return;
                }
                SqlCommand MyCmd = new SqlCommand(GetTabListCmdTxt, MyConn);
                SqlDataAdapter MyAdp = new SqlDataAdapter(MyCmd);
                DataTable TabDT = new DataTable();
                MyAdp.Fill(TabDT);
                if (MyConn.State == ConnectionState.Open)
                {
                    MyConn.Close();
                }
                ShowMsg("成功获取数据表列表");
                this.DBTabList.Items.Clear();
                foreach (DataRow dr in TabDT.Rows)
                {
                    this.DBTabList.Items.Add(dr["name"].ToString(), false);
                }
            }
            else if (this.DBType == "MySQL")
            {
                GetTabListCmdTxt =
                    @"select table_name from information_schema.tables where table_schema='home_rawdemand' and table_type='base table';";

                MySqlConnection MyConn = new MySqlConnection(DBConnString);
                try
                {
                    if (MyConn.State == ConnectionState.Closed)
                    {
                        MyConn.Open();
                    }
                }
                catch
                {
                    ShowMsg("数据库连接失败");
                    return;
                }
                MySqlCommand MyCmd = new MySqlCommand(GetTabListCmdTxt, MyConn);
                MySqlDataAdapter MyAdp = new MySqlDataAdapter(MyCmd);
                DataTable TabDT = new DataTable();
                MyAdp.Fill(TabDT);
                if (MyConn.State == ConnectionState.Open)
                {
                    MyConn.Close();
                }
                ShowMsg("成功获取数据表列表");
                this.DBTabList.Items.Clear();
                foreach (DataRow dr in TabDT.Rows)
                {
                    this.DBTabList.Items.Add(dr["table_name"].ToString(), false);
                }
            }
        }
        /// <summary>
        /// 选择实体类输出目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelDATDir_Btn_Click(object sender, EventArgs e)
        {
            if (Model_SelDirDlg.ShowDialog() == DialogResult.OK)
            {
                this.OutDATDir_Box.Text = Model_SelDirDlg.SelectedPath;
                ShowMsg("<数据实体类>输出目录：" + this.OutDATDir_Box.Text);
            }
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMsg(string msg)
        {
            this.Msg_Box.AppendText(msg + "\r\n");
        }
        /// <summary>
        /// 创建数据库操作代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBT_Btn_Click(object sender, EventArgs e)
        {
            //实体类命名空间头,如:Vending.Model.Base.
            string NameSpaceStr_DAT = this.NameSpaceDAT_Box.Text;
            //数据实体类输出目录,如:D:\\Vending.Model.Base
            string OutDirStr_DAT = this.OutDATDir_Box.Text;

            string ConnectionMark = this.CONNMARK_Box.Text.Trim();
            this.DBT_Progress.Maximum = this.DBTabList.CheckedItems.Count;
            this.DBT_Progress.Minimum = 0;
            bool UserReadOnlyForSelect = this.CHK_READONLY.Checked;

            for (int i = 0; i < this.DBTabList.CheckedItems.Count; i++)
            {
                DBS_Control DBSC = new DBS_Control(this.DBConnString, NameSpaceStr_DAT, OutDirStr_DAT, this.DBTabList.CheckedItems[i].ToString(), ConnectionMark);
                DBSC.CreateAll();
                DisplayProgress(i + 1);
                DisplayMessage(this.DBTabList.CheckedItems[i].ToString());
            }
            if (this.DBT_Progress.Value == this.DBTabList.CheckedItems.Count)
            {
                MessageBox.Show("操作成功", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public delegate void ShowMessageHandler(string TabName);
        public void ShowMessage(string TabName)
        {
            this.Msg_Box.AppendText("表：" + TabName + ",处理完成!\r\n");
            this.Msg_Box.ScrollToCaret();
        }
        public void DisplayMessage(string TabName)
        {
            if (this.Msg_Box.InvokeRequired)
            {
                ShowMessageHandler smh = new ShowMessageHandler(ShowMessage);
                smh.Invoke(TabName);
            }
            else
            {
                ShowMessage(TabName);
            }
        }

        public delegate void ShowProgressHandler(int Val);
        public void ShowProgress(int Val)
        {
            this.DBT_Progress.Value = Val;
        }

        public void DisplayProgress(int Val)
        {
            if (this.DBT_Progress.InvokeRequired)
            {
                ShowProgressHandler sph = new ShowProgressHandler(ShowProgress);
                sph.Invoke(Val);
            }
            else
            {
                ShowProgress(Val);
            }
        }

        private void SelAll_Btn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DBTabList.Items.Count; i++)
            {
                this.DBTabList.SetItemChecked(i, true);
            }
        }

        private void Combo_Conn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Combo_Conn.SelectedIndex != -1 && !string.IsNullOrEmpty(this.Combo_Conn.Text))
            {
                if (ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ProviderName == "SQL Server")
                {
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ConnectionString);
                    this.Server_Box.Text = conn.DataSource;
                    this.DataBase_Box.Text = conn.Database;
                    this.DBConnString = ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ConnectionString;
                    this.DBType = ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ProviderName;
                    conn.Dispose();
                }
                else if (ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ProviderName == "MySQL")
                {
                    MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ConnectionString);
                    this.Server_Box.Text = conn.DataSource;
                    this.DataBase_Box.Text = conn.Database;
                    this.DBConnString = ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ConnectionString;
                    this.DBType = ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ProviderName;
                    conn.Dispose();
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_DIR"]))
                {
                    this.OutDATDir_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_DIR"];
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_NAS"]))
                {
                    this.NameSpaceDAT_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_NAS"];
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_CON"]))
                {
                    this.CONNMARK_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_CON"];
                }
            }
        }
    }
}