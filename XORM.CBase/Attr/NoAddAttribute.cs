using System;
using System.ComponentModel;

namespace XORM.CBase.Attr
{
    /// <summary>
    /// 标记特性:用于标记添加记录时不需要插入数据库的字段或数据库自行维护的字段，如自增列、版本列等
    /// </summary>
    [Description("标记特性，用于标记添加记录时不需要插入数据库的字段或数据库自行维护的字段，如自增列、版本列等")]
    public class NoAddAttribute : Attribute
    {
    }
}