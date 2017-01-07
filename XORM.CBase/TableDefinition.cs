using System.Collections.Generic;
using System.Reflection;

namespace XORM.CBase
{
    internal class TableDefinition
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string ORMTableName { get; set; }
        /// <summary>
        /// 数据库表字段列表
        /// </summary>
        public List<string> ORMColList { get; set; }
        /// <summary>
        /// 实体类型名
        /// </summary>
        public string ORMTypeName { get; set; }
        /// <summary>
        /// 实体类所在程序集
        /// </summary>
        public string ORMAssemblyName { get; set; }
        /// <summary>
        /// 数据库链接标记
        /// </summary>
        public string ORMConnectionMark { get; set; }
        /// <summary>
        /// 数据库类型，1SQLServer，2MySQL，3Oracle
        /// </summary>
        public string ORMConnectionProvider { get; set; }
        /// <summary>
        /// 类型属性字典
        /// </summary>
        public Dictionary<string,PropertyInfo> ORM_TypePropDic { get; set; }
        /// <summary>
        /// 数据库系统自动维护的列列表，如自增列，版本列等
        /// </summary>
        public List<string> ORM_NoAddCols { get; set; }
        /// <summary>
        /// 自增列名
        /// </summary>
        public string ORM_AutoIncreaseColName { get; set; }
        /// <summary>
        /// 数据库主键列表
        /// </summary>
        public List<string> ORM_PrimaryKeys { get; set; }
        /// <summary>
        /// 大写属性列表与真实属性名字典
        /// </summary>
        public Dictionary<string, string> ORM_RealColumnName = new Dictionary<string, string>();
    }
}