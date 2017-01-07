using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace XORM.CoreTool
{
    public partial class Form_Main : Form
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string DBConnString = string.Empty;
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
            TT.SetToolTip(NameSpaceDBO_Box, "数据访问组件命名空间,如:Vending.DatasInfo.");
            TT.SetToolTip(SelDBODir_Btn, "数据访问组件根目录,如D:\\Vending.DatasInfo");
        }
        /// <summary>
        /// 连接字符串,并获取表列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Conn_Btn_Click(object sender, EventArgs e)
        {
            //DBConnString = "Persist Security Info=False;Data Source=" + this.Server_Box.Text.Trim() +
            //               ";Initial Catalog=" + this.DataBase_Box.Text.Trim() + ";User ID=" + this.UID_Box.Text.Trim() +
            //               ";Password=" + this.PWD_Box.Text.Trim();
            string GetTabListCmdTxt =
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
        /// <summary>
        /// 选择访问类输出目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelDir_Btn_Click(object sender, EventArgs e)
        {
            if (Data_SelDirDlg.ShowDialog() == DialogResult.OK)
            {
                this.OutDBODir_Box.Text = Data_SelDirDlg.SelectedPath;
                ShowMsg("<数据访问类>输出目录：" + this.OutDBODir_Box.Text);
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
            //访问类命名控件头,如:Vending.DatasInfo.
            string NameSpaceStr_DBO = this.NameSpaceDBO_Box.Text + ".";
            //实体类命名空间头,如:Vending.Model.Base.
            string NameSpaceStr_DAT = this.NameSpaceDAT_Box.Text + ".";
            //数据访问类输出目录,如:D:\\Vending.DatasInfo
            string OutDirStr_DBO = this.OutDBODir_Box.Text;
            //数据实体类输出目录,如:D:\\Vending.Model.Base
            string OutDirStr_DAT = this.OutDATDir_Box.Text;

            string ConnectionMark = this.CONNMARK_Box.Text.Trim();
            this.DBT_Progress.Maximum = this.DBTabList.CheckedItems.Count;
            this.DBT_Progress.Minimum = 0;
            bool UserReadOnlyForSelect = this.CHK_READONLY.Checked;

            for (int i = 0; i < this.DBTabList.CheckedItems.Count; i++)
            {
                DBS_Control DBSC = new DBS_Control(this.DBConnString, NameSpaceStr_DBO, OutDirStr_DBO, NameSpaceStr_DAT, OutDirStr_DAT, this.DBTabList.CheckedItems[i].ToString(), ConnectionMark, UserReadOnlyForSelect);
                DBSC.CreateAll();
                this.DBT_Progress.Value = i + 1;
            }
            if (this.DBT_Progress.Value == this.DBTabList.CheckedItems.Count)
            {
                MessageBox.Show("操作成功", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ConnectionString);
                this.Server_Box.Text = conn.DataSource;
                this.DataBase_Box.Text = conn.Database;
                this.DBConnString = ConfigurationManager.ConnectionStrings[this.Combo_Conn.Text].ConnectionString;
                conn.Dispose();

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_DIR_DBO"]))
                {
                    this.OutDBODir_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_DIR_DBO"];
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_DIR_MOD"]))
                {
                    this.OutDATDir_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_DIR_MOD"];
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_NAS_DAT"]))
                {
                    this.NameSpaceDAT_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_NAS_DAT"];
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_NAS_DBO"]))
                {
                    this.NameSpaceDBO_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_NAS_DBO"];
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_MARK_CON"]))
                {
                    this.CONNMARK_Box.Text = ConfigurationManager.AppSettings[this.Combo_Conn.Text + "_MARK_CON"];
                }
            }
        }
    }
}