namespace XORM.CTool
{
    /// <summary>
    /// 原子类型映射
    /// </summary>
    public class DBS_AtomType
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_DBType">数据库中类型</param>
        /// <param name="_CodeType">C#中变量类型，程序类型</param>
        /// <param name="_ConvertType">C#代码转换文本，ConvertTo</param>
        /// <param name="_SqlCommandType">数据库操作参数类型</param>
        public DBS_AtomType(string _DBType, string _CodeType, string _ConvertType, string _SqlCommandType)
        {
            this.DBType = _DBType;
            this.CodeType = _CodeType;
            this.ConvertType = _ConvertType;
            this.SqlCommandType = _SqlCommandType;
        }
        /// <summary>
        /// C#代码转换文本，ConvertTo
        /// </summary>
        public string ConvertType { get; set; }
        /// <summary>
        /// 程序类型
        /// </summary>
        public string CodeType { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }
        /// <summary>
        /// 数据库操作参数类型
        /// </summary>
        public string SqlCommandType { get; set; }
    }
}