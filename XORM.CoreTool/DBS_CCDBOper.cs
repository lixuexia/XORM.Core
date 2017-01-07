using System;
using System.Text;
using System.Data;

namespace XORM.CoreTool
{
    public class DBS_CCDBOper
    {
        #region 变量
        /// <summary>
        /// 命名控件头,如:Vending.Data.DBOper
        /// </summary>
        private string _namespaceHead = string.Empty;
        /// <summary>
        /// 文件根目录,如:D:/Vending.Data.DBOper
        /// </summary>
        private string _folderHead = string.Empty;
        /// <summary>
        /// 表名
        /// </summary>
        private string _tableName = string.Empty;
        /// <summary>
        /// 表结构数据表
        /// </summary>
        private DataTable DT = null;
        /// <summary>
        /// 实体类命名空间头
        /// </summary>
        private string _NamespaceClasses = string.Empty;
        /// <summary>
        /// 查询请求是否采用只读字符串
        /// </summary>
        private bool _UseReadOnlyForSelect = true;
        /// <summary>
        /// 函数定义是否只读标记
        /// </summary>
        private string URFS = "true";
        /// <summary>
        /// 数据访问类命名空间
        /// </summary>
        private string _NamespaceDbu = "XORM.Db";
        /// <summary>
        /// 链接字符串前缀
        /// </summary>
        private string _DbConnectionConfigMark = "";
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dt">表结构</param>
        /// <param name="NameSpaceHead">命名控件头,如:CECC.DatasInfo</param>
        /// <param name="FolderHead">文件根目录,如:D:/CECC.DatasInfo</param>
        /// <param name="TableName">表名</param>
        /// <param name="NamespaceClasses">实体类命名空间头</param>
        /// <param name="UseReadOnlyForSelect">是否默认使用只读数据源</param>
        public DBS_CCDBOper(DataTable dt, string NameSpaceHead, string FolderHead, string TableName, string NamespaceClasses, string DbConnectionConfigMark, bool UseReadOnlyForSelect = true)
        {
            this.DT = dt;
            this._namespaceHead = NameSpaceHead.TrimEnd('.');
            this._folderHead = FolderHead;
            this._tableName = TableName;
            this._NamespaceClasses = NamespaceClasses;
            this._UseReadOnlyForSelect = UseReadOnlyForSelect;
            if (this._UseReadOnlyForSelect)
            {
                URFS = "true";
            }
            else
            {
                URFS = "false";
            }
            this._DbConnectionConfigMark = DbConnectionConfigMark;
        }
        #endregion

        #region 创建文件内容
        /// <summary>
        /// 创建文件内容
        /// </summary>
        /// <returns></returns>
        public string CreateFileContent()
        {
            StringBuilder txt = new StringBuilder();
            txt.AppendLine("using System;");
            txt.AppendLine("using System.Collections.Generic;");
            txt.AppendLine("using System.Data.SqlClient;");
            txt.AppendLine("using System.Text;");
            txt.AppendLine("using System.Text.RegularExpressions;");
            txt.AppendLine("using XORM.Core;");
            txt.AppendLine("");
            txt.AppendLine("namespace " + this._namespaceHead);
            txt.AppendLine("{");
            txt.AppendLine("\t/// <summary>");
            txt.AppendLine("\t/// 数据库操作:" + this._tableName);
            txt.AppendLine("\t/// </summary>");
            txt.AppendLine("\tpublic class " + this._tableName);
            txt.AppendLine("\t{");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 默认查询是否用只读串");
            txt.AppendLine("\t\t/// </summary>");
            if (_UseReadOnlyForSelect)
            {
                txt.AppendLine("\t\tpublic static bool ReadOnlyDataSource = true;");
            }
            else
            {
                txt.AppendLine("\t\tpublic static bool ReadOnlyDataSource = false;");
            }
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 数据库链接配置前缀");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\tpublic static string ConnectionConfigMark = \""+ this._DbConnectionConfigMark + "\";");

            txt.AppendLine(GetSelectContent());
            txt.AppendLine(GetDeleteContent());
            txt.AppendLine(GetUpdateContent());
            txt.AppendLine(GetInsertContent());
            txt.AppendLine(GetParametersContent());
            txt.AppendLine("\t}");
            txt.Append("}");
            return txt.ToString();
        }
        #endregion

        #region 查询语句
        /// <summary>
        /// 查询语句
        /// </summary>
        /// <returns></returns>
        private string GetSelectContent()
        {
            StringBuilder txt = new StringBuilder();

            txt.AppendLine("\t\t#region 查询");
            #region 查询：返回数据表
            txt.AppendLine("\t\t#region 查询：返回数据表");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询：返回数据表");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，可带参数，如username='123' and usertype=@usertype</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">需要查询的字段列表,如:ID,Name,CreateTime,默认*或空标识全部字段</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static XORM.Core.DataTable GetTable(string sqlWhere = \"\", string sqlSort = \"\", string sqlCols = \"*\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = new XORM.Core.DataTable();");
            txt.AppendLine("\t\t\tstring txtCols = sqlCols;");
            txt.AppendLine("\t\t\tif (string.IsNullOrEmpty(sqlCols))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\ttxtCols = \"*\";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"SELECT \" + sqlCols + \" FROM [" + this._tableName + "](NOLOCK)\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" WHERE \").Append(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlSort))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" ORDER BY \").Append(sqlSort);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\ttable = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecDataTable(cmd);");
            txt.AppendLine("\t\t\tif (table == null)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\ttable = new XORM.Core.DataTable();");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\ttable.TableName = \"table\";");
            txt.AppendLine("\t\t\treturn table;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            #endregion
            txt.AppendLine("");

            #region 查询：内分组-返回列表
            txt.AppendLine("\t\t#region 内分组查询");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询：内分组查询数据");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionWhere\">内分组条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionSort\">内分组排序条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionBy\">内分组字段</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionRange\">内分组筛选行号</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，可带参数，如username='123' and usertype=@usertype</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">需要查询的字段列表,如:ID,Name,CreateTime,默认*或空标识全部字段</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <return></return>");
            txt.AppendLine("\t\tpublic static List<T> GetInnerSortList<T>(string sqlPartitionWhere = \"\", string sqlPartitionSort = \"\", string sqlPartitionBy = \"\", List<int> sqlPartitionRange = null, string sqlWhere = \"\", string sqlSort = \"\", string sqlCols = \"*\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetInnerSortTable(sqlPartitionWhere, sqlPartitionSort, sqlPartitionBy, sqlPartitionRange, sqlWhere, sqlSort, sqlCols, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\tList<T> rtnObjList = new List<T>();");
            txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\trtnObjList.Add((T)System.Activator.CreateInstance(typeof(T), dr));");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn rtnObjList;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询：内分组查询数据");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionWhere\">内分组条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionSort\">内分组排序条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionBy\">内分组字段</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionRange\">内分组筛选行号</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，可带参数，如username='123' and usertype=@usertype</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">需要查询的字段列表,如:ID,Name,CreateTime,默认*或空标识全部字段</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <return></return>");
            txt.AppendLine("\t\tpublic static List<" + this._NamespaceClasses + this._tableName + "> GetInnerSortList(string sqlPartitionWhere = \"\", string sqlPartitionSort = \"\", string sqlPartitionBy = \"\", List<int> sqlPartitionRange = null, string sqlWhere = \"\", string sqlSort = \"\", string sqlCols = \"*\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetInnerSortTable(sqlPartitionWhere, sqlPartitionSort, sqlPartitionBy, sqlPartitionRange, sqlWhere, sqlSort, sqlCols, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\tList<" + this._NamespaceClasses + this._tableName + "> rtnObjList = new List<" + this._NamespaceClasses + this._tableName + ">();");
            txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\trtnObjList.Add(new " + this._NamespaceClasses + this._tableName + "(dr));");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn rtnObjList;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            txt.AppendLine("\t\t#region 查询：内分组查询-返回数据表");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询：内分组查询-返回数据表");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionWhere\">内分组条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionSort\">内分组排序条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionBy\">内分组字段</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlPartitionRange\">内分组筛选行号</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，可带参数，如username='123' and usertype=@usertype</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">需要查询的字段列表,如:ID,Name,CreateTime,默认*或空标识全部字段</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static XORM.Core.DataTable GetInnerSortTable(string sqlPartitionWhere = \"\", string sqlPartitionSort = \"\", string sqlPartitionBy = \"\", List<int> sqlPartitionRange = null, string sqlWhere = \"\", string sqlSort = \"\", string sqlCols = \"*\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = new XORM.Core.DataTable();");
            txt.AppendLine("\t\t\tstring txtCols = sqlCols;");
            txt.AppendLine("\t\t\tif (string.IsNullOrEmpty(sqlCols))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\ttxtCols = \"*\";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder();");
            txt.AppendLine("\t\t\tsql.AppendLine(\"SELECT * FROM \");");
            txt.AppendLine("\t\t\tsql.AppendLine(\"(\");");
            txt.AppendLine("\t\t\tsql.Append(\"SELECT ROW_NUMBER() OVER(PARTITION BY \").Append(sqlPartitionBy).Append(\" ORDER BY \").Append(sqlPartitionSort).Append(\") AS RN,\").Append(txtCols).AppendLine(\" FROM [" + this._tableName + "](NOLOCK)\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlPartitionWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" WHERE \").AppendLine(sqlPartitionWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tsql.AppendLine(\") AS TempTAB\");");
            txt.AppendLine("\t\t\tif (sqlPartitionRange == null || sqlPartitionRange.Count <= 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsqlPartitionRange = new List<int>(){ 1 };");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tsql.Append(\"WHERE RN IN (\").Append(string.Join(\",\", sqlPartitionRange)).AppendLine(\")\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" AND \").AppendLine(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlSort))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" ORDER BY \").AppendLine(sqlSort);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\ttable = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecDataTable(cmd);");
            txt.AppendLine("\t\t\tif (table == null)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\ttable = new XORM.Core.DataTable();");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\ttable.TableName = \"table\";");
            txt.AppendLine("\t\t\treturn table;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            #endregion

            #region 查询：返回对象列表
            txt.AppendLine("\t\t#region 查询：返回对象列表");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询：返回对象列表");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and usertype=@usertype或distinct userid,username</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">需要查询的字段列表,如:ID,Name,CreateTime,默认*或空标识全部字段</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static List<T> GetList<T>(string sqlWhere = \"\", string sqlSort = \"\", string sqlCols = \"*\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetTable(sqlWhere, sqlSort, sqlCols, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\tList<T> rtnObjList = new List<T>();");
            txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\trtnObjList.Add((T)System.Activator.CreateInstance(typeof(T), dr));");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn rtnObjList;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            txt.AppendLine("\t\t#region 查询：返回对象列表");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询：返回对象列表");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and usertype=@usertype</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static List<" + this._NamespaceClasses + this._tableName + "> GetList(string sqlWhere = \"\", string sqlSort = \"\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetTable(sqlWhere, sqlSort, \"*\", ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\tList<" + this._NamespaceClasses + this._tableName + "> rtnObjList = new List<" + this._NamespaceClasses + this._tableName + ">();");
            txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\trtnObjList.Add(new " + this._NamespaceClasses + this._tableName + "(dr));");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn rtnObjList;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            txt.AppendLine("\t\t#region 分页查询");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 分页查询");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序条件,如username desc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">数据库字段名组,用逗号分割,例如:username,userpwd,userid</param>");
            txt.AppendLine("\t\t/// <param name=\"pageIndex\">页码</param>");
            txt.AppendLine("\t\t/// <param name=\"pageSize\">页大小</param>");
            txt.AppendLine("\t\t/// <param name=\"recordCount\">记录行数</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static List<" + this._NamespaceClasses + this._tableName + "> GetList(string sqlWhere, string sqlSort, string sqlCols, int pageIndex, int pageSize, out int recordCount, object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tlong LRecordCount = 0;");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetTable(sqlWhere, sqlSort, sqlCols, pageIndex, pageSize, out LRecordCount, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\trecordCount = Convert.ToInt32(LRecordCount);");
            txt.AppendLine("\t\t\tList<" + this._NamespaceClasses + this._tableName + "> rtnObjList = new List<" + this._NamespaceClasses + this._tableName + ">();");
            txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\trtnObjList.Add(new " + this._NamespaceClasses + this._tableName + "(dr));");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn rtnObjList;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            txt.AppendLine("\t\t#region 分页查询");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 分页查询");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序条件,如username desc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">数据库字段名组,用逗号分割,例如:username,userpwd,userid</param>");
            txt.AppendLine("\t\t/// <param name=\"pageIndex\">页码</param>");
            txt.AppendLine("\t\t/// <param name=\"pageSize\">页大小</param>");
            txt.AppendLine("\t\t/// <param name=\"recordCount\">记录行数</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static List<" + this._NamespaceClasses + this._tableName + "> GetList(string sqlWhere, string sqlSort, string sqlCols, int pageIndex, int pageSize, out long recordCount, object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tlong LRecordCount = 0;");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetTable(sqlWhere, sqlSort, sqlCols, pageIndex, pageSize, out LRecordCount, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\trecordCount = Convert.ToInt64(LRecordCount);");
            txt.AppendLine("\t\t\tList<" + this._NamespaceClasses + this._tableName + "> rtnObjList = new List<" + this._NamespaceClasses + this._tableName + ">();");
            txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\trtnObjList.Add(new " + this._NamespaceClasses + this._tableName + "(dr));");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn rtnObjList;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            #endregion
            txt.AppendLine("");
            #region 查询：根据单个主键列
            DataRow[] PKDRS = DT.Select("PK=1");
            DBS_TypeMap TM = new DBS_TypeMap();
            if (PKDRS != null && PKDRS.Length > 0)
            {
                for (int i = 0; i < PKDRS.Length; i++)
                {
                    DataRow dr = PKDRS[i];
                    txt.AppendLine("\t\t#region 根据主键:" + dr["name"].ToString() + " 查询");
                    txt.AppendLine("\t\t/// <summary>");
                    txt.AppendLine("\t\t/// 根据主键:" + dr["name"].ToString() + " 查询");
                    txt.AppendLine("\t\t/// </summary>");
                    txt.AppendLine("\t\t/// <param name=\"" + dr["name"].ToString() + "\">" + dr["Description"].ToString() + "</param>");
                    txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
                    txt.AppendLine("\t\t/// <returns></returns>");
                    txt.AppendLine("\t\tpublic static XORM.Core.DataTable GetTableBy" + dr["name"].ToString() + "(" + TM[dr["Xtype_Name"].ToString()].CodeType + " _" + dr["name"].ToString() + ", bool ReadOnlyDataSource = " + URFS + ")");
                    txt.AppendLine("\t\t{");
                    txt.AppendLine("\t\t\tXORM.Core.DataTable table = new XORM.Core.DataTable();");
                    txt.AppendLine("\t\t\tstring sql = \"SELECT * FROM " + this._tableName + "(NOLOCK) WHERE [" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\";");
                    txt.AppendLine("\t\t\tSqlCommand cmd = new SqlCommand();");
                    txt.AppendLine("\t\t\tcmd.CommandText = sql;");
                    txt.AppendLine("\t\t\tcmd.Parameters.Add(\"@" + dr["name"].ToString() + "\", " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ").Value = _" + dr["name"].ToString() + ";");
                    txt.AppendLine("\t\t\ttable = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecDataTable(cmd);");
                    txt.AppendLine("\t\t\tif (table == null)");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\ttable = new XORM.Core.DataTable();");
                    txt.AppendLine("\t\t\t}");
                    txt.AppendLine("\t\t\ttable.TableName = \"table\";");
                    txt.AppendLine("\t\t\treturn table;");
                    txt.AppendLine("\t\t}");
                    txt.AppendLine("\t\t#endregion");
                    txt.AppendLine("");
                }
            }

            if (PKDRS != null && PKDRS.Length > 0)
            {
                for (int i = 0; i < PKDRS.Length; i++)
                {
                    DataRow dr = PKDRS[i];
                    txt.AppendLine("\t\t#region 根据主键:" + dr["name"].ToString() + " 查询，返回对象列表");
                    txt.AppendLine("\t\t/// <summary>");
                    txt.AppendLine("\t\t/// 根据主键:" + dr["name"].ToString() + " 查询，返回对象列表");
                    txt.AppendLine("\t\t/// </summary>");
                    txt.AppendLine("\t\t/// <param name=\"" + dr["name"].ToString() + "\">" + dr["Description"].ToString() + "</param>");
                    txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
                    txt.AppendLine("\t\t/// <returns></returns>");
                    txt.AppendLine("\t\tpublic static List<" + this._NamespaceClasses + this._tableName + "> GetListBy" + dr["name"].ToString() + "(" + TM[dr["Xtype_Name"].ToString()].CodeType + " _" + dr["name"].ToString() + ", bool ReadOnlyDataSource = " + URFS + ")");
                    txt.AppendLine("\t\t{");
                    txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetTableBy" + dr["name"].ToString() + "(_" + dr["name"].ToString() + ", ReadOnlyDataSource);");
                    txt.AppendLine("\t\t\tList<" + this._NamespaceClasses + this._tableName + "> objList = new List<" + this._NamespaceClasses + this._tableName + ">();");
                    txt.AppendLine("\t\t\tif (table != null && table.Rows.Count > 0)");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tforeach (DataRow dr in table.Rows)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tobjList.Add(new " + this._NamespaceClasses + this._tableName + "(dr));");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t}");
                    txt.AppendLine("\t\t\treturn objList;");
                    txt.AppendLine("\t\t}");
                    txt.AppendLine("\t\t#endregion");
                    txt.AppendLine("");
                }
            }
            #endregion
            
            #region 查询：条件查询,查询某字段数据第一行第一列数据
            txt.AppendLine("\t\t#region 查询某字段数据第一行第一列数据");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 条件查询,查询某字段数据第一行第一列数据");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlCol\">数据库字段名,也可以为COUNT(),TOP 1 列名</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123' and usertype=@usertype</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序，如 date desc,id asc</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static object GetSingle(string sqlCol, string sqlWhere = \"\", string sqlSort = \"\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"SELECT TOP 1 \" + sqlCol + \" FROM " + this._tableName + "(NOLOCK)\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" WHERE \").Append(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlSort))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" ORDER BY \").Append(sqlSort);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\treturn new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecScalar(cmd);");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            #endregion

            #region 查询:记录总数
            txt.AppendLine("\t\t#region 条件查询,查询记录总数");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询记录总数");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123',并可带排序条件</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static long CountOutLong(string sqlWhere, object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"SELECT COUNT(1) FROM " + this._tableName + "(NOLOCK) WHERE 1=1\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" AND \").Append(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\tobject CountObj = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecScalar(cmd);");
            txt.AppendLine("\t\t\tlong RtnCount = 0;");
            txt.AppendLine("\t\t\tif (CountObj != null && CountObj != DBNull.Value && !string.IsNullOrEmpty(CountObj.ToString()))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tlong.TryParse(CountObj.ToString(), out RtnCount);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn RtnCount;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            txt.AppendLine("\t\t#region 条件查询,查询记录总数");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 条件查询,查询记录总数");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123',并可带排序条件</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static int Count(string sqlWhere, object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tlong rtnInt= CountOutLong(sqlWhere, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\treturn Convert.ToInt32(rtnInt);");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("");
            #endregion

            #region 查询：分页查询
            string drText = "";
            foreach (DataRow dr in this.DT.Rows)
            {
                if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(drText))
                {
                    drText = dr["name"].ToString();
                }
                else
                {
                    drText += "," + dr["name"].ToString();
                }
            }
            txt.AppendLine("\t\t#region 分页查询");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 分页查询");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序条件,如username desc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">数据库字段名组,用逗号分割,例如:username,userpwd,userid</param>");
            txt.AppendLine("\t\t/// <param name=\"pageIndex\">页码</param>");
            txt.AppendLine("\t\t/// <param name=\"pageSize\">页大小</param>");
            txt.AppendLine("\t\t/// <param name=\"recordCount\">记录行数</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static XORM.Core.DataTable GetTable(string sqlWhere, string sqlSort, string sqlCols, int pageIndex, int pageSize, out int recordCount, object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tlong LRecordCount = 0;");
            txt.AppendLine("\t\t\tXORM.Core.DataTable table = GetTable(sqlWhere, sqlSort, sqlCols, pageIndex, pageSize, out LRecordCount, ParamsList, ReadOnlyDataSource);");
            txt.AppendLine("\t\t\trecordCount = Convert.ToInt32(LRecordCount);");
            txt.AppendLine("\t\t\treturn table;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            #endregion
            txt.AppendLine("");
            #region 查询：分页查询
            txt.AppendLine("\t\t#region 分页查询");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 分页查询");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序条件,如username desc</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlCols\">数据库字段名组,用逗号分割,例如:username,userpwd,userid</param>");
            txt.AppendLine("\t\t/// <param name=\"pageIndex\">页码</param>");
            txt.AppendLine("\t\t/// <param name=\"pageSize\">页大小</param>");
            txt.AppendLine("\t\t/// <param name=\"recordCount\">记录行数</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static XORM.Core.DataTable GetTable(string sqlWhere, string sqlSort, string sqlCols, int pageIndex, int pageSize, out long recordCount, object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tint SI = pageIndex * pageSize - pageSize + 1;");
            txt.AppendLine("\t\t\tint EI = pageIndex * pageSize;");
            txt.AppendLine("\t\t\tDataSet ds = new DataSet();");
            txt.AppendLine("\t\t\tif (string.IsNullOrEmpty(sqlCols) || sqlCols == \"*\")");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsqlCols = \"" + drText + "\";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"WITH PST(RN,\" + sqlCols + \") AS\");");
            txt.AppendLine("\t\t\tsql.Append(\"(\");");
            txt.AppendLine("\t\t\tsql.Append(\"SELECT ROW_NUMBER() OVER(ORDER BY \").Append(sqlSort).Append(\") RN,\").Append(sqlCols).Append(\" \");");
            txt.AppendLine("\t\t\tsql.Append(\"FROM " + this._tableName + "(NOLOCK)\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" WHERE \").Append(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tsql.Append(\")\");");
            txt.AppendLine("\t\t\tsql.Append(\"SELECT RN,\").Append(sqlCols).Append(\" FROM PST WHERE RN BETWEEN @SI AND @EI\");");
            txt.AppendLine("\t\t\tobject[] NewParamsList;");
            txt.AppendLine("\t\t\tif (ParamsList != null)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tNewParamsList = new object[ParamsList.Length + 2];");
            txt.AppendLine("\t\t\t\tfor (int i = 0;i < ParamsList.Length; i++)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\tNewParamsList[i] = ParamsList[i];");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t\tNewParamsList[ParamsList.Length] = SI;");
            txt.AppendLine("\t\t\t\tNewParamsList[ParamsList.Length + 1] = EI;");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tNewParamsList = new object[2];");
            txt.AppendLine("\t\t\t\tNewParamsList[0] = SI;");
            txt.AppendLine("\t\t\t\tNewParamsList[1] = EI;");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\";SELECT COUNT(1) FROM " + this._tableName + "(NOLOCK) WHERE \").Append(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\";SELECT COUNT(1) FROM " + this._tableName + "(NOLOCK)\");");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), NewParamsList);");
            txt.AppendLine("\t\t\tds = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecDataSet(cmd);");
            txt.AppendLine("\t\t\tif (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count >0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\trecordCount = Convert.ToInt64(ds.Tables[1].Rows[0][0]);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\trecordCount = 0;");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tif (ds != null && ds.Tables.Count >1)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn ds.Tables[0];");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn new XORM.Core.DataTable();");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            #endregion
            txt.AppendLine("");
            #region 查询：查询并构建对象,返回第一个对象
            txt.AppendLine("\t\t#region 查询并构建对象,返回第一个对象");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询并构建对象,返回第一个对象");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSort\">排序条件</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">参数值列表，需与sqlWhere中顺序对应，相同参数只提供一次，如 {\"123\",1}</param>");
            txt.AppendLine("\t\t/// <param name=\"ReadOnlyDataSource\">是否使用只读数据源</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static " + this._NamespaceClasses + this._tableName + " Get(string sqlWhere, string sqlSort = \"\", object[] ParamsList = null, bool ReadOnlyDataSource = " + URFS + ")");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"SELECT TOP 1 * FROM " + this._tableName + "(NOLOCK)\");");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlWhere))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" WHERE \").Append(sqlWhere);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sqlSort))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql.Append(\" ORDER BY \").Append(sqlSort);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\tXORM.Core.DataTable DT = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, ReadOnlyDataSource).ExecDataTable(cmd);");
            txt.AppendLine("\t\t\tif(DT != null && DT.Rows.Count > 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn new " + this._NamespaceClasses + this._tableName + "(DT.Rows[0]);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn new " + this._NamespaceClasses + this._tableName + "();");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            #endregion
            txt.AppendLine("\t\t#endregion");
            return txt.ToString();
        }
        #endregion

        #region 删除语句
        /// <summary>
        /// 删除语句
        /// </summary>
        /// <returns></returns>
        private string GetDeleteContent()
        {
            StringBuilder txt = new StringBuilder();
            txt.AppendLine("\t\t#region 删除");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 删除数据，数据删除，Delete");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static int DBDelete(string sqlWhere, object[] ParamsList = null, bool IsRowLock = true)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tstring deleteSql = string.Empty;");
            txt.AppendLine("\t\t\tif (IsRowLock)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tdeleteSql = \"DELETE FROM " + this._tableName + " WITH(ROWLOCK) WHERE \";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tdeleteSql = \"DELETE FROM " + this._tableName + " WHERE \";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(deleteSql);");
            txt.AppendLine("\t\t\tsql.Append(sqlWhere);");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\treturn new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecNonQuery(cmd);");
            txt.AppendLine("\t\t}");
            txt.AppendLine("");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 删除数据，逻辑删除，IsDel=1");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static int Delete(string sqlWhere, object[] ParamsList = null)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"UPDATE " + this._tableName + " SET IsDel=1 WHERE \");");
            txt.AppendLine("\t\t\tsql.Append(sqlWhere);");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sql.ToString(), ParamsList);");
            txt.AppendLine("\t\t\treturn new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecNonQuery(cmd);");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            return txt.ToString();
        }
        #endregion

        #region 更新语句
        /// <summary>
        /// 更新语句
        /// </summary>
        /// <returns></returns>
        private string GetUpdateContent()
        {
            StringBuilder txt = new StringBuilder();
            txt.AppendLine("\t\t#region 更新");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 更新数据");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sqlWhere\">查询条件，如username='123' and password='123'</param>");
            txt.AppendLine("\t\t/// <param name=\"sqlSet\">数据设置，如username='123',password='123'</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static int Update(string sqlWhere, string sqlSet, object[] ParamsList = null)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tStringBuilder sql = new StringBuilder(\"UPDATE " + this._tableName + " SET \");");
            txt.AppendLine("\t\t\tsql.Append(sqlSet).Append(\" WHERE \").Append(sqlWhere);");
            txt.AppendLine("\t\t\tSqlCommand cmd = BuildCommand(sqlWhere + \" \" + sqlSet, ParamsList);");
            txt.AppendLine("\t\t\tcmd.CommandText = sql.ToString();");
            txt.AppendLine("\t\t\treturn new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecNonQuery(cmd);");
            txt.AppendLine("\t\t}");
            txt.AppendLine("");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 更新数据");
            txt.AppendLine("\t\t/// 返回：操作影响记录数，-1表示没有需要更新的列");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"" + this._tableName + "_obj" + "\"></param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static int Update(" + this._NamespaceClasses + this._tableName + " " + this._tableName + "_obj)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tif(" + this._tableName + "_obj.ModifiedColumns.Count <= 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn -1;");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tStringBuilder sqlSet = new StringBuilder();");
            txt.AppendLine("\t\t\tStringBuilder sqlWhere = new StringBuilder();");
            txt.AppendLine("\t\t\tstring sql = \"UPDATE " + this._tableName + " SET {0} WHERE {1}\";");
            txt.AppendLine("\t\t\tSqlCommand cmd = new SqlCommand();");
            DataRow[] PKDRS = this.DT.Select("PK=1");
            DBS_TypeMap TM = new DBS_TypeMap();

            #region 根据主键构建更新条件
            foreach (DataRow dr in PKDRS)
            {
                //uniqueidentifier类型在插入、更新时不做处理
                if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                {
                    continue;
                }
                txt.AppendLine("\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                txt.AppendLine("\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                txt.AppendLine("\t\t\tif (sqlWhere.Length > 0)");
                txt.AppendLine("\t\t\t{");
                txt.AppendLine("\t\t\t\tsqlWhere.Append(\" AND \");");
                txt.AppendLine("\t\t\t}");
                txt.AppendLine("\t\t\tsqlWhere.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
            }
            #endregion

            DataRow[] NPKDRS = this.DT.Select("PK<>1");

            #region 更新数据列
            foreach (DataRow dr in NPKDRS)
            {
                //自增列不可更新
                if (Convert.ToInt32(dr["isidentity"]) == 1)
                {
                    continue;
                }
                //uniqueidentifier类型在插入、更新时不做处理
                if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].CodeType == "DateTime")
                {
                    txt.AppendLine("\t\t\tif (" + this._tableName + "_obj." + dr["name"].ToString() + " != null && " + this._tableName + "_obj." + dr["name"].ToString() + " > DateTime.MinValue && " + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                    txt.AppendLine("\t\t\t}");
                }
                //else if (TM[dr["Xtype_Name"].ToString()].DBType.ToUpper() == "XML")
                //{
                //    txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + ") && " + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                //    txt.AppendLine("\t\t\t{");
                //    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                //    txt.AppendLine("\t\t\t\t{");
                //    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                //    txt.AppendLine("\t\t\t\t}");
                //    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                //    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + this._tableName + "_obj." + dr["name"].ToString() + ", XmlNodeType.Document, null)));");
                //    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                //    txt.AppendLine("\t\t\t}");
                //}
                else if (TM[dr["Xtype_Name"].ToString()].CodeType == "string")
                {
                    txt.AppendLine("\t\t\tif(" + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tif (string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                    txt.AppendLine("\t\t\t\t{");
                    if (string.IsNullOrEmpty(dr["dval"].ToString()))
                    {
                        txt.AppendLine("\t\t\t\t\t" + this._tableName + "_obj." + dr["name"].ToString() + " = \"\";");
                    }
                    else
                    {
                        txt.AppendLine("\t\t\t\t\t" + this._tableName + "_obj." + dr["name"].ToString() + " = \"" + dr["dval"].ToString().Replace("(","").Replace(")","").Replace("'","") + "\";");
                    }
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                    txt.AppendLine("\t\t\t}");
                }
                else
                {
                    txt.AppendLine("\t\t\tif(" + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                    txt.AppendLine("\t\t\t}");
                }
            }
            #endregion

            txt.AppendLine("\t\t\tsql = string.Format(sql, sqlSet.ToString(), sqlWhere.ToString());");
            txt.AppendLine("\t\t\tcmd.CommandText = sql;");
            txt.AppendLine("\t\t\ttry");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecNonQuery(cmd);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tcatch { return -1; }");
            txt.AppendLine("\t\t}");
            txt.AppendLine("");

            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 更新数据，行级数据锁定");
            txt.AppendLine("\t\t/// 返回：操作影响记录数，-1表示没有需要更新的列");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"" + this._tableName + "_obj" + "\"></param>");
            txt.AppendLine("\t\t/// <param name=\"IsRowLock\">是否锁行</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static int Update(" + this._NamespaceClasses + this._tableName + " " + this._tableName + "_obj, bool IsRowLock)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tif(" + this._tableName + "_obj.ModifiedColumns.Count <= 0)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn -1;");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tStringBuilder sqlSet = new StringBuilder();");
            txt.AppendLine("\t\t\tStringBuilder sqlWhere = new StringBuilder();");
            txt.AppendLine("\t\t\tstring sql = string.Empty;");
            txt.AppendLine("\t\t\tif (IsRowLock)");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql = \"UPDATE " + this._tableName + " WITH(ROWLOCK) SET {0} WHERE {1}\";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\telse");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tsql = \"UPDATE " + this._tableName + " SET {0} WHERE {1}\";");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tSqlCommand cmd = new SqlCommand();");
            DataRow[] PKDRSRL = this.DT.Select("PK=1");

            #region 根据主键构建更新条件
            foreach (DataRow dr in PKDRS)
            {
                //uniqueidentifier类型在插入、更新时不做处理
                if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                {
                    continue;
                }
                txt.AppendLine("\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                txt.AppendLine("\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                txt.AppendLine("\t\t\tif (sqlWhere.Length > 0)");
                txt.AppendLine("\t\t\t{");
                txt.AppendLine("\t\t\t\tsqlWhere.Append(\" AND \");");
                txt.AppendLine("\t\t\t}");
                txt.AppendLine("\t\t\tsqlWhere.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
            }
            #endregion

            DataRow[] NPKDRSRL = this.DT.Select("PK<>1");

            #region 更新数据列
            foreach (DataRow dr in NPKDRS)
            {
                //自增列不可更新
                if (Convert.ToInt32(dr["isidentity"]) == 1)
                {
                    continue;
                }
                //uniqueidentifier类型在插入、更新时不做处理
                if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].CodeType == "DateTime")
                {
                    txt.AppendLine("\t\t\tif (" + this._tableName + "_obj." + dr["name"].ToString() + " != null && " + this._tableName + "_obj." + dr["name"].ToString() + " > DateTime.MinValue && " + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                    txt.AppendLine("\t\t\t}");
                }
                //else if (TM[dr["Xtype_Name"].ToString()].DBType.ToUpper() == "XML")
                //{
                //    txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + ") && " + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                //    txt.AppendLine("\t\t\t{");
                //    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                //    txt.AppendLine("\t\t\t\t{");
                //    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                //    txt.AppendLine("\t\t\t\t}");
                //    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                //    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + this._tableName + "_obj." + dr["name"].ToString() + ", XmlNodeType.Document, null)));");
                //    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                //    txt.AppendLine("\t\t\t}");
                //}
                else if (TM[dr["Xtype_Name"].ToString()].CodeType == "string")
                {
                    txt.AppendLine("\t\t\tif(" + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tif (string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                    txt.AppendLine("\t\t\t\t{");
                    if (string.IsNullOrEmpty(dr["dval"].ToString()))
                    {
                        txt.AppendLine("\t\t\t\t\t" + this._tableName + "_obj." + dr["name"].ToString() + " = \"\";");
                    }
                    else
                    {
                        txt.AppendLine("\t\t\t\t\t" + this._tableName + "_obj." + dr["name"].ToString() + " = \"" + dr["dval"].ToString().Replace("(", "").Replace(")", "").Replace("'", "") + "\";");
                    }
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                    txt.AppendLine("\t\t\t}");
                }
                else
                {
                    txt.AppendLine("\t\t\tif(" + this._tableName + "_obj.ModifiedColumns.Contains(\"[" + dr["name"].ToString() + "]\"))");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tif (sqlSet.Length > 0)");
                    txt.AppendLine("\t\t\t\t{");
                    txt.AppendLine("\t\t\t\t\tsqlSet.Append(\",\");");
                    txt.AppendLine("\t\t\t\t}");
                    txt.AppendLine("\t\t\t\tsqlSet.Append(\"[" + dr["name"].ToString() + "]=@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    txt.AppendLine("\t\t\t\tcmd.Parameters[\"@" + dr["name"].ToString() + "\"].SqlDbType = " + TM[dr["Xtype_Name"].ToString()].SqlCommandType + ";");
                    txt.AppendLine("\t\t\t}");
                }
            }
            #endregion

            txt.AppendLine("\t\t\tsql = string.Format(sql, sqlSet.ToString(), sqlWhere.ToString());");
            txt.AppendLine("\t\t\tcmd.CommandText = sql;");
            txt.AppendLine("\t\t\ttry");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\treturn new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecNonQuery(cmd);");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tcatch { return -1; }");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");

            return txt.ToString();
        }
        #endregion

        #region 插入语句
        /// <summary>
        /// 插入语句
        /// </summary>
        /// <returns></returns>
        private string GetInsertContent()
        {
            StringBuilder txt = new StringBuilder();
            txt.AppendLine("\t\t#region 插入");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 插入数据");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tpublic static bool insert( " + this._NamespaceClasses + this._tableName + " " + this._tableName + "_obj)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tSqlCommand cmd = new SqlCommand();");
            txt.AppendLine("\t\t\tStringBuilder cols = new StringBuilder();");
            txt.AppendLine("\t\t\tStringBuilder parameters = new StringBuilder();");
            txt.AppendLine("\t\t\tstring sql = \"INSERT INTO " + this._tableName + "({0}) values({1})\";");
            DBS_TypeMap TM = new DBS_TypeMap();

            #region 数据列组合
            foreach (DataRow dr in this.DT.Rows)
            {
                //自增列不在添加的队列
                if (Convert.ToInt32(dr["isidentity"]) == 1)
                {
                    continue;
                }
                //uniqueidentifier类型在插入、更新时不做处理
                if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                {
                    continue;
                }
                if (TM[dr["Xtype_Name"].ToString()].CodeType == "DateTime")
                {
                    #region DateTime类型
                    if (dr["isnullable"].ToString() == "1")
                    {
                        txt.AppendLine("\t\t\tif (" + this._tableName + "_obj." + dr["name"].ToString() + " != null && " + this._tableName + "_obj." + dr["name"].ToString() + " > DateTime.MinValue)");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                        txt.AppendLine("\t\t\t\t{");
                        txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                        txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                        txt.AppendLine("\t\t\t\t}");
                        txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                        txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                        txt.AppendLine("\t\t\t}");
                        if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                        {
                            txt.AppendLine("\t\t\telse");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                            txt.AppendLine("\t\t\t\t{");
                            txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                            txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                            txt.AppendLine("\t\t\t\t}");
                            txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                            txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                            if (dr["dval"].ToString().ToLower() == "(getdate())")
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Now);");
                            }
                            else
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Parse(\"" + dr["dval"].ToString().Replace("(", "").Replace(")", "").Replace("'", "") + "\"));");
                            }
                            txt.AppendLine("\t\t\t}");
                        }
                    }
                    else
                    {
                        txt.AppendLine("\t\t\tif (cols.Length > 0)");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                        txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                        txt.AppendLine("\t\t\t}");
                        txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                        txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                        txt.AppendLine("\t\t\tif(" + this._tableName + "_obj." + dr["name"].ToString() + " == null || " + this._tableName + "_obj." + dr["name"].ToString() + " == DateTime.MinValue)");
                        txt.AppendLine("\t\t\t{");
                        if (dr["dval"].ToString().ToLower() == "(getdate())")
                        {
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Now);");
                        }
                        else if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                        {
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Parse(\"" + dr["dval"].ToString().Replace("(", "").Replace(")", "").Replace("'", "") + "\"));");
                        }
                        else
                        {
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Parse(\"1900-01-01 00:00:00\"));");
                        }
                        txt.AppendLine("\t\t\t}");
                        txt.AppendLine("\t\t\telse");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                        txt.AppendLine("\t\t\t}");
                    }
                    #endregion
                }
                //else if (TM[dr["Xtype_Name"].ToString()].DBType.ToUpper() == "XML")
                //{
                //    #region XML类型
                //    if (dr["isnullable"].ToString() == "1")
                //    {
                //        txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                //        txt.AppendLine("\t\t\t{");
                //        txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                //        txt.AppendLine("\t\t\t\t{");
                //        txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                //        txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                //        txt.AppendLine("\t\t\t\t}");
                //        txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                //        txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                //        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + this._tableName + "_obj." + dr["name"].ToString() + ", XmlNodeType.Document, null)));");
                //        txt.AppendLine("\t\t\t}");
                //    }
                //    else
                //    {
                //        txt.AppendLine("\t\t\tif (cols.Length > 0)");
                //        txt.AppendLine("\t\t\t{");
                //        txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                //        txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                //        txt.AppendLine("\t\t\t}");
                //        txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                //        txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                //        txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                //        txt.AppendLine("\t\t\t{");
                //        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + this._tableName + "_obj." + dr["name"].ToString() + ", XmlNodeType.Document, null)));");
                //        txt.AppendLine("\t\t\t}");
                //        txt.AppendLine("\t\t\telse");
                //        txt.AppendLine("\t\t\t{");
                //        if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                //        {
                //            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + dr["dval"].ToString().Replace("(", "").Replace(")", "") + ", XmlNodeType.Document, null)));");
                //        }
                //        else
                //        {
                //            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(\"<?xml version=\\\"1.0\\\"?><Items></Items>\", XmlNodeType.Document, null)));");
                //        }
                //        txt.AppendLine("\t\t\t}");
                //    }
                //    #endregion
                //}
                else if (TM[dr["Xtype_Name"].ToString()].CodeType == "string")
                {
                    #region String类型
                    if (dr["isnullable"].ToString() == "1")
                    {
                        txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                        txt.AppendLine("\t\t\t\t{");
                        txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                        txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                        txt.AppendLine("\t\t\t\t}");
                        txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                        txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                        txt.AppendLine("\t\t\t}");
                    }
                    else
                    {
                        txt.AppendLine("\t\t\tif (cols.Length > 0)");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                        txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                        txt.AppendLine("\t\t\t}");
                        txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                        txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                        txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                        txt.AppendLine("\t\t\t}");
                        txt.AppendLine("\t\t\telse");
                        txt.AppendLine("\t\t\t{");
                        if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                        {
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", \"" + dr["dval"].ToString().Replace("(", "").Replace(")", "").Replace("'", "") + "\");");
                        }
                        else
                        {
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", \"\");");
                        }
                        txt.AppendLine("\t\t\t}");
                    }
                    #endregion
                }
                else
                {
                    txt.AppendLine("\t\t\tif (cols.Length > 0)");
                    txt.AppendLine("\t\t\t{");
                    txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                    txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                    txt.AppendLine("\t\t\t}");
                    txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                    txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                    txt.AppendLine("\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                }
            }
            #endregion

            txt.AppendLine("\t\t\tsql = string.Format(sql, cols.ToString(), parameters.ToString());");
            txt.AppendLine("\t\t\tcmd.CommandText = sql;");
            txt.AppendLine("\t\t\tbool b = true;");
            txt.AppendLine("\t\t\ttry");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tint QueryCount = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecNonQuery(cmd);");
            txt.AppendLine("\t\t\t\tif (QueryCount < 1)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\tb = false;");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\tcatch { b = false; }");
            txt.AppendLine("\t\t\treturn b;");
            txt.AppendLine("\t\t}");

            if (this.DT.Select("Isidentity=1").Length > 0)
            {
                DataRow idr = this.DT.Select("Isidentity=1")[0];
                string IdentityColName = idr["name"].ToString();
                string IdentityColType = idr["Xtype_Name"].ToString();
                txt.AppendLine("");
                txt.AppendLine("\t\t/// <summary>");
                txt.AppendLine("\t\t/// 插入数据,返回自增列ID");
                txt.AppendLine("\t\t/// </summary>");
                txt.AppendLine("\t\t/// <returns></returns>");
                txt.AppendLine("\t\tpublic static bool Add( " + this._NamespaceClasses + this._tableName + " " + this._tableName + "_obj, out " + TM[IdentityColType].CodeType + " " + IdentityColName + ")");
                txt.AppendLine("\t\t{");
                txt.AppendLine("\t\t\t" + IdentityColName + " = 0;");
                txt.AppendLine("\t\t\tSqlCommand cmd = new SqlCommand();");
                txt.AppendLine("\t\t\tStringBuilder cols = new StringBuilder();");
                txt.AppendLine("\t\t\tStringBuilder parameters = new StringBuilder();");
                txt.AppendLine("\t\t\tstring sql = \"INSERT INTO " + this._tableName + "({0}) values({1});SELECT @@IDENTITY;\";");

                #region 数据列组合
                foreach (DataRow dr in this.DT.Rows)
                {
                    //自增列不在添加的队列
                    if (Convert.ToInt32(dr["isidentity"]) == 1)
                    {
                        continue;
                    }
                    //uniqueidentifier类型在插入、更新时不做处理
                    if (TM[dr["Xtype_Name"].ToString()].DBType == "uniqueidentifier")
                    {
                        continue;
                    }
                    if (TM[dr["Xtype_Name"].ToString()].DBType == "timestamp")
                    {
                        continue;
                    }
                    if (TM[dr["Xtype_Name"].ToString()].CodeType == "DateTime")
                    {
                        #region DateTime类型
                        if (dr["isnullable"].ToString() == "1")
                        {
                            txt.AppendLine("\t\t\tif (" + this._tableName + "_obj." + dr["name"].ToString() + " != null && " + this._tableName + "_obj." + dr["name"].ToString() + " > DateTime.MinValue)");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                            txt.AppendLine("\t\t\t\t{");
                            txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                            txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                            txt.AppendLine("\t\t\t\t}");
                            txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                            txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                            txt.AppendLine("\t\t\t}");
                            if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                            {
                                txt.AppendLine("\t\t\telse");
                                txt.AppendLine("\t\t\t{");
                                txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                                txt.AppendLine("\t\t\t\t{");
                                txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                                txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                                txt.AppendLine("\t\t\t\t}");
                                txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                                txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                                if (dr["dval"].ToString().ToLower() == "(getdate())")
                                {
                                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Now);");
                                }
                                else
                                {
                                    txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Parse(\"" + dr["dval"].ToString().Replace("(", "").Replace(")", "").Replace("'", "") + "\"));");
                                }
                                txt.AppendLine("\t\t\t}");
                            }
                        }
                        else
                        {
                            txt.AppendLine("\t\t\tif (cols.Length > 0)");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                            txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                            txt.AppendLine("\t\t\t}");
                            txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                            txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                            txt.AppendLine("\t\t\tif(" + this._tableName + "_obj." + dr["name"].ToString() + " == null || " + this._tableName + "_obj." + dr["name"].ToString() + " == DateTime.MinValue)");
                            txt.AppendLine("\t\t\t{");
                            if (dr["dval"].ToString().ToLower() == "(getdate())")
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Now);");
                            }
                            else if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Parse(\"" + dr["dval"].ToString().Replace("(", "").Replace(")", "").Replace("'", "") + "\"));");
                            }
                            else
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", DateTime.Parse(\"1900-01-01 00:00:00\"));");
                            }
                            txt.AppendLine("\t\t\t}");
                            txt.AppendLine("\t\t\telse");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                            txt.AppendLine("\t\t\t}");
                        }
                        #endregion
                    }
                    //else if (TM[dr["Xtype_Name"].ToString()].DBType.ToUpper() == "XML")
                    //{
                    //    #region XML类型
                    //    if (dr["isnullable"].ToString() == "1")
                    //    {
                    //        txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                    //        txt.AppendLine("\t\t\t{");
                    //        txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                    //        txt.AppendLine("\t\t\t\t{");
                    //        txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                    //        txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                    //        txt.AppendLine("\t\t\t\t}");
                    //        txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                    //        txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                    //        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + this._tableName + "_obj." + dr["name"].ToString() + ", XmlNodeType.Document, null)));");
                    //        txt.AppendLine("\t\t\t}");
                    //    }
                    //    else
                    //    {
                    //        txt.AppendLine("\t\t\tif (cols.Length > 0)");
                    //        txt.AppendLine("\t\t\t{");
                    //        txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                    //        txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                    //        txt.AppendLine("\t\t\t}");
                    //        txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                    //        txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                    //        txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                    //        txt.AppendLine("\t\t\t{");
                    //        txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + this._tableName + "_obj." + dr["name"].ToString() + ", XmlNodeType.Document, null)));");
                    //        txt.AppendLine("\t\t\t}");
                    //        txt.AppendLine("\t\t\telse");
                    //        txt.AppendLine("\t\t\t{");
                    //        if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                    //        {
                    //            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(" + dr["dval"].ToString().Replace("(","").Replace(")","") + ", XmlNodeType.Document, null)));");
                    //        }
                    //        else
                    //        {
                    //            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", new SqlXml(new XmlTextReader(\"<?xml version=\\\"1.0\\\"?><Items></Items>\", XmlNodeType.Document, null)));");
                    //        }
                    //        txt.AppendLine("\t\t\t}");
                    //    }
                    //    #endregion
                    //}
                    else if (TM[dr["Xtype_Name"].ToString()].CodeType == "string")
                    {
                        #region String类型
                        if (dr["isnullable"].ToString() == "1")
                        {
                            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tif (cols.Length > 0)");
                            txt.AppendLine("\t\t\t\t{");
                            txt.AppendLine("\t\t\t\t\tcols.Append(\",\");");
                            txt.AppendLine("\t\t\t\t\tparameters.Append(\",\");");
                            txt.AppendLine("\t\t\t\t}");
                            txt.AppendLine("\t\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                            txt.AppendLine("\t\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                            txt.AppendLine("\t\t\t}");
                        }
                        else
                        {
                            txt.AppendLine("\t\t\tif (cols.Length > 0)");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                            txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                            txt.AppendLine("\t\t\t}");
                            txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                            txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(" + this._tableName + "_obj." + dr["name"].ToString() + "))");
                            txt.AppendLine("\t\t\t{");
                            txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                            txt.AppendLine("\t\t\t}");
                            txt.AppendLine("\t\t\telse");
                            txt.AppendLine("\t\t\t{");
                            if (!string.IsNullOrEmpty(dr["dval"].ToString()))
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", \"" + dr["dval"].ToString().Replace("(","").Replace(")","").Replace("'","") + "\");");
                            }
                            else
                            {
                                txt.AppendLine("\t\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", \"\");");
                            }
                            txt.AppendLine("\t\t\t}");
                        }
                        #endregion
                    }
                    else
                    {
                        txt.AppendLine("\t\t\tif (cols.Length > 0)");
                        txt.AppendLine("\t\t\t{");
                        txt.AppendLine("\t\t\t\tcols.Append(\",\");");
                        txt.AppendLine("\t\t\t\tparameters.Append(\",\");");
                        txt.AppendLine("\t\t\t}");
                        txt.AppendLine("\t\t\tcols.Append(\"[" + dr["name"].ToString() + "]\");");
                        txt.AppendLine("\t\t\tparameters.Append(\"@" + dr["name"].ToString() + "\");");
                        txt.AppendLine("\t\t\tcmd.Parameters.AddWithValue(\"@" + dr["name"].ToString() + "\", " + this._tableName + "_obj." + dr["name"].ToString() + ");");
                    }
                }
                #endregion

                txt.AppendLine("\t\t\tsql = string.Format(sql, cols.ToString(), parameters.ToString());");
                txt.AppendLine("\t\t\tcmd.CommandText = sql;");
                txt.AppendLine("\t\t\tbool b = true;");
                txt.AppendLine("\t\t\ttry");
                txt.AppendLine("\t\t\t{");
                txt.AppendLine("\t\t\t\tobject idobj = new " + this._NamespaceDbu + ".DBHelper(ConnectionConfigMark, false).ExecScalar(cmd);");
                if (TM[IdentityColType].CodeType == "Int64")
                {
                    txt.AppendLine("\t\t\t\t" + IdentityColName + " = Convert.ToInt64(idobj);");
                }
                else if (TM[IdentityColType].CodeType == "Int32")
                {
                    txt.AppendLine("\t\t\t\t" + IdentityColName + " = Convert.ToInt32(idobj);");
                }
                else if (TM[IdentityColType].CodeType == "Int16")
                {
                    txt.AppendLine("\t\t\t\t" + IdentityColName + " = Convert.ToInt16(idobj);");
                }
                txt.AppendLine("\t\t\t\tif (" + IdentityColName + " == 0)");
                txt.AppendLine("\t\t\t\t{");
                txt.AppendLine("\t\t\t\t\tb = false;");
                txt.AppendLine("\t\t\t\t}");
                txt.AppendLine("\t\t\t}");
                txt.AppendLine("\t\t\tcatch { b = false; }");
                txt.AppendLine("\t\t\treturn b;");
                txt.AppendLine("\t\t}");
            }
            txt.AppendLine("\t\t#endregion");
            return txt.ToString();
        }
        #endregion

        #region 参数准备
        /// <summary>
        /// 参数准备
        /// </summary>
        /// <returns></returns>
        private string GetParametersContent()
        {
            StringBuilder txt = new StringBuilder();
            #region 参数处理
            txt.AppendLine("\t\t#region 查询执行器构造");
            txt.AppendLine("\t\t/// <summary>");
            txt.AppendLine("\t\t/// 查询执行器构造");
            txt.AppendLine("\t\t/// </summary>");
            txt.AppendLine("\t\t/// <param name=\"sql\">完整SQL语句</param>");
            txt.AppendLine("\t\t/// <param name=\"ParamsList\">可选参数列表</param>");
            txt.AppendLine("\t\t/// <returns></returns>");
            txt.AppendLine("\t\tprivate static SqlCommand BuildCommand(string sql, object[] ParamsList = null)");
            txt.AppendLine("\t\t{");
            txt.AppendLine("\t\t\tSqlCommand cmd = new SqlCommand();");
            txt.AppendLine("\t\t\tcmd.CommandText = sql;");
            txt.AppendLine("\t\t\tif (!string.IsNullOrEmpty(sql))");
            txt.AppendLine("\t\t\t{");
            txt.AppendLine("\t\t\t\tList<string> ParameterList = new List<string>();");
            txt.AppendLine("\t\t\t\tRegex reg = new Regex(\"(@[0-9a-zA-Z_]{1,30})\", RegexOptions.IgnoreCase);");
            txt.AppendLine("\t\t\t\tMatchCollection mc = reg.Matches(sql);");
            txt.AppendLine("\t\t\t\tif (mc != null && mc.Count > 0)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\tforeach (Match m in mc)");
            txt.AppendLine("\t\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\t\tif (!ParameterList.Contains(m.Groups[1].Value))");
            txt.AppendLine("\t\t\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\t\t\tParameterList.Add(m.Groups[1].Value);");
            txt.AppendLine("\t\t\t\t\t\t}");
            txt.AppendLine("\t\t\t\t\t}");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t\tif (ParameterList.Count > 0)");
            txt.AppendLine("\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\tint i = 0;");
            txt.AppendLine("\t\t\t\t\tforeach (string ParameterName in ParameterList)");
            txt.AppendLine("\t\t\t\t\t{");
            txt.AppendLine("\t\t\t\t\t\tcmd.Parameters.AddWithValue(ParameterName, ParamsList[i]);");
            txt.AppendLine("\t\t\t\t\t\ti++;");
            txt.AppendLine("\t\t\t\t\t}");
            txt.AppendLine("\t\t\t\t}");
            txt.AppendLine("\t\t\t}");
            txt.AppendLine("\t\t\treturn cmd;");
            txt.AppendLine("\t\t}");
            txt.AppendLine("\t\t#endregion");
            #endregion
            return txt.ToString();
        }
        #endregion
    }
}