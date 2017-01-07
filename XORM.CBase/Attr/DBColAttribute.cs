using System;
using System.ComponentModel;

namespace XORM.CBase.Attr
{
    /// <summary>
    /// 标记属性:用于标识数据库字段
    /// </summary>
    [Description("标记属性:用于标识数据库字段")]
    public class DBColAttribute : Attribute
    {
        public DBColAttribute()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ColName">列名</param>
        /// <param name="_ColDbTypeName">列数据库类型</param>
        public DBColAttribute(string _ColName, string _ColDbTypeName)
        {
            this.ColName = _ColName;
            this.ColDbTypeName = _ColDbTypeName;
        }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColName
        {
            get; set;
        }
        /// <summary>
        /// 列数据库类型
        /// </summary>
        public string ColDbTypeName
        {
            get; set;
        }
    }
}