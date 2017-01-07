using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace XORM.CBase
{
    public class Query
    {        
        #region 内部变量
        /// <summary>
        /// 实例：输出字段构造
        /// </summary>
        internal StringBuilder OutCols = new StringBuilder();
        /// <summary>
        /// 实例：分页输出列表
        /// </summary>
        internal StringBuilder PageCols = new StringBuilder();
        /// <summary>
        /// 实例：连接文本构造
        /// </summary>
        internal StringBuilder JoinTxt = new StringBuilder();
        /// <summary>
        /// 实例：表-伪名字典
        /// </summary>
        internal Dictionary<string, string> TabNameDic = new Dictionary<string, string>();
        /// <summary>
        /// 实例：表-字段列表字典
        /// </summary>
        internal Dictionary<string, List<string>> TabColDic = new Dictionary<string, List<string>>();
        /// <summary>
        /// 实例：表-类字典
        /// </summary>
        internal Dictionary<string, string> TabClassDic = new Dictionary<string, string>();
        /// <summary>
        /// 实例：输出表伪名列表
        /// </summary>
        internal List<string> OutTabList = new List<string>();
        /// <summary>
        /// 实例：表信息
        /// </summary>
        internal TableDefinition ORM_TabInfo = new TableDefinition();
        /// <summary>
        /// 实例：查询命令
        /// </summary>
        internal SqlCommand MyCmd = new SqlCommand();
        #endregion

        /// <summary>
        /// 通用初始化
        /// </summary>
        internal void Init()
        {
            JoinTxt.Append("[").Append(this.ORM_TabInfo.ORMTableName).Append("] AS SRCTAB ");
            TabNameDic.Add("SRCTAB", this.ORM_TabInfo.ORMTableName);
            TabColDic.Add("SRCTAB", this.ORM_TabInfo.ORMColList);
            TabClassDic.Add("SRCTAB", this.ORM_TabInfo.ORMTypeName);
            foreach (string colName in ORM_TabInfo.ORMColList)
            {
                if (OutCols.Length == 0)
                {
                    OutCols.Append("SRCTAB.").Append("[").Append(colName.ToUpper()).Append("] AS SRCTAB_").Append(colName);
                }
                else
                {
                    OutCols.Append(",").Append("SRCTAB.").Append("[").Append(colName.ToUpper()).Append("] AS SRCTAB_").Append(colName);
                }
                if (PageCols.Length == 0)
                {
                    PageCols.Append("SRCTAB_").Append(colName);
                }
                else
                {
                    PageCols.Append(",SRCTAB_").Append(colName);
                }
            }
        }
        /// <summary>
        /// 是否指定输出列为排重模式
        /// </summary>
        private bool OutDistinct = false;

        #region 最终SQL
        /// <summary>
        /// 最终SQL，连接查询全数据列
        /// </summary>
        internal string SQLTEXT
        {
            get
            {
                StringBuilder sql = new StringBuilder();
                if (!OutDistinct)
                {
                    sql.Append("SELECT ").Append(OutCols.ToString().TrimEnd(',')).Append(" FROM ")
                        .Append(JoinTxt.ToString());
                }
                else
                {
                    sql.Append("SELECT DISTINCT ").Append(OutCols.ToString().TrimEnd(',')).Append(" FROM ")
                        .Append(JoinTxt.ToString());
                }
                return sql.ToString();
            }
        }

        /// <summary>
        /// 最终SQL，记录总数
        /// </summary>
        internal string SQLTEXT_COUNT
        {
            get
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT COUNT(1) FROM ")
                    .Append(JoinTxt.ToString());
                return sql.ToString();
            }
        }

        internal string SingleSql = "";
        /// <summary>
        /// 最终SQL，单项函数，如：sum(),count()等
        /// </summary>
        internal string SQLTEXT_SINGLE
        {
            get
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT ").Append(SingleSql.TrimEnd(',')).Append(" FROM ")
                    .Append(JoinTxt.ToString());
                return sql.ToString();
            }
        }

        internal string PageSort = "";
        /// <summary>
        /// 最终SQL，分页
        /// </summary>
        internal string SQLTEXT_PAGE
        {
            get
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("WITH PST(RN,").Append(PageCols.ToString().TrimEnd(',')).Append(")AS(");
                sql.Append("SELECT ROW_NUMBER() OVER(ORDER BY ").Append(PageSort).Append(") RN,").Append(OutCols.ToString().TrimEnd(',')).Append(" FROM ")
                    .Append(JoinTxt.ToString()).Append(")");
                sql.Append("SELECT RN,").Append(PageCols.ToString().TrimEnd(',')).Append(" FROM PST WHERE RN BETWEEN @SI AND @EI;");
                sql.Append("SELECT COUNT(1) FROM ").Append(JoinTxt.ToString());
                return sql.ToString();
            }
        }
        #endregion
    }
}