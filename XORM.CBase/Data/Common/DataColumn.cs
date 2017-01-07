namespace XORM.CBase.Data.Common
{
    /// <summary>
    /// 数据库列
    /// </summary>
    public class DataColumn
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataColumn()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="columnName">列名</param>
        public DataColumn(string columnName)
        {
            this.ColumnName = columnName;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="dataType">列类型</param>
        public DataColumn(string columnName, object dataType)
        {
            this.ColumnName = columnName;
            this.DataType = dataType;
        }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; internal set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public object DataType { get; internal set; }
    }
}