using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace XORM.CoreTool
{
    public class DBS_Control
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string _AdminConnectionString = string.Empty;
        /// <summary>
        /// 数据访问类命名空间头
        /// </summary>
        private string _DBONameSpace = string.Empty;
        /// <summary>
        /// 数据实体类命名空间头
        /// </summary>
        private string _ModelNameSpace = string.Empty;
        /// <summary>
        /// 数据访问类输出目录
        /// </summary>
        private string _DBOFolder = string.Empty;
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
        /// <summary>
        /// 是否默认使用只读源
        /// </summary>
        private bool _UseReadOnlyForSelect = true;

        public DBS_Control(string ConnStr, string NameSpaceHead_DBO, string FolderHead_DBO, string NameSpaceHead_DAT, string FolderHead_DAT, string TableName, string ConnectionMark, bool UseReadOnlyForSelect = true)
        {
            this._AdminConnectionString = ConnStr;
            this._DBONameSpace = NameSpaceHead_DBO;
            this._DBOFolder = FolderHead_DBO;
            this._ModelNameSpace = NameSpaceHead_DAT;
            this._ModelFolder = FolderHead_DAT;
            this._TableName = TableName;
            this._ConnectionMark = ConnectionMark;
            this._UseReadOnlyForSelect = UseReadOnlyForSelect;
        }

        public void CreateAll()
        {
            string sql_GetStruct =
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
                string RootFolder_DBO = this._DBOFolder;
                string RootFolder_DAT = this._ModelFolder;

                this.CheckAndCreateDir(RootFolder_DBO);
                this.CheckAndCreateDir(RootFolder_DAT);

                //创建数据访问类目录
                this.CheckAndCreateDir(RootFolder_DBO);
                //创建数据实体类
                this.Create_Class(SDT, this._TableName, RootFolder_DAT + "\\" + this._TableName + ".cs", this._ModelNameSpace, this._ModelFolder);

                //创建数据访问类
                this.Create_DBOper(SDT, this._TableName, RootFolder_DBO + "\\" + this._TableName + ".cs", this._DBONameSpace, this._DBOFolder, this._ModelNameSpace, this._ConnectionMark, this._UseReadOnlyForSelect);
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
        /// 创建代码访问文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <param name="aimFile"></param>
        /// <param name="namespaceHead_dbo"></param>
        /// <param name="dbofolderHead"></param>
        /// <param name="namespaceHead_class"></param>
        /// <param name="connectionConfigMark"></param>
        /// <param name="UseReadOnlyForSelect"></param>
        private void Create_DBOper(DataTable dt, string tableName, string aimFile, string namespaceHead_dbo, string dbofolderHead, string namespaceHead_class, string connectionConfigMark, bool UseReadOnlyForSelect)
        {
            DBS_CCDBOper DBSCCD = new DBS_CCDBOper(dt, namespaceHead_dbo, dbofolderHead, tableName, namespaceHead_class, connectionConfigMark, UseReadOnlyForSelect);
            string fileContent = DBSCCD.CreateFileContent();
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

            StreamWriter swt = new StreamWriter(fs, Encoding.UTF8);

            swt.Write(Content);
            swt.Close();
            swt.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}