using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace XORM.CBase.Tool
{
    /// <summary>
    /// SQL构建器
    /// </summary>
    internal class SqlGenerator
    {
        #region 内存缓存：表类型-表SQL字典集合
        /// <summary>
        /// 内存缓存：表类型-表SQL字典集合
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> TableSQLDic = new Dictionary<string, Dictionary<string, string>>();
        #endregion

        #region 创建分页查询SQL
        /// <summary>
        /// 创建分页查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="EWhere"></param>
        /// <param name="SqlOrderBy"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_List(TableDefinition TabInfo, ExpandoObject EWhere, string SqlOrderBy, int PageIndex, int PageSize, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromExpandoObject(TabInfo, EWhere, out SqlParams);
            return SQLBUILDER_List(TabInfo, SqlWhere, SqlOrderBy, PageIndex, PageSize);
        }
        /// <summary>
        /// 创建分页查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlOrderBy"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_List(TableDefinition TabInfo, dynamic DWhere, string SqlOrderBy, int PageIndex, int PageSize, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_List(TabInfo, SqlWhere, SqlOrderBy, PageIndex, PageSize);
        }
        /// <summary>
        /// 创建分页查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="SqlWhere"></param>
        /// <param name="SqlOrderBy"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static string SQLBUILDER_List(TableDefinition TabInfo, string SqlWhere, string SqlOrderBy, int PageIndex, int PageSize)
        {
            string SQLKey = "";
            if (PageIndex == 1)
            {
                SQLKey = "LIST1_" + PageSize.ToString();
            }
            else
            {
                SQLKey = "LISTALL";
            }
            if (string.IsNullOrEmpty(SqlOrderBy))
            {
                SQLKey += "_NOORDERBY";
            }
            else
            {
                SQLKey += "_USEORDERBY";
            }

            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return string.Format(TableSQLDic[TabInfo.ORMTypeName][SQLKey], SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : "Order By " + SqlOrderBy).ToUpper();
            }

            StringBuilder SqlBuilder;
            string SqlText = "";
            SqlBuilder = new StringBuilder();
            switch (TabInfo.ORMConnectionProvider)
            {
                case "1":
                    SqlBuilder = new StringBuilder();
                    if (PageIndex == 1)
                    {
                        SqlBuilder
                            .Append("SELECT TOP ").Append(Convert.ToString(PageIndex * PageSize)).Append(" * FROM [").Append(TabInfo.ORMTableName).Append("](NOLOCK) ").Append("WHERE {0} {1}");
                    }
                    else
                    {
                        SqlBuilder
                            .Append("SELECT * FROM ")
                            .Append("(")
                            .Append("SELECT ROW_NUMBER() OVER({1}) RN,* FROM [").Append(TabInfo.ORMTableName).Append("](NOLOCK) ")
                            .Append("WHERE {0}")
                            .Append(") PST ")
                            .Append("WHERE RN BETWEEN @ST AND @ET");
                    }
                    SqlBuilder.Append(";").Append(SQLBUILDER_Count(TabInfo, SqlWhere));
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : "Order By " + SqlOrderBy).ToUpper();
                case "2":
                case "3":
                    SqlBuilder = new StringBuilder();
                    if (string.IsNullOrEmpty(SqlOrderBy))
                    {
                        /*
                        SELECT * FROM 
                        (
                            SELECT ROWNUM AS rowno, t.* FROM emp t WHERE hire_date BETWEEN TO_DATE ('20060501', 'yyyymmdd') AND TO_DATE ('20060731', 'yyyymmdd') AND ROWNUM <= 20
                        ) table_alias
                        WHERE table_alias.rowno >= 10
                        */
                        SqlBuilder
                            .Append("SELECT * FROM ")
                            .Append("(")
                            .Append("SELECT ROWNUM AS ROWNO, T.* FROM ").Append(TabInfo.ORMTableName).Append(" T WHERE {0}")
                            .Append(" AND ROWNUM<=").Append(Convert.ToString(PageIndex * PageSize))
                            .Append(") PST WHERE PST.ROWNO>=").Append(Convert.ToInt32(PageIndex * PageSize - PageSize));
                        SqlBuilder.Append(";").Append(SQLBUILDER_Count(TabInfo, SqlWhere));
                    }
                    else
                    {
                        /*
                        SELECT * FROM 
                        (
                            SELECT tt.*, ROWNUM AS rowno FROM 
                            (  
                                SELECT t.* FROM emp t WHERE hire_date BETWEEN TO_DATE ('20060501', 'yyyymmdd') AND TO_DATE ('20060731', 'yyyymmdd') ORDER BY create_time DESC, emp_no
                            ) tt
                            WHERE ROWNUM <= 20
                        ) table_alias
                        WHERE table_alias.rowno >= 10;
                        */
                        SqlBuilder
                            .Append("SELECT * FROM ")
                            .Append("(")
                            .Append("SELECT TT.*,ROWNUM AS ROWNO FROM ")
                            .Append("(")
                            .Append("SELECT T.* FROM ").Append(TabInfo.ORMTableName).Append(" T ")
                            .Append("WHERE {0} {1}")
                            .Append(") TT ")
                            .Append("WHERE ROWNUM<=").Append(Convert.ToString(PageIndex * PageSize))
                            .Append(") PST ")
                            .Append("WHERE PST.ROWNO>=").Append(Convert.ToString(PageIndex * PageSize - PageSize));
                    }
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : "Order By " + SqlOrderBy).ToUpper();
                default:
                    throw new Exception("数据源类型不明确，请配置数据源类型：1SQLServer，2MySQL，3Oracle");
            }
        }
        #endregion

        #region 创建统计查询SQL
        /// <summary>
        /// 创建统计查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Count(TableDefinition TabInfo, dynamic DWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_Count(TabInfo, SqlWhere);
        }
        /// <summary>
        /// 创建统计查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Count(TableDefinition TabInfo, ExpandoObject DWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromExpandoObject(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_Count(TabInfo, SqlWhere);
        }
        /// <summary>
        /// 创建统计查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="SqlWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Count(TableDefinition TabInfo, string SqlWhere)
        {
            string SQLKey = "COUNT";
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return string.Format(TableSQLDic[TabInfo.ORMTypeName][SQLKey], SqlWhere).ToUpper();
            }

            StringBuilder SqlBuilder;
            string SqlText = "";
            SqlBuilder = new StringBuilder();
            switch (TabInfo.ORMConnectionProvider)
            {
                case "1":
                    SqlBuilder = new StringBuilder();
                    SqlBuilder.Append("SELECT COUNT(1) FROM [").Append(TabInfo.ORMTableName).Append("](NOLOCK)").Append(" WHERE {0}");
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere).ToUpper();
                case "2":
                case "3":
                    SqlBuilder = new StringBuilder();
                    SqlBuilder.Append("SELECT COUNT(1) FROM ").Append(TabInfo.ORMTableName).Append(" WHERE {0}");
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere).ToUpper();
                default:
                    throw new Exception("数据源类型不明确，请配置数据源类型：1SQLServer，2MySQL，3Oracle");
            }
        }
        #endregion

        #region 创建单条查询SQL
        /// <summary>
        /// 创建单条查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="SqlWhere"></param>
        /// <param name="SqlOrderBy"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Get(TableDefinition TabInfo, string SqlWhere, string SqlOrderBy)
        {
            string SQLKey = "GET";
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return string.Format(TableSQLDic[TabInfo.ORMTypeName][SQLKey], string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : " ORDER BY " + SqlOrderBy).ToUpper();
            }

            StringBuilder SqlBuilder;
            string SqlText = "";
            switch (TabInfo.ORMConnectionProvider)
            {
                case "1"://SQLServer写法：select top 1 * from A(nolock) where 1=1 order by id asc
                    SqlBuilder = new StringBuilder();
                    SqlBuilder
                        .Append("SELECT TOP 1 * FROM [")
                        .Append(TabInfo.ORMTableName)
                        .Append("](NOLOCK) ")
                        .Append(" WHERE {0} {1}");
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : " ORDER BY " + SqlOrderBy).ToUpper();
                case "2"://MySQL写法：select * from user order by id desc limit 1
                    SqlBuilder = new StringBuilder();
                    SqlBuilder
                        .Append("SELECT ")
                        .Append("* FROM ")
                        .Append(TabInfo.ORMTableName)
                        .Append(" WHERE {0} {1}")
                        .Append(" LIMIT 1");
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : " ORDER BY " + SqlOrderBy).ToUpper();
                case "3"://ORACLE优化写法：select a,b from (select a,b from table order by c) where rownum<=1
                    SqlBuilder = new StringBuilder();
                    SqlBuilder
                        .Append("SELECT * FROM (SELECT * FROM ")
                        .Append(TabInfo.ORMTableName)
                        .Append(" WHERE {0} {1}")
                        .Append(") WHERE ROWNUM<=1");
                    SqlText = SqlBuilder.ToString().ToUpper();
                    if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                    {
                        Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                        SingleTableSQLDic.Add(SQLKey, SqlText);
                        TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                    }
                    else
                    {
                        lock (TableSQLDic[TabInfo.ORMTypeName])
                        {
                            if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                            {
                                TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                            }
                        }
                    }
                    return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere, string.IsNullOrEmpty(SqlOrderBy) ? "" : " ORDER BY " + SqlOrderBy).ToUpper();
                default:
                    throw new Exception("数据源类型不明确，请配置数据源类型：1SQLServer，2MySQL，3Oracle");
            }
        }
        /// <summary>
        /// 创建单条查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Get(TableDefinition TabInfo, dynamic DWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_Get(TabInfo, SqlWhere, "");
        }
        /// <summary>
        /// 创建单条查询SQL
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="EWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Get(TableDefinition TabInfo, ExpandoObject EWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, EWhere, out SqlParams);
            return SQLBUILDER_Get(TabInfo, SqlWhere, "");
        }
        #endregion

        #region 创建逻辑删除语句
        /// <summary>
        /// 创建逻辑删除语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="SqlWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Del(TableDefinition TabInfo, string SqlWhere)
        {
            string SQLKey = "DEL";
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return string.Format(TableSQLDic[TabInfo.ORMTypeName][SQLKey], string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere).ToUpper();
            }

            StringBuilder SqlBuilder = new StringBuilder();
            string SqlText = "";
            SqlBuilder
                .Append("UPDATE ")
                .Append(TabInfo.ORMTableName)
                .Append(" SET IsDel=1")
                .Append(" WHERE {0}");
            SqlText = SqlBuilder.ToString().ToUpper();
            if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
            {
                Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                SingleTableSQLDic.Add(SQLKey, SqlText);
                TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
            }
            else
            {
                lock (TableSQLDic[TabInfo.ORMTypeName])
                {
                    if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                    {
                        TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                    }
                }
            }
            return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere).ToUpper();
        }
        /// <summary>
        /// 创建逻辑删除语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Del(TableDefinition TabInfo, dynamic DWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_Del(TabInfo, SqlWhere);
        }
        /// <summary>
        /// 创建逻辑删除语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="EWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Del(TableDefinition TabInfo, ExpandoObject EWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, EWhere, out SqlParams);
            return SQLBUILDER_Del(TabInfo, SqlWhere);
        }
        #endregion

        #region 创建物理删除语句
        /// <summary>
        /// 创建物理删除语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="SqlWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_DbDel(TableDefinition TabInfo, string SqlWhere)
        {
            string SQLKey = "DBDEL";
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return string.Format(TableSQLDic[TabInfo.ORMTypeName][SQLKey], string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere).ToUpper();
            }

            StringBuilder SqlBuilder = new StringBuilder();
            string SqlText = "";
            SqlBuilder
                .Append("DELETE FROM ")
                .Append(TabInfo.ORMTableName)
                .Append(" WHERE {0}");
            SqlText = SqlBuilder.ToString().ToUpper();
            if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
            {
                Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                SingleTableSQLDic.Add(SQLKey, SqlText);
                TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
            }
            else
            {
                lock (TableSQLDic[TabInfo.ORMTypeName])
                {
                    if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                    {
                        TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                    }
                }
            }
            return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere).ToUpper();
        }
        /// <summary>
        /// 创建物理删除语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_DbDel(TableDefinition TabInfo, dynamic DWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_DbDel(TabInfo, SqlWhere);
        }
        /// <summary>
        /// 创建物理删除语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="EWhere"></param>
        /// <returns></returns>
        public static string SQLBUILDER_DbDel(TableDefinition TabInfo, ExpandoObject EWhere, out List<object> SqlParams)
        {
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, EWhere, out SqlParams);
            return SQLBUILDER_DbDel(TabInfo, SqlWhere);
        }
        #endregion

        #region 创建添加语句
        /// <summary>
        /// 创建添加语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Add(TableDefinition TabInfo, bool ReturnAutoIncreaseColId = true)
        {
            string SQLKey = "MODELADD";
            if (ReturnAutoIncreaseColId)
            {
                SQLKey += "_RAI";
            }
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return TableSQLDic[TabInfo.ORMTypeName][SQLKey];
            }
            StringBuilder SqlBuilder = new StringBuilder();
            string SqlText = "";

            StringBuilder ColsBuilder = new StringBuilder();
            StringBuilder ColParamsBuilder = new StringBuilder();
            foreach (string ColName in TabInfo.ORMColList)
            {
                if (TabInfo.ORM_NoAddCols.Contains(ColName))
                {
                    continue;
                }
                ColsBuilder.Append(ColName).Append(",");
                ColParamsBuilder.Append("@").Append(ColName).Append(",");
            }
            ColsBuilder.Remove(ColsBuilder.Length - 1, 1);
            ColParamsBuilder.Remove(ColParamsBuilder.Length - 1, 1);
            SqlBuilder
                .Append("INSERT INTO ")
                .Append(TabInfo.ORMTableName)
                .Append("(")
                .Append(ColsBuilder.ToString())
                .Append(")")
                .Append("VALUES(")
                .Append(ColParamsBuilder.ToString())
                .Append(")");
            if (ReturnAutoIncreaseColId)
            {
                switch (TabInfo.ORMConnectionProvider)
                {
                    case "1":
                    case "2":
                        SqlBuilder.Append(";SELECT @@IDENTITY;");
                        break;
                    default:
                        break;
                }
            }
            SqlText = SqlBuilder.ToString().ToUpper();
            if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
            {
                Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                SingleTableSQLDic.Add(SQLKey, SqlText);
                TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
            }
            else
            {
                lock (TableSQLDic[TabInfo.ORMTypeName])
                {
                    if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                    {
                        TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                    }
                }
            }
            return SqlText;
        }
        #endregion

        #region 创建更新语句
        /// <summary>
        /// 创建更新语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="SqlWhere"></param>
        /// <param name="SqlSet"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Update(TableDefinition TabInfo, string SqlSet, string SqlWhere)
        {
            string SQLKey = "UPDATE";
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                return string.Format(TableSQLDic[TabInfo.ORMTypeName][SQLKey], string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere, SqlSet).ToUpper();
            }

            StringBuilder SqlBuilder = new StringBuilder();
            string SqlText = "";
            SqlBuilder
                .Append("UPDATE ")
                .Append(TabInfo.ORMTableName)
                .Append(" SET {1}")
                .Append(" WHERE {0}");
            SqlText = SqlBuilder.ToString().ToUpper();
            if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
            {
                Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                SingleTableSQLDic.Add(SQLKey, SqlText);
                TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
            }
            else
            {
                lock (TableSQLDic[TabInfo.ORMTypeName])
                {
                    if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                    {
                        TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                    }
                }
            }
            return string.Format(SqlText, string.IsNullOrEmpty(SqlWhere) ? "1=1" : SqlWhere, SqlSet).ToUpper();
        }
        /// <summary>
        /// 创建动态条件更新语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DSet"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Update(TableDefinition TabInfo, dynamic DSet, dynamic DWhere, out List<object> SqlParams)
        {
            SqlParams = new List<object>();
            List<object> TempSetParams = new List<object>();
            List<object> TempWhereParams = new List<object>();
            string SqlSet = GenerateSQLSetFromDynamic(TabInfo, DSet, out TempSetParams);
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out TempWhereParams);
            if (TempSetParams != null)
            {
                SqlParams.AddRange(TempSetParams);
            }
            if (TempWhereParams != null)
            {
                SqlParams.AddRange(TempWhereParams);
            }
            return SQLBUILDER_Update(TabInfo, SqlSet, SqlWhere);
        }
        /// <summary>
        /// 创建动态条件更新语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="ESet"></param>
        /// <param name="EWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Update(TableDefinition TabInfo, ExpandoObject ESet, ExpandoObject EWhere, out List<object> SqlParams)
        {
            SqlParams = new List<object>();
            List<object> TempSetParams = new List<object>();
            List<object> TempWhereParams = new List<object>();
            string SqlSet = GenerateSQLSetFromExpandoObject(TabInfo, ESet, out TempSetParams);
            string SqlWhere = GenerateSQLWhereFromExpandoObject(TabInfo, EWhere, out TempWhereParams);
            if (TempSetParams != null)
            {
                SqlParams.AddRange(TempSetParams);
            }
            if (TempWhereParams != null)
            {
                SqlParams.AddRange(TempWhereParams);
            }
            return SQLBUILDER_Update(TabInfo, SqlSet, SqlWhere);
        }
        /// <summary>
        /// 创建更新语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Update(TableDefinition TabInfo, HashSet<string> ModifyColumns)
        {
            string SqlText = "";
            string SqlSet = GenerateSQLSetFromTModel(TabInfo, ModifyColumns);
            string SqlWhere = GenerateSQLWhereFromTModel(TabInfo);
            SqlText = SQLBUILDER_Update(TabInfo, SqlSet, SqlWhere);
            return SqlText;
        }
        /// <summary>
        /// 创建更新语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Update(TableDefinition TabInfo, HashSet<string> ModifyColumns, dynamic DWhere, out List<object> SqlParams)
        {
            string SqlSet = GenerateSQLSetFromTModel(TabInfo, ModifyColumns);
            string SqlWhere = GenerateSQLWhereFromDynamic(TabInfo, DWhere, out SqlParams);
            return SQLBUILDER_Update(TabInfo, SqlSet, SqlWhere);
        }
        /// <summary>
        /// 创建更新语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public static string SQLBUILDER_Update(TableDefinition TabInfo, HashSet<string> ModifyColumns, ExpandoObject EWhere, out List<object> SqlParams)
        {
            string SqlSet = GenerateSQLSetFromTModel(TabInfo, ModifyColumns);
            string SqlWhere = GenerateSQLWhereFromExpandoObject(TabInfo, EWhere, out SqlParams);
            return SQLBUILDER_Update(TabInfo, SqlSet, SqlWhere);
        }
        #endregion

        #region 根据动态类型创建条件语句
        /// <summary>
        /// 根据动态类型创建条件语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        private static string GenerateSQLWhereFromDynamic(TableDefinition TabInfo, dynamic DWhere, out List<object> SqlParams)
        {
            StringBuilder SqlWhereBuilder = new StringBuilder();
            SqlParams = new List<object>();
            Type DWhereType = DWhere.GetType();

            foreach (var Property in DWhereType.GetProperties())
            {
                string ColName = Property.Name.ToUpper();
                if (!TabInfo.ORMColList.Contains(ColName))
                {
                    continue;
                }
                SqlWhereBuilder.Append(ColName).Append("=@W").Append(ColName).Append(" AND ");
                var ValObj = Property.GetValue(DWhere);
                if (Property.PropertyType == typeof(DateTime))
                {
                    DateTime TempDT = Convert.ToDateTime(ValObj);
                    if (TempDT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(ValObj);
                    }
                }
                else
                {
                    SqlParams.Add(ValObj);
                }
            }
            return SqlWhereBuilder.Append("1=1").ToString();
        }
        #endregion

        #region 根据动态类型创建设置语句
        /// <summary>
        /// 根据动态类型创建设置语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="DSet"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        private static string GenerateSQLSetFromDynamic(TableDefinition TabInfo, dynamic DSet, out List<object> SqlParams)
        {
            StringBuilder SqlSetBuilder = new StringBuilder();
            SqlParams = new List<object>();
            Type DSetType = DSet.GetType();

            foreach (var Property in DSetType.GetProperties())
            {
                string ColName = Property.Name.ToUpper();
                if (TabInfo.ORM_NoAddCols.Contains(ColName) || TabInfo.ORM_AutoIncreaseColName == ColName || !TabInfo.ORMColList.Contains(ColName))
                {
                    continue;
                }
                SqlSetBuilder.Append(ColName).Append("=@S").Append(ColName).Append(',');
                var ValObj = Property.GetValue(DSet);
                if (Property.PropertyType == typeof(DateTime))
                {
                    DateTime TempDT = Convert.ToDateTime(ValObj);
                    if (TempDT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(ValObj);
                    }
                }
                else
                {
                    SqlParams.Add(ValObj);
                }
            }
            return SqlSetBuilder.ToString().ToUpper();
        }
        #endregion

        #region 根据动态属性创建条件语句
        /// <summary>
        /// 根据动态类型创建条件语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="EWhere"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        private static string GenerateSQLWhereFromExpandoObject(TableDefinition TabInfo, ExpandoObject EWhere, out List<object> SqlParams)
        {
            StringBuilder SqlWhereBuilder = new StringBuilder();
            SqlParams = new List<object>();
            foreach (var Property in (IDictionary<string, object>)EWhere)
            {
                string ColName = Property.Key.ToUpper();
                if (!TabInfo.ORMColList.Contains(ColName))
                {
                    continue;
                }
                SqlWhereBuilder.Append(ColName).Append("=@W").Append(ColName).Append(" AND ");
                var ValObj = Property.Value;
                if (ValObj.GetType() == typeof(DateTime))
                {
                    DateTime TempDT = Convert.ToDateTime(ValObj);
                    if (TempDT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(ValObj);
                    }
                }
                else
                {
                    SqlParams.Add(Property.Value);
                }
            }
            SqlWhereBuilder.Append("1=1");
            return SqlWhereBuilder.ToString().ToUpper();
        }
        #endregion

        #region 根据动态属性创建设置语句
        /// <summary>
        /// 根据动态属性创建设置语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="ESet"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        private static string GenerateSQLSetFromExpandoObject(TableDefinition TabInfo, ExpandoObject ESet, out List<object> SqlParams)
        {
            StringBuilder SqlSetBuilder = new StringBuilder();
            SqlParams = new List<object>();
            foreach (var Property in (IDictionary<string, object>)ESet)
            {
                string ColName = Property.Key.ToUpper();
                if (TabInfo.ORM_NoAddCols.Contains(ColName) || TabInfo.ORM_AutoIncreaseColName == ColName || !TabInfo.ORMColList.Contains(ColName))
                {
                    continue;
                }
                SqlSetBuilder.Append(ColName).Append("=@S").Append(ColName).Append(',');
                var ValObj = Property.Value;
                if (ValObj.GetType() == typeof(DateTime))
                {
                    DateTime TempDT = Convert.ToDateTime(ValObj);
                    if (TempDT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(ValObj);
                    }
                }
                else
                {
                    SqlParams.Add(Property.Value);
                }
            }
            return SqlSetBuilder.ToString().ToUpper();
        }
        #endregion

        #region 根据模型获取设置列语句
        /// <summary>
        /// 根据模型获取设置列语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <returns></returns>
        private static string GenerateSQLSetFromTModel(TableDefinition TabInfo, HashSet<string> ModifyColumns)
        {
            StringBuilder SqlSetBuilder = new StringBuilder();
            string SqlText = "";
            foreach (string ColName in TabInfo.ORMColList)
            {
                if (TabInfo.ORM_NoAddCols.Contains(ColName) || TabInfo.ORM_AutoIncreaseColName == ColName || !ModifyColumns.Contains("[" + ColName + "]"))
                {
                    continue;
                }
                SqlSetBuilder.Append(ColName).Append("=@S").Append(ColName).Append(",");
            }
            SqlSetBuilder.Remove(SqlSetBuilder.Length - 1, 1);
            SqlText = SqlSetBuilder.ToString().ToUpper();
            return SqlText;
        }
        #endregion

        #region 根据模型获取条件列语句
        /// <summary>
        /// 根据模型获取条件列语句
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <returns></returns>
        private static string GenerateSQLWhereFromTModel(TableDefinition TabInfo)
        {
            string SQLKey = "UPDATEMODEL_MODELWHERE";
            StringBuilder SqlWhereBuilder = new StringBuilder();
            string SqlText = "";
            if (TableSQLDic.ContainsKey(TabInfo.ORMTypeName) && TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
            {
                SqlText = TableSQLDic[TabInfo.ORMTypeName][SQLKey];
            }
            else
            {
                if (!string.IsNullOrEmpty(TabInfo.ORM_AutoIncreaseColName))
                {
                    SqlWhereBuilder.Append(TabInfo.ORM_AutoIncreaseColName).Append("=@W").Append(TabInfo.ORM_AutoIncreaseColName).Append(" AND ");
                }
                else
                {
                    foreach (string ColName in TabInfo.ORM_PrimaryKeys)
                    {
                        SqlWhereBuilder.Append(ColName).Append("=@W").Append(ColName).Append(" AND ");
                    }
                }
                SqlText = SqlWhereBuilder.Append("1=1").ToString().ToUpper();
                if (!TableSQLDic.ContainsKey(TabInfo.ORMTypeName))
                {
                    Dictionary<string, string> SingleTableSQLDic = new Dictionary<string, string>();
                    SingleTableSQLDic.Add(SQLKey, SqlText);
                    TableSQLDic.Add(TabInfo.ORMTypeName, SingleTableSQLDic);
                }
                else
                {
                    lock (TableSQLDic[TabInfo.ORMTypeName])
                    {
                        if (!TableSQLDic[TabInfo.ORMTypeName].ContainsKey(SQLKey))
                        {
                            TableSQLDic[TabInfo.ORMTypeName].Add(SQLKey, SqlText);
                        }
                    }
                }
            }
            return SqlText;
        }
        #endregion
    }
}