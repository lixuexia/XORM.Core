using System;
using System.ComponentModel;

namespace XORM.CBase.Attr
{
    /// <summary>
    /// 标记特性:用于标记数据源配置节点名称，如：EBS
    /// </summary>
    [Description("标记特性，用于标记数据源配置节点名称，如：EBS")]
    public class DbSourceAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_DbConnMark">数据源配置节点名称，如：EBS</param>
        /// <param name="Provider">数据源类型,1代表SQLSERVER,2代表ORACLE,0代表OLEDB,3代表MYSQL</param>
        public DbSourceAttribute(string _DbConnMark, int Provider = 1)
        {
            DbConnectionMark = _DbConnMark;
            DbProvider = Provider;
        }
        /// <summary>
        /// 数据源配置节点名称，如：EBS
        /// </summary>
        public string DbConnectionMark { get; private set; }
        /// <summary>
        /// 数据源类型,1代表SQLSERVER,2代表ORACLE,0代表OLEDB,3代表MYSQL
        /// </summary>
        public int DbProvider { get; set; }
    }
}