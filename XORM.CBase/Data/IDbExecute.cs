using System.Data;
using System.Data.Common;
using XORM.CBase.Data.Common;

namespace XORM.CBase.Data
{
    internal interface IDbExecute
    {
        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        bool Initialize(string ConnectionString);
        #endregion

        #region 执行SQL语句,返回第一行第一列数据
        /// <summary>
        /// 执行SQL语句,返回第一行第一列数据
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        object ExecTextScalar(string SQLText);
        /// <summary>
        /// 执行SQL语句,返回第一行第一列数据
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        object ExecTextScalar(string SQLText, params object[] cmdParams);
        #endregion

        #region 执行存储过程,返回第一行第一列数据
        /// <summary>
        /// 执行存储过程,返回第一行第一列数据
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <returns></returns>
        object ExecProcScalar(string ProcName);
        /// <summary>
        /// 执行存储过程,返回第一行第一列数据
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        object ExecProcScalar(string ProcName, params object[] cmdParams);
        #endregion

        #region 执行SQL语句,返回受影响行数
        /// <summary>
        /// 执行SQL语句,返回受影响行数
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        int ExecTextNonQuery(string SQLText);
        /// <summary>
        /// 执行SQL语句,返回受影响行数
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        int ExecTextNonQuery(string SQLText, params object[] cmdParams);
        #endregion

        #region 执行存储过程,返回受影响行数
        /// <summary>
        /// 执行存储过程,返回受影响行数
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <returns></returns>
        int ExecProcNonQuery(string ProcName);
        /// <summary>
        /// 执行存储过程,返回受影响行数
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        int ExecProcNonQuery(string ProcName, params object[] cmdParams);
        #endregion

        #region 执行SQL语句,返回DataSet
        /// <summary>
        /// 执行SQL语句,返回DataSet
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        DataSet ExecTextDataSet(string SQLText);
        /// <summary>
        /// 执行SQL语句,返回DataSet
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        DataSet ExecTextDataSet(string SQLText, params object[] cmdParams);
        #endregion

        #region 执行存储过程,返回DataSet
        /// <summary>
        /// 执行存储过程,返回DataSet
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <returns></returns>
        DataSet ExecProcDataSet(string ProcName);
        /// <summary>
        /// 执行存储过程,返回DataSet
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        DataSet ExecProcDataSet(string ProcName, params object[] cmdParams);
        #endregion

        #region 执行SQL语句,返回DataTable
        /// <summary>
        /// 执行SQL语句,返回DataTable
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns></returns>
        XORM.CBase.Data.Common.DataTable ExecTextDataTable(string SQLText);
        /// <summary>
        /// 执行SQL语句,返回DataTable
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        XORM.CBase.Data.Common.DataTable ExecTextDataTable(string SQLText, params object[] cmdParams);
        #endregion

        #region 执行存储过程,返回DataTable
        /// <summary>
        /// 执行存储过程,返回DataTable
        /// </summary>
        /// <param name="ProcName">存储过程名称</param>
        /// <returns></returns>
        XORM.CBase.Data.Common.DataTable ExecProcDataTable(string ProcName);
        /// <summary>
        /// 执行存储过程,返回DataTable
        /// </summary>
        /// <param name="ProcName">存储过程</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns></returns>
        XORM.CBase.Data.Common.DataTable ExecProcDataTable(string ProcName, params object[] cmdParams);
        #endregion

        #region 执行SQL命令,返回第一行第一列数据
        /// <summary>
        /// 执行SQL命令,返回第一行第一列数据
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        object ExecScalar(DbCommand cmd);
        object ExecScalar(string cmdText);
        #endregion

        #region 执行SQL命令,返回受影响行数
        /// <summary>
        /// 执行SQL命令,返回受影响行数
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        int ExecNonQuery(DbCommand cmd);
        #endregion

        #region 执行SQL命令,返回DataTable
        /// <summary>
        /// 执行SQL命令,返回DataTable
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        XORM.CBase.Data.Common.DataTable ExecDataTable(DbCommand cmd);
        #endregion

        #region 执行SQL命令,返回DataSet
        /// <summary>
        /// 执行SQL命令,返回DataSet
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        DataSet ExecDataSet(DbCommand cmd);
        #endregion

        #region 执行SQL命令,返回DbDataReader
        /// <summary>
        /// 执行SQL命令,返回DbDataReader
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <returns></returns>
        DbDataReader ExecDataReader(string cmdText);

        DbDataReader ExecDataReader(string cmdText, params object[] cmdParams);
        #endregion
    }
}