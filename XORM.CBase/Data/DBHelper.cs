using System.Data;
using System.Data.Common;
using System;
using XORM.CBase.Data.Common;
using Microsoft.Extensions.Configuration;

namespace XORM.CBase.Data
{
    /// <summary>
    /// 数据通用访问类
    /// </summary>
    public class DBHelper
    {
        public static void SetConfigurationService(IConfiguration ConfigurationService)
        {
            if (Configuration == null)
            {
                Configuration = ConfigurationService;
            }
        }
        private static IConfiguration Configuration { get; set; }
        /// <summary>
        /// 数据库类型,1代表SQLSERVER,2代表ORACLE,0代表OLEDB,3代表MYSQL
        /// </summary>
        private int DBTypeMARK = 1;
        private string _ConnectionMark = "";

        #region 查询执行对象
        private IDbExecute DbExecutor
        {
            get
            {
                string ConnectionString = Configuration.GetConnectionString(_ConnectionMark);
                DBTypeMARK = 1;
                switch (DBTypeMARK)
                {
                    case 1:
                        {
                            IDbExecute _Executor = new SQLServerImpl();
                            _Executor.Initialize(ConnectionString);
                            return _Executor;
                        }
                    case 2:
                        {
                            IDbExecute _Executor = new MySQLServerImpl();
                            _Executor.Initialize(ConnectionString);
                            return _Executor;
                        }
                    default: return null;
                }
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ConnectionMark">链接字符串前缀</param>
        /// <param name="IsReadOnly">是否连接只读数据库</param>
        /// <param name="Provider">数据源类型,1代表SQLSERVER,2代表ORACLE,0代表OLEDB,3代表MYSQL</param>
        public DBHelper(string ConnectionMark, bool IsReadOnly = true, int Provider = 1)
        {
            this._ConnectionMark = ConnectionMark;
            this.DBTypeMARK = Provider;
            if (IsReadOnly)
            {
                this._ConnectionMark += "_READ";
            }
            else
            {
                this._ConnectionMark += "_WRITE";
            }
        }
        #endregion

        #region 执行SQL语句,返回第一行第一列数据
        /// <summary>
        /// 执行SQL语句,返回第一行第一列数据
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        public object ExecTextScalar(string SQLText)
        {
            try
            {
                return DbExecutor.ExecTextScalar(SQLText);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText);
                throw e;
            }
        }
        /// <summary>
        /// 执行SQL语句,返回第一行第一列数据
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public object ExecTextScalar(string SQLText, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecTextScalar(SQLText, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行存储过程,返回第一行第一列数据
        /// <summary>
        /// 执行存储过程,返回第一行第一列数据
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <returns></returns>
        public object ExecProcScalar(string ProcName)
        {
            try
            {
                return DbExecutor.ExecProcScalar(ProcName);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName);
                throw e;
            }
        }
        /// <summary>
        /// 执行存储过程,返回第一行第一列数据
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public object ExecProcScalar(string ProcName, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecProcScalar(ProcName, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行SQL语句,返回受影响行数
        /// <summary>
        /// 执行SQL语句,返回受影响行数
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        public int ExecTextNonQuery(string SQLText)
        {
            try
            {
                return DbExecutor.ExecTextNonQuery(SQLText);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText);
                throw e;
            }
        }
        /// <summary>
        /// 执行SQL语句,返回受影响行数
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public int ExecTextNonQuery(string SQLText, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecTextNonQuery(SQLText, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行存储过程,返回受影响行数
        /// <summary>
        /// 执行存储过程,返回受影响行数
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <returns></returns>
        public int ExecProcNonQuery(string ProcName)
        {
            try
            {
                return DbExecutor.ExecProcNonQuery(ProcName);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName);
                throw e;
            }
        }
        /// <summary>
        /// 执行存储过程,返回受影响行数
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public int ExecProcNonQuery(string ProcName, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecProcNonQuery(ProcName, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行SQL语句,返回DataSet
        /// <summary>
        /// 执行SQL语句,返回DataSet
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        public DataSet ExecTextDataSet(string SQLText)
        {
            try
            {
                return DbExecutor.ExecTextDataSet(SQLText);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText);
                throw e;
            }
        }
        /// <summary>
        /// 执行SQL语句,返回DataSet
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public DataSet ExecTextDataSet(string SQLText, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecTextDataSet(SQLText, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行存储过程,返回DataSet
        /// <summary>
        /// 执行存储过程,返回DataSet
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <returns></returns>
        public DataSet ExecProcDataSet(string ProcName)
        {
            try
            {
                return DbExecutor.ExecProcDataSet(ProcName);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName);
                throw e;
            }
        }
        /// <summary>
        /// 执行存储过程,返回DataSet
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public DataSet ExecProcDataSet(string ProcName, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecProcDataSet(ProcName, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行SQL语句,返回DataTable
        /// <summary>
        /// 执行SQL语句,返回DataTable
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        public XORM.CBase.Data.Common.DataTable ExecTextDataTable(string SQLText)
        {
            try
            {
                return DbExecutor.ExecTextDataTable(SQLText);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText);
                throw e;
            }
        }
        /// <summary>
        /// 执行SQL语句,返回DataTable
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public XORM.CBase.Data.Common.DataTable ExecTextDataTable(string SQLText, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecTextDataTable(SQLText, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, SQLText, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行存储过程,返回DataTable
        /// <summary>
        /// 执行存储过程,返回DataTable
        /// </summary>
        /// <param name="ProcName">存储过程名称</param>
        /// <returns></returns>
        public XORM.CBase.Data.Common.DataTable ExecProcDataTable(string ProcName)
        {
            try
            {
                return DbExecutor.ExecProcDataTable(ProcName);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName);
                throw e;
            }
        }
        /// <summary>
        /// 执行存储过程,返回DataTable
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        public XORM.CBase.Data.Common.DataTable ExecProcDataTable(string ProcName, params object[] cmdParams)
        {
            try
            {
                return DbExecutor.ExecProcDataTable(ProcName, cmdParams);
            }
            catch (Exception e)
            {
                log.Save(e, ProcName, cmdParams);
                throw e;
            }
        }
        #endregion

        #region 执行SQL命令,返回第一行第一列数据
        /// <summary>
        /// 执行SQL命令,返回第一行第一列数据
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        public object ExecScalar(DbCommand cmd)
        {
            try
            {
                return DbExecutor.ExecScalar(cmd);
            }
            catch (Exception e)
            {
                log.Save(e, cmd);
                throw e;
            }
        }
        public object ExecScalar(string cmdText)
        {
            try
            {
                return DbExecutor.ExecScalar(cmdText);
            }
            catch (Exception e)
            {
                log.Save(e, cmdText);
                throw e;
            }
        }
        #endregion

        #region 执行SQL命令,返回受影响行数
        /// <summary>
        /// 执行SQL命令,返回受影响行数
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        public int ExecNonQuery(DbCommand cmd)
        {
            try
            {
                return DbExecutor.ExecNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Save(e, cmd);
                throw e;
            }
        }
        #endregion

        #region 执行SQL命令,返回DataTable
        /// <summary>
        /// 执行SQL命令,返回DataTable
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        public XORM.CBase.Data.Common.DataTable ExecDataTable(DbCommand cmd)
        {
            try
            {
                return DbExecutor.ExecDataTable(cmd);
            }
            catch (Exception e)
            {
                log.Save(e, cmd);
                throw e;
            }
        }
        #endregion

        #region 执行SQL命令,返回DataSet
        /// <summary>
        /// 执行SQL命令,返回DataSet
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        public DataSet ExecDataSet(DbCommand cmd)
        {
            try
            {
                return DbExecutor.ExecDataSet(cmd);
            }
            catch (Exception e)
            {
                log.Save(e, cmd);
                throw e;
            }
        }
        #endregion

        #region 执行SQL命令,返回DataSet
        /// <summary>
        /// 执行SQL命令,返回SqlDataReader
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        public DbDataReader ExecDataReader(string cmdText)
        {
            try
            {
                return DbExecutor.ExecDataReader(cmdText);
            }
            catch (Exception e)
            {
                log.Save(e, cmdText);
                throw e;
            }
        }
        #endregion
        private Loger log = new Loger();
    }
}