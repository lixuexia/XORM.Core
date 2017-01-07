using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using XORM.CBase.Data;
using XORM.CBase.Data.Common;

namespace XORM.CBase
{
    public static class JoinExtension
    {
        /// <summary>
        /// 从类型获取表信息，用于链接查询
        /// </summary>
        /// <param name="_TypeInfo"></param>
        /// <returns></returns>
        private static Query GetQueryFromT(Type _TypeInfo)
        {
            return ModelBase<object>.BuildQuery(_TypeInfo);
        }
        /// <summary>
        /// 内链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryObj"></param>
        /// <param name="SName"></param>
        /// <param name="JoinCondition"></param>
        /// <param name="ParamsList"></param>
        /// <returns></returns>
        public static Query INNER_JOIN<T>(this Query QueryObj,string SName, string JoinCondition, object[] ParamsList)
        {
            return Join<T>(QueryObj, SName, JoinCondition, ParamsList, "INNER");
        }
        /// <summary>
        /// 左链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryObj"></param>
        /// <param name="SName"></param>
        /// <param name="JoinCondition"></param>
        /// <param name="ParamsList"></param>
        /// <returns></returns>
        public static Query LEFT_JOIN<T>(this Query QueryObj, string SName, string JoinCondition, object[] ParamsList = null)
        {
            return Join<T>(QueryObj, SName, JoinCondition, ParamsList, "LEFT OUTER");
        }
        /// <summary>
        /// 右链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryObj"></param>
        /// <param name="SName"></param>
        /// <param name="JoinCondition"></param>
        /// <param name="ParamsList"></param>
        /// <returns></returns>
        public static Query RIGHT_JOIN<T>(this Query QueryObj, string SName, string JoinCondition, object[] ParamsList = null)
        {
            return Join<T>(QueryObj, SName, JoinCondition, ParamsList, "RIGHT OUTER");
        }
        /// <summary>
        /// 链接方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryObj"></param>
        /// <param name="SName"></param>
        /// <param name="JoinCondition"></param>
        /// <param name="ParamsList"></param>
        /// <param name="JoinAction"></param>
        /// <returns></returns>
        private static Query Join<T>(Query QueryObj, string SName, string JoinCondition, object[] ParamsList = null,string JoinAction="INNER")
        {
            Query DesQuery = GetQueryFromT(typeof(T));
            if(DesQuery.ORM_TabInfo.ORMConnectionMark!=QueryObj.ORM_TabInfo.ORMConnectionMark)
            {
                throw new Exception("链接查询的两个表需要属于同一数据源");
            }
            QueryObj.TabNameDic.Add(SName, DesQuery.ORM_TabInfo.ORMTableName);
            QueryObj.TabColDic.Add(SName, DesQuery.ORM_TabInfo.ORMColList);
            QueryObj.TabClassDic.Add(SName, DesQuery.ORM_TabInfo.ORMTypeName);
            QueryObj.JoinTxt.Append(" ").Append(JoinAction).Append(" JOIN ").Append("[").Append(DesQuery.ORM_TabInfo.ORMTableName).Append("]").Append(" AS ").Append(SName);
            AddOutCols(QueryObj, DesQuery.ORM_TabInfo.ORMColList, SName);
            string SafeCondition = "";
            CheckJoinCols(QueryObj, JoinCondition, out SafeCondition);
            QueryObj.JoinTxt.Append(" ON ").Append(SafeCondition);
            BuildCommand(QueryObj, string.Concat(" ON ", SafeCondition), ParamsList);
            return QueryObj;
        }
        /// <summary>
        /// 数据筛选
        /// </summary>
        /// <param name="QueryObj"></param>
        /// <param name="Condition"></param>
        /// <param name="ParamsList"></param>
        /// <returns></returns>
        public static Query Where(this Query QueryObj, string Condition, object[] ParamsList = null)
        {
            string SafeCondition = "";
            CheckJoinCols(QueryObj, Condition, out SafeCondition);
            QueryObj.JoinTxt.Append(" WHERE ").Append(SafeCondition);
            BuildCommand(QueryObj, SafeCondition, ParamsList);
            return QueryObj;
        }
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="QueryObj"></param>
        /// <param name="sqlSort"></param>
        /// <param name="RecordCount"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UseReadOnlyDataSource"></param>
        /// <returns></returns>
        public static List<QueryResult> List(this Query QueryObj, string sqlSort, out long RecordCount, int PageIndex = 1, int PageSize = 20, bool UseReadOnlyDataSource = true)
        {
            RecordCount = 0;
            QueryObj.PageSort = sqlSort;
            QueryObj.MyCmd.CommandText = QueryObj.SQLTEXT_PAGE;
            QueryObj.MyCmd.Parameters.Add("@SI", SqlDbType.Int).Value = PageIndex * PageSize - PageSize + 1;
            QueryObj.MyCmd.Parameters.Add("@EI", SqlDbType.Int).Value = PageIndex * PageSize;
            DataSet DS = new DBHelper(QueryObj.ORM_TabInfo.ORMConnectionMark, UseReadOnlyDataSource).ExecDataSet(QueryObj.MyCmd);
            List<QueryResult> jlist = new List<QueryResult>();
            if (DS != null && DS.Tables.Count == 2)
            {
                Data.Common.DataTable DT = DS.Tables[0];
                try
                {
                    long.TryParse(DS.Tables[1].Rows[0][0].ToString(), out RecordCount);
                }
                catch { }
                jlist = BuildQueryResult(QueryObj, DT);
            }

            return jlist;
        }

        #region 添加输出字段
        /// <summary>
        /// 添加输出字段
        /// </summary>
        /// <param name="JoinTableColList"></param>
        /// <param name="SName"></param>
        internal static void AddOutCols(this Query QueryObj, List<string> JoinTableColList, string SName)
        {
            foreach (string colName in JoinTableColList)
            {
                QueryObj.OutCols.Append(",").Append(SName).Append(".[").Append(colName).Append("]").Append(" AS ").Append(SName).Append("_").Append(colName);
                QueryObj.PageCols.Append(",").Append(SName).Append("_").Append(colName);
            }
        }
        #endregion

        #region 条件检查
        /// <summary>
        /// 条件检查
        /// </summary>
        /// <param name="JoinCondition"></param>
        internal static void CheckJoinCols(Query QueryObj, string JoinCondition, out string SafeCondition)
        {
            StringBuilder SafeConditionTxt = new StringBuilder(JoinCondition);
            SafeConditionTxt.Replace("[", "").Replace("]", "");
            Regex JoinArrayReg = new Regex(@"([a-zA-Z_]{1,30}[0-9a-zA-Z_]{0,30})\.[\[]?([a-zA-Z_]{1,30}[0-9a-zA-Z_]{0,30})[\]]?", RegexOptions.IgnoreCase);
            MatchCollection MC = JoinArrayReg.Matches(JoinCondition);
            if (MC != null && MC.Count > 0)
            {
                foreach (Match m in MC)
                {
                    string TabName = m.Groups[1].Value;
                    string ColName = m.Groups[2].Value;

                    if (!QueryObj.TabNameDic.ContainsKey(TabName))
                    {
                        throw new Exception(string.Concat(TabName, "表不存在"));
                    }

                    if (!QueryObj.TabColDic[TabName].Contains(ColName.ToUpper()))
                    {
                        throw new Exception(string.Concat(TabName, ":[", QueryObj.TabNameDic[TabName], "]", "表不存在字段", ColName));
                    }
                    SafeConditionTxt = SafeConditionTxt.Replace(string.Concat(TabName, ".", ColName), string.Concat(TabName, ".[", ColName, "]"));

                    if (!QueryObj.OutTabList.Contains(TabName))
                    {
                        QueryObj.OutTabList.Add(TabName);
                    }
                }
                SafeCondition = SafeConditionTxt.ToString();
            }
            else
            {
                SafeCondition = JoinCondition;
            }
        }
        /// <summary>
        /// 输出字段变换
        /// </summary>
        /// <param name="JoinCondition"></param>
        internal static void CheckOutCols(Query QueryObj, string OutColumns)
        {
            Regex JoinArrayReg = new Regex(@"([a-zA-Z_]{1,30}[0-9a-zA-Z_]{0,30})\.[\[]?([a-zA-Z_]{1,30}[0-9a-zA-Z_]{0,30}|[\*]?)[\]]?", RegexOptions.IgnoreCase);
            MatchCollection MC = JoinArrayReg.Matches(OutColumns);
            StringBuilder OutputTxt = new StringBuilder();
            if (MC != null && MC.Count > 0)
            {
                foreach (Match m in MC)
                {
                    string TabName = m.Groups[1].Value;
                    string ColName = m.Groups[2].Value;

                    if (!QueryObj.TabNameDic.ContainsKey(TabName))
                    {
                        throw new Exception(string.Concat(TabName, "表不存在"));
                    }

                    if (ColName == "*")
                    {
                        foreach (string col in QueryObj.TabColDic[TabName])
                        {
                            if (OutputTxt.Length == 0)
                            {
                                OutputTxt.Append(string.Concat(TabName, ".", col, " AS ", TabName, "_", col));
                            }
                            else
                            {
                                OutputTxt.Append(",").Append(string.Concat(TabName, ".", col, " AS ", TabName, "_", col));
                            }
                        }
                    }
                    else
                    {
                        if (!QueryObj.TabColDic[TabName].Contains(ColName))
                        {
                            throw new Exception(string.Concat(TabName, ":[", QueryObj.TabNameDic[TabName], "]", "表不存在字段", ColName));
                        }

                        if (OutputTxt.Length == 0)
                        {
                            OutputTxt.Append(string.Concat(TabName, ".", ColName, " AS ", TabName, "_", ColName));
                        }
                        else
                        {
                            OutputTxt.Append(",").Append(string.Concat(TabName, ".", ColName, " AS ", TabName, "_", ColName));
                        }
                    }

                    if (!QueryObj.OutTabList.Contains(TabName))
                    {
                        QueryObj.OutTabList.Add(TabName);
                    }
                }
                QueryObj.OutCols.Clear();
                QueryObj.OutCols.Append(OutputTxt.ToString());
            }
        }
        #endregion                        

        #region 输出结果
        /// <summary>
        /// 输出列表
        /// </summary>
        /// <param name="UseReadOnlyDataSource">是否使用只读数据源</param>
        /// <returns></returns>
        public static List<QueryResult> List(this Query QueryObj, bool UseReadOnlyDataSource = true)
        {
            QueryObj.MyCmd.CommandText = QueryObj.SQLTEXT;
            Data.Common.DataTable DT = new DBHelper(QueryObj.ORM_TabInfo.ORMConnectionMark, UseReadOnlyDataSource).ExecDataTable(QueryObj.MyCmd);
            return BuildQueryResult(QueryObj, DT);
        }
        /// <summary>
        /// 输出列表
        /// </summary>
        /// <param name="UseReadOnlyDataSource">是否使用只读数据源</param>
        /// <returns></returns>
        public static List<QueryResult> List(this Query QueryObj, string sqlSort, bool UseReadOnlyDataSource = true)
        {
            QueryObj.MyCmd.CommandText = QueryObj.SQLTEXT + " ORDER BY " + sqlSort;
            Data.Common.DataTable DT = new DBHelper(QueryObj.ORM_TabInfo.ORMConnectionMark, UseReadOnlyDataSource).ExecDataTable(QueryObj.MyCmd);
            List<QueryResult> jlist = new List<QueryResult>();
            return BuildQueryResult(QueryObj, DT);
        }
        /// <summary>
        /// 查询结果整合为结果对象
        /// </summary>
        /// <param name="QueryObj"></param>
        /// <param name="DT"></param>
        /// <returns></returns>
        private static List<QueryResult> BuildQueryResult(Query QueryObj, Data.Common.DataTable DT)
        {
            List<QueryResult> jlist = new List<QueryResult>();
            if (DT != null && DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    QueryResult jr = new QueryResult();
                    foreach (string tabSName in QueryObj.OutTabList)
                    {
                        Data.Common.DataTable TempDT = new Data.Common.DataTable();
                        foreach (DataColumn dc in DT.Columns)
                        {
                            if (dc.ColumnName.StartsWith(tabSName + "_"))
                            {
                                TempDT.Columns.Add(dc.ColumnName.Substring((tabSName + "_").Length));
                            }
                        }
                        DataRow TempDr = TempDT.NewRow();
                        foreach (DataColumn dc in TempDT.Columns)
                        {
                            TempDr[dc.ColumnName] = dr[tabSName + "_" + dc.ColumnName];
                        }
                        TempDT.Rows.Add(TempDr);
                        if (!jr.TabSNameDic.ContainsKey(QueryObj.TabClassDic[tabSName]))
                        {
                            jr.TabSNameDic.Add(QueryObj.TabClassDic[tabSName], new List<string>());
                        }
                        jr.RowTabDic.Add(tabSName, TempDr);
                        jr.TabSNameDic[QueryObj.TabClassDic[tabSName]].Add(tabSName);
                    }
                    jlist.Add(jr);
                }
            }

            return jlist;
        }

        /// <summary>
        /// 执行成功返回记录总数，执行错误返回-1
        /// </summary>
        /// <param name="UseReadOnlyDataSource">是否使用只读数据源</param>
        /// <returns></returns>
        public static long Count(this Query QueryObj, bool UseReadOnlyDataSource = true)
        {
            QueryObj.MyCmd.CommandText = QueryObj.SQLTEXT_COUNT;
            object CountObject = new DBHelper(QueryObj.ORM_TabInfo.ORMConnectionMark, UseReadOnlyDataSource).ExecScalar(QueryObj.MyCmd);
            if (CountObject != null && !string.IsNullOrEmpty(CountObject.ToString()))
            {
                long CountVal = 0;
                long.TryParse(CountObject.ToString(), out CountVal);
                return CountVal;
            }
            return -1;
        }

        /// <summary>
        /// 输出列表,输出自定义输出函数结果集,如：sum(price),sum(amount),执行错误返回Null
        /// </summary>
        /// <param name="FunctionList">自定义函数列表，如：sum(price),sum(amount)</param>
        /// <param name="UseReadOnlyDataSource">是否使用只读数据源</param>
        /// <returns></returns>
        public static Data.Common.DataTable Function(this Query QueryObj, string FunctionList, bool UseReadOnlyDataSource = true)
        {
            QueryObj.SingleSql = FunctionList;
            QueryObj.MyCmd.CommandText = QueryObj.SQLTEXT_SINGLE;
            Data.Common.DataTable DT = new DBHelper(QueryObj.ORM_TabInfo.ORMConnectionMark, UseReadOnlyDataSource).ExecDataTable(QueryObj.MyCmd);
            return DT;
        }
        #endregion

        #region 查询命令构造
        /// <summary>
        /// 查询命令构造
        /// </summary>
        /// <param name="sqlPart">部分SQL语句</param>
        /// <param name="ParamsList">可选参数列表</param>
        /// <returns></returns>
        internal static void BuildCommand(Query QueryObj, string sqlPart, object[] ParamsList = null)
        {
            if (!string.IsNullOrEmpty(sqlPart))
            {
                List<string> ParameterList = new List<string>();
                Regex reg = new Regex("(@[0-9a-zA-Z_]{1,30})", RegexOptions.IgnoreCase);
                MatchCollection mc = reg.Matches(sqlPart);
                if (mc != null && mc.Count > 0)
                {
                    foreach (Match m in mc)
                    {
                        if (!ParameterList.Contains(m.Groups[1].Value))
                        {
                            ParameterList.Add(m.Groups[1].Value);
                        }
                    }
                }
                if (ParameterList.Count > 0)
                {
                    int i = 0;
                    foreach (string ParameterName in ParameterList)
                    {
                        if (!QueryObj.MyCmd.Parameters.Contains(ParameterName))
                        {
                            QueryObj.MyCmd.Parameters.AddWithValue(ParameterName, ParamsList[i]);
                        }
                        i++;
                    }
                }
            }
        }
        #endregion
    }
}