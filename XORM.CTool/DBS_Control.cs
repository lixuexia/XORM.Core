using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace XORM.CTool
{
    public class DBS_Control
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string _AdminConnectionString = string.Empty;
        /// <summary>
        /// 数据实体类命名空间头
        /// </summary>
        private string _ModelNameSpace = string.Empty;
        /// <summary>
        /// 数据实体类输出目录
        /// </summary>
        private string _ModelFolder = string.Empty;
        /// <summary>
        /// 数据库表名
        /// </summary>
        private string _TableName = string.Empty;
        /// <summary>
        /// 链接字符串前缀
        /// </summary>
        private string _ConnectionMark = string.Empty;

        public DBS_Control(string ConnStr, string NameSpaceStr, string FolderStr, string TableName, string ConnectionMark)
        {
            this._AdminConnectionString = ConnStr;
            this._ModelNameSpace = NameSpaceStr;
            this._ModelFolder = FolderStr;
            this._TableName = TableName;
            this._ConnectionMark = ConnectionMark;
        }

        public void CreateAll()
        {
            string sql_GetStruct = "";
            if (ConfigurationManager.ConnectionStrings[this._ConnectionMark].ProviderName == "SQL Server")
            {
                sql_GetStruct =
@"select distinct a.*,b.value as Description,c.name as Xtype_Name,comm.text as dval from 
(
select id,colid,name,xtype,length,colstat,autoval,isnullable,COLUMNPROPERTY(a.id,a.name,'IsIdentity') as IsIdentity,cdefault,
(SELECT count(*) FROM sysobjects WHERE (name in (SELECT name FROM sysindexes WHERE (id = a.id) AND
(indid in (SELECT indid FROM sysindexkeys WHERE (id = a.id) AND (colid in (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name))))))) AND (xtype = 'PK')) as PK
from syscolumns as a where name<>'rowguid' and id in(select id from sysobjects where xtype='U' and name='" + this._TableName + @"')
) as a 
left outer join sys.extended_properties as b on (a.id=b.major_id and a.colid=b.minor_id)
left outer join systypes as c on (a.xtype=c.xtype and c.xtype=c.xusertype)
left outer join syscomments as comm on a.cdefault = comm.id 
where b.class_desc ='OBJECT_OR_COLUMN' or b.class_desc is null";

                SqlConnection STRUCTConn = new SqlConnection(this._AdminConnectionString);
                SqlCommand STRUCTCmd = new SqlCommand(sql_GetStruct, STRUCTConn);
                SqlDataAdapter STRUCTAdp = new SqlDataAdapter(STRUCTCmd);
                DataTable SDT = new DataTable();
                STRUCTAdp.Fill(SDT);
                STRUCTConn.Close();
                STRUCTConn.Dispose();

                if (SDT != null && SDT.Rows.Count > 0)
                {
                    this.CheckAndCreateDir(this._ModelFolder);
                    //创建数据实体类
                    this.Create_Class(SDT, this._TableName, this._ModelFolder + "\\" + this._TableName + ".cs", this._ModelNameSpace, this._ModelFolder);
                }
            }
            else if(ConfigurationManager.ConnectionStrings[this._ConnectionMark].ProviderName == "MySQL")
            {
                sql_GetStruct =
                    @"select COLUMN_NAME as name,ORDINAL_POSITION,COLUMN_DEFAULT as dval,IS_NULLABLE as isnullable,DATA_TYPE as Xtype_name,case when COLUMN_KEY='PRI' then 1 else 0 end as pk,COLUMN_COMMENT as description,
case when EXTRA='auto_increment' then '1' else 0 END as IsIdentity from information_schema.columns where  table_name = '" + this._TableName + @"'";

                MySqlConnection STRUCTConn = new MySqlConnection(this._AdminConnectionString);
                MySqlCommand STRUCTCmd = new MySqlCommand(sql_GetStruct, STRUCTConn);
                MySqlDataAdapter STRUCTAdp = new MySqlDataAdapter(STRUCTCmd);
                DataTable SDT = new DataTable();
                STRUCTAdp.Fill(SDT);
                STRUCTConn.Close();
                STRUCTConn.Dispose();

                if (SDT != null && SDT.Rows.Count > 0)
                {
                    this.CheckAndCreateDir(this._ModelFolder);
                    //创建数据实体类
                    this.Create_Class(SDT, this._TableName, this._ModelFolder + "\\" + this._TableName + ".cs", this._ModelNameSpace, this._ModelFolder);
                }
            }
        }
        /// <summary>
        /// 创建实体文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <param name="aimFile"></param>
        /// <param name="namespaceHead"></param>
        /// <param name="classfolderHead"></param>
        private void Create_Class(DataTable dt, string tableName, string aimFile, string namespaceHead, string classfolderHead)
        {
            DBS_CCClass DBSCCC = new DBS_CCClass(dt, namespaceHead, classfolderHead, tableName, _ConnectionMark);
            string fileContent = DBSCCC.CreateFileContent();
            CheckAndWriteContent(aimFile, fileContent);
        }
        /// <summary>
        /// 检测目录，如不存在则创建
        /// </summary>
        /// <param name="Dir"></param>
        private void CheckAndCreateDir(string Dir)
        {
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
        }
        /// <summary>
        /// 检查并创建文件内容
        /// </summary>
        /// <param name="AimFile"></param>
        /// <param name="Content"></param>
        private void CheckAndWriteContent(string AimFile, string Content)
        {
            FileStream fs = null;
            if (!File.Exists(AimFile))
            {
                fs = File.Create(AimFile);
            }
            else
            {
                File.Delete(AimFile);
                fs = new FileStream(AimFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            }

            StreamWriter swt = new StreamWriter(fs, Encoding.Default);

            swt.Write(Content);
            swt.Close();
            swt.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}