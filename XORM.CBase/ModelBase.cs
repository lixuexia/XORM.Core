using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using XORM.CBase.Attr;
using XORM.CBase.Data.Common;
using XORM.CBase.Tool;

namespace XORM.CBase
{
    public class ModelBase<T> where T : new()
    {
        /// <summary>
        /// 类型-数据库表信息字典：内存缓存
        /// </summary>
        private static Dictionary<string, CBase.TableDefinition> MemTabInfoDic = new Dictionary<string, TableDefinition>();
        /// <summary>
        /// 被修改的字段列表，用于按需更新
        /// </summary>
        public HashSet<string> ModifiedColumns = new HashSet<string>();

        public IConfiguration ConfigService { get; set; }

        #region 创建数据库表信息
        /// <summary>
        /// 创建数据库表信息
        /// </summary>
        /// <returns></returns>
        internal static CBase.Query BuildQuery()
        {
            CBase.Query DbQueryInfo = new CBase.Query();
            DbQueryInfo.ORM_TabInfo = BuildTabInfo();
            return DbQueryInfo;
        }
        /// <summary>
        /// 创建指定类型的数据库表信息
        /// </summary>
        /// <param name="TypeInfo">指定类型信息</param>
        /// <returns></returns>
        internal static CBase.Query BuildQuery(Type TypeInfo)
        {
            CBase.Query DbQueryInfo = new Query();
            DbQueryInfo.ORM_TabInfo = BuildTabInfo(TypeInfo);
            return DbQueryInfo;
        }
        /// <summary>
        /// 获取数据库表字段真实名，区分大小写
        /// </summary>
        /// <param name="TypeInfo"></param>
        /// <returns></returns>
        internal static Dictionary<string,string> GetRealColNameDic(Type TypeInfo)
        {
            CBase.Query DbQueryInfo = new Query();
            DbQueryInfo.ORM_TabInfo = BuildTabInfo(TypeInfo);
            return DbQueryInfo.ORM_TabInfo.ORM_RealColumnName;
        }
        /// <summary>
        /// 构建数据库对应表信息
        /// </summary>
        /// <returns></returns>
        internal static CBase.TableDefinition BuildTabInfo(Type TypeInfo)
        {
            string FullTypeName = TypeInfo.FullName;
            if (MemTabInfoDic.ContainsKey(FullTypeName))
            {
                return MemTabInfoDic[FullTypeName];
            }
            CBase.TableDefinition TabInfo = new CBase.TableDefinition();
            TabInfo.ORMTableName = FullTypeName.Substring(FullTypeName.LastIndexOf('.') + 1);
            TabInfo.ORMTypeName = FullTypeName;
            TabInfo.ORMAssemblyName = TypeInfo.GetTypeInfo().UnderlyingSystemType.AssemblyQualifiedName;
            TabInfo.ORMColList = new List<string>();
            TabInfo.ORM_TypePropDic = new Dictionary<string, PropertyInfo>();
            TabInfo.ORM_NoAddCols = new List<string>();
            TabInfo.ORM_PrimaryKeys = new List<string>();
            TabInfo.ORM_RealColumnName = new Dictionary<string, string>();
            PropertyInfo[] Props = TypeInfo.GetProperties();
            List<string> ColList = new List<string>();
            foreach (var Prop in Props)
            {
                IEnumerable<Attribute> PropAttrArray = Prop.GetCustomAttributes(true);//Prop.GetCustomAttributes(true);
                List<Attribute> PropAttrs = new List<Attribute>();
                foreach (object PropAttrObj in PropAttrArray)
                {
                    PropAttrs.Add((Attribute)PropAttrObj);
                }
                if (PropAttrs != null && PropAttrs.Count > 0)
                {
                    if (PropAttrs.Exists(p => p.GetType() == typeof(DBColAttribute)))
                    {
                        string PropName = Prop.Name.ToUpper();
                        ColList.Add(PropName);
                        TabInfo.ORM_RealColumnName.Add(PropName, Prop.Name);
                        TabInfo.ORM_TypePropDic.Add(PropName, Prop);
                    }
                    if (PropAttrs.Exists(p => p.GetType() == typeof(NoAddAttribute)))
                    {
                        string PropName = Prop.Name.ToUpper();
                        TabInfo.ORM_NoAddCols.Add(PropName);
                    }
                    if (PropAttrs.Exists(p => p.GetType() == typeof(PrimaryKeyAttribute)))
                    {
                        string PropName = Prop.Name.ToUpper();
                        TabInfo.ORM_PrimaryKeys.Add(PropName);
                    }
                    if (PropAttrs.Exists(p => p.GetType() == typeof(AutoInCrementAttribute)))
                    {
                        string PropName = Prop.Name.ToUpper();
                        if (!TabInfo.ORM_NoAddCols.Contains(PropName))
                        {
                            TabInfo.ORM_NoAddCols.Add(PropName);
                        }
                        TabInfo.ORM_AutoIncreaseColName = PropName;
                    }
                }
            }
            TabInfo.ORMColList = ColList;
            if (!MemTabInfoDic.ContainsKey(FullTypeName))
            {
                MemTabInfoDic.Add(FullTypeName, TabInfo);
            }
            DbSourceAttribute DbSourceAttr = TypeInfo.GetTypeInfo().GetCustomAttribute<DbSourceAttribute>(true);
            if (DbSourceAttr != null)
            {
                TabInfo.ORMConnectionMark = DbSourceAttr.DbConnectionMark;
                //var ConnectionConfig = System.Configuration.ConfigurationManager.ConnectionStrings[TabInfo.ORMConnectionMark + "_READ"];
                TabInfo.ORMConnectionProvider = "1";// ConnectionConfig.ProviderName;
            }
            return TabInfo;
        }
        /// <summary>
        /// 构建数据库对应表信息
        /// </summary>
        /// <returns></returns>
        internal static CBase.TableDefinition BuildTabInfo()
        {
            Type TypeInfo = typeof(T);
            return BuildTabInfo(TypeInfo);
        }
        #endregion

        #region 查询统计信息
        /// <summary>
        /// 查询统计信息
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static long Count(string SqlWhere, bool UseReadonlySource = true)
        {
            return Count(SqlWhere, UseReadonlySource, null);
        }
        /// <summary>
        /// 查询统计信息
        /// </summary>
        /// <param name="DWhere">动态条件，如：new {status=1,IsDel=0}</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static long Count(dynamic DWhere, bool UseReadonlySource = true)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Count(DB.ORM_TabInfo, DWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            object CountObj = db.ExecTextScalar(SqlText, SqlParams);
            if (CountObj != null && CountObj != DBNull.Value)
            {
                long CountVal = Convert.ToInt64(CountObj);
                return CountVal;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 查询统计信息
        /// </summary>
        /// <param name="EWhere">动态属性，如：exObj.isdel=0</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static long Count(ExpandoObject EWhere, bool UseReadonlySource = true)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Count(DB.ORM_TabInfo, EWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            object CountObj = db.ExecTextScalar(SqlText, SqlParams);
            if (CountObj != null && CountObj != DBNull.Value)
            {
                long CountVal = Convert.ToInt64(CountObj);
                return CountVal;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 查询统计信息
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static long Count(string SqlWhere, bool UseReadonlySource = true, params object[] SqlParams)
        {
            string SqlText = SqlGenerator.SQLBUILDER_Count(DB.ORM_TabInfo, SqlWhere);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            object CountObj = db.ExecTextScalar(SqlText, SqlParams);
            if (CountObj != null && CountObj != DBNull.Value)
            {
                long CountVal = Convert.ToInt64(CountObj);
                return CountVal;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 查询统计信息
        /// </summary>
        /// <param name="ExpWhere">lamda表达式</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static long Count(Expression<Func<T, bool>> ExpWhere,bool UseReadonlySource = true)
        {
            ResolveExpress re = new Tool.ResolveExpress();
            re.ResolveExpression(re, ExpWhere);
            string SqlWhere = "1=1" + re.SqlWhere;
            string SqlText = SqlGenerator.SQLBUILDER_Count(DB.ORM_TabInfo, SqlWhere);
            object[] SqlParams = re.SqlParams.ToArray();
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            object CountObj = db.ExecTextScalar(SqlText, SqlParams);
            if (CountObj != null && CountObj != DBNull.Value)
            {
                long CountVal = Convert.ToInt64(CountObj);
                return CountVal;
            }
            else
            {
                return -1;
            }
        }
        #endregion

        #region 查询列表记录
        /// <summary>
        /// 查询分页记录
        /// </summary>
        /// <param name="EWhere">动态属性</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="RecordCount">数据记录总数</param>
        /// <param name="PageIndex">页码，默认1</param>
        /// <param name="PageSize">页大小，默认20</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static List<T> List(ExpandoObject EWhere, string SqlOrderBy, out long RecordCount, int PageIndex = 1, int PageSize = 20, bool UseReadonlySource = true)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_List(DB.ORM_TabInfo, EWhere, SqlOrderBy, PageIndex, PageSize, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            List<object> TempParams = new List<object>();
            if (SqlParams != null)
            {
                TempParams.AddRange(SqlParams);
            }
            TempParams.Add(PageIndex * PageSize - PageSize + 1);
            TempParams.Add(PageIndex * PageSize);
            DataSet ds = db.ExecTextDataSet(SqlText, TempParams.ToArray());
            List<T> ReturnObjs = new List<T>();
            RecordCount = -1;
            if (ds != null && ds.Tables.Count == 2)
            {
                Data.Common.DataTable DataDT = ds.Tables[0];
                Data.Common.DataTable CountDT = ds.Tables[1];
                if (DataDT != null && DataDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DataDT.Rows)
                    {
                        DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                        T Model = DyBuilder.Build(dr);
                        ModelBase<T> TempModel = Model as ModelBase<T>;
                        TempModel.ModifiedColumns.Clear();
                        ReturnObjs.Add(Model);
                    }
                }
                if (CountDT != null && CountDT.Rows.Count == 1)
                {
                    long.TryParse(CountDT.Rows[0][0].ToString(), out RecordCount);
                }
                else
                {
                    RecordCount = 0;
                }
            }
            return ReturnObjs;
        }
        /// <summary>
        /// 查询分页记录
        /// </summary>
        /// <param name="DWhere">动态条件</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="RecordCount">数据记录总数</param>
        /// <param name="PageIndex">页码，默认1</param>
        /// <param name="PageSize">页大小，默认20</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static List<T> List(dynamic DWhere, string SqlOrderBy, out long RecordCount, int PageIndex = 1, int PageSize = 20, bool UseReadonlySource = true)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_List(DB.ORM_TabInfo, DWhere, SqlOrderBy, PageIndex, PageSize,out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            List<object> TempParams = new List<object>();
            if (SqlParams != null)
            {
                TempParams.AddRange(SqlParams);
            }
            TempParams.Add(PageIndex * PageSize - PageSize + 1);
            TempParams.Add(PageIndex * PageSize);
            DataSet ds = db.ExecTextDataSet(SqlText, TempParams.ToArray());
            List<T> ReturnObjs = new List<T>();
            RecordCount = -1;
            if (ds != null && ds.Tables.Count == 2)
            {
                XORM.CBase.Data.Common.DataTable DataDT = ds.Tables[0];
                XORM.CBase.Data.Common.DataTable CountDT = ds.Tables[1];
                if (DataDT != null && DataDT.Rows.Count > 0)
                {
                    foreach (XORM.CBase.Data.Common.DataRow dr in DataDT.Rows)
                    {
                        DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                        T Model = DyBuilder.Build(dr);
                        ModelBase<T> TempModel = Model as ModelBase<T>;
                        TempModel.ModifiedColumns.Clear();
                        ReturnObjs.Add(Model);
                    }
                }
                if (CountDT != null && CountDT.Rows.Count == 1)
                {
                    long.TryParse(CountDT.Rows[0][0].ToString(), out RecordCount);
                }
                else
                {
                    RecordCount = 0;
                }
            }
            return ReturnObjs;
        }
        /// <summary>
        /// 查询分页记录
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="RecordCount">数据记录总数</param>
        /// <param name="PageIndex">页码，默认1</param>
        /// <param name="PageSize">页大小，默认20</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static List<T> List(string SqlWhere, string SqlOrderBy, out long RecordCount, int PageIndex = 1, int PageSize = 20, bool UseReadonlySource = true, params object[] SqlParams)
        {
            string SqlText = SqlGenerator.SQLBUILDER_List(DB.ORM_TabInfo, SqlWhere, SqlOrderBy, PageIndex, PageSize);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            List<object> TempParams = new List<object>();
            if (SqlParams != null)
            {
                TempParams.AddRange(SqlParams);
            }
            TempParams.Add(PageIndex * PageSize - PageSize + 1);
            TempParams.Add(PageIndex * PageSize);
            DataSet ds = db.ExecTextDataSet(SqlText, TempParams.ToArray());
            List<T> ReturnObjs = new List<T>();
            RecordCount = -1;
            if (ds != null && ds.Tables.Count == 2)
            {
                Data.Common.DataTable DataDT = ds.Tables[0];
                Data.Common.DataTable CountDT = ds.Tables[1];
                if (DataDT != null && DataDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DataDT.Rows)
                    {
                        DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                        T Model = DyBuilder.Build(dr);
                        ModelBase<T> TempModel = Model as ModelBase<T>;
                        TempModel.ModifiedColumns.Clear();
                        ReturnObjs.Add(Model);
                    }
                }
                if (CountDT != null && CountDT.Rows.Count == 1)
                {
                    long.TryParse(CountDT.Rows[0][0].ToString(), out RecordCount);
                }
                else
                {
                    RecordCount = 0;
                }
            }
            return ReturnObjs;
        }
        /// <summary>
        /// 查询列表记录,最多2000条
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static List<T> List(string SqlWhere, string SqlOrderBy, bool UseReadonlySource = true, params object[] SqlParams)
        {
            long RecordCount = 0L;
            List<T> ObjList = List(SqlWhere, SqlOrderBy, out RecordCount, 1, 2000, UseReadonlySource, SqlParams);
            return ObjList;
        }
        /// <summary>
        /// 查询分页记录
        /// </summary>
        /// <param name="ExpWhere">lamda表达式</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="RecordCount">数据记录总数</param>
        /// <param name="PageIndex">页码，默认1</param>
        /// <param name="PageSize">页大小，默认20</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static List<T> List(Expression<Func<T,bool>> ExpWhere, string SqlOrderBy, out long RecordCount, int PageIndex = 1, int PageSize = 20, bool UseReadonlySource = true)
        {
            ResolveExpress re = new Tool.ResolveExpress();
            re.ResolveExpression(re, ExpWhere);
            string SqlWhere = "1=1" + re.SqlWhere;
            object[] SqlParams = re.SqlParams.ToArray();
            return List(SqlWhere, SqlOrderBy, out RecordCount, PageIndex, PageSize, UseReadonlySource, SqlParams);
        }
        /// <summary>
        /// 查询列表记录,最多2000条
        /// </summary>
        /// <param name="ExpWhere">lamda表达式</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static List<T> List(Expression<Func<T, bool>> ExpWhere, string SqlOrderBy, bool UseReadonlySource = true, params object[] SqlParams)
        {
            long RecordCount = 0L;
            List<T> ObjList = List(ExpWhere, SqlOrderBy, out RecordCount, 1, 2000, UseReadonlySource);
            return ObjList;
        }
        #endregion

        #region 查询单条记录
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="DWhere">动态条件，如：new {IsDel=0,Status=1}</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static T Get(dynamic DWhere,bool UseReadonlySource=true)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Get(DB.ORM_TabInfo, DWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            Data.Common.DataTable dt = db.ExecTextDataTable(SqlText, SqlParams);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                T Model = DyBuilder.Build(dr);
                ModelBase<T> TempModel = Model as ModelBase<T>;
                TempModel.ModifiedColumns.Clear();
                return Model;
            }
            else
            {
                return new T();
            }
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="EWhere">动态属性，如：exOb.IsDel=0</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static T Get(ExpandoObject EWhere, bool UseReadonlySource = true)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Get(DB.ORM_TabInfo, EWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            Data.Common.DataTable dt = db.ExecTextDataTable(SqlText, SqlParams);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                T Model = DyBuilder.Build(dr);
                ModelBase<T> TempModel = Model as ModelBase<T>;
                TempModel.ModifiedColumns.Clear();
                return Model;
            }
            else
            {
                return new T();
            }
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static T Get(string SqlWhere, bool UseReadonlySource = true, params object[] SqlParams)
        {
            return Get(SqlWhere, "", UseReadonlySource, SqlParams);
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static T Get(string SqlWhere, string SqlOrderBy, bool UseReadonlySource = true, params object[] SqlParams)
        {
            string SqlText = SqlGenerator.SQLBUILDER_Get(DB.ORM_TabInfo, SqlWhere, SqlOrderBy);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            Data.Common.DataTable dt = db.ExecTextDataTable(SqlText, SqlParams);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                T Model = DyBuilder.Build(dr);
                ModelBase<T> TempModel = Model as ModelBase<T>;
                TempModel.ModifiedColumns.Clear();
                return Model;
            }
            else
            {
                return new T();
            }
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="ExpWhere">lamda表达式</param>
        /// <param name="SqlOrderBy">取值排序字段，如：Id Asc,CreateTime Desc</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static T Get(Expression<Func<T, bool>> ExpWhere, string SqlOrderBy, bool UseReadonlySource = true)
        {
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, ExpWhere);
            string SqlWhere = "1=1" + re.SqlWhere;
            object[] SqlParams = re.SqlParams.ToArray();
            string SqlText = SqlGenerator.SQLBUILDER_Get(DB.ORM_TabInfo, SqlWhere, SqlOrderBy);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, UseReadonlySource);
            Data.Common.DataTable dt = db.ExecTextDataTable(SqlText, SqlParams);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                T Model = DyBuilder.Build(dr);
                ModelBase<T> TempModel = Model as ModelBase<T>;
                TempModel.ModifiedColumns.Clear();
                return Model;
            }
            else
            {
                return new T();
            }
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="ExpWhere">lamda表达式</param>
        /// <param name="UseReadonlySource">是否实用只读数据源，true-是，false-否</param>
        /// <returns></returns>
        public static T Get(Expression<Func<T, bool>> ExpWhere, bool UseReadonlySource = true)
        {
            return Get(ExpWhere, "", UseReadonlySource);
        }
        #endregion

        #region 保存数据记录
        /// <summary>
        /// 保存数据记录
        /// </summary>
        /// <param name="Model">实体</param>
        /// <param name="NewId">如存在自增列，则为新自增列Id</param>
        /// <returns></returns>
        public static bool Add(T Model, out long NewId)
        {
            NewId = -1;
            string SqlText = SqlGenerator.SQLBUILDER_Add(DB.ORM_TabInfo);
            List<object> SqlParams = new List<object>();
            foreach (string PropName in DB.ORM_TabInfo.ORMColList)
            {
                if (DB.ORM_TabInfo.ORM_NoAddCols.Contains(PropName) || DB.ORM_TabInfo.ORM_AutoIncreaseColName == PropName)
                {
                    continue;
                }
                PropertyInfo Prop = DB.ORM_TabInfo.ORM_TypePropDic[PropName];
                object PropValue = Prop.GetValue(Model);
                if (Prop.PropertyType == typeof(DateTime))
                {
                    DateTime DT = Convert.ToDateTime(PropValue);
                    if (DT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(PropValue);
                    }
                }
                else
                {
                    SqlParams.Add(PropValue);
                }
            }
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            object NewIdObj = null;
            try
            {
                NewIdObj = db.ExecTextScalar(SqlText, SqlParams.ToArray());
                if (NewIdObj != null && NewIdObj != DBNull.Value)
                {
                    long.TryParse(NewIdObj.ToString(), out NewId);
                    if (!string.IsNullOrEmpty(DB.ORM_TabInfo.ORM_AutoIncreaseColName) && DB.ORM_TabInfo.ORM_TypePropDic.ContainsKey(DB.ORM_TabInfo.ORM_AutoIncreaseColName))
                    {
                        PropertyInfo AutoIncreaseProp = DB.ORM_TabInfo.ORM_TypePropDic[DB.ORM_TabInfo.ORM_AutoIncreaseColName];
                        if (AutoIncreaseProp.PropertyType == typeof(int))
                        {
                            DB.ORM_TabInfo.ORM_TypePropDic[DB.ORM_TabInfo.ORM_AutoIncreaseColName].SetValue(Model, Convert.ToInt32(NewId));
                        }
                        else if (AutoIncreaseProp.PropertyType == typeof(short))
                        {
                            DB.ORM_TabInfo.ORM_TypePropDic[DB.ORM_TabInfo.ORM_AutoIncreaseColName].SetValue(Model, Convert.ToInt16(NewId));
                        }
                        else
                        {
                            DB.ORM_TabInfo.ORM_TypePropDic[DB.ORM_TabInfo.ORM_AutoIncreaseColName].SetValue(Model, NewId);
                        }
                    }
                }
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
                return false;
            }
        }
        /// <summary>
        /// 保存数据记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static bool Add(T Model)
        {
            string SqlText = SqlGenerator.SQLBUILDER_Add(DB.ORM_TabInfo, false);
            List<object> SqlParams = new List<object>();
            foreach (string PropName in DB.ORM_TabInfo.ORMColList)
            {
                if (DB.ORM_TabInfo.ORM_NoAddCols.Contains(PropName) || DB.ORM_TabInfo.ORM_AutoIncreaseColName == PropName)
                {
                    continue;
                }
                PropertyInfo Prop = DB.ORM_TabInfo.ORM_TypePropDic[PropName];
                object PropValue = Prop.GetValue(Model);
                if (Prop.PropertyType == typeof(DateTime))
                {
                    DateTime DT = Convert.ToDateTime(PropValue);
                    if (DT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(PropValue);
                    }
                }
                else
                {
                    SqlParams.Add(PropValue);
                }
            }
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            try
            {
                return db.ExecTextNonQuery(SqlText, SqlParams.ToArray()) == 1;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
                return false;
            }
        }
        #endregion

        #region 更新数据记录
        public static bool Update(T Model)
        {
            if (Model is ModelBase<T>)
            {
                ModelBase<T> TempModel = Model as ModelBase<T>;
                return UpdateWhereNeed(TempModel);
            }
            return false;
        }
        /// <summary>
        /// 更新数据模型
        /// </summary>
        /// <param name="Model">数据模型</param>
        /// <returns></returns>
        private static bool UpdateWhereNeed(ModelBase<T> Model)
        {
            string SqlText = SqlGenerator.SQLBUILDER_Update(DB.ORM_TabInfo,Model.ModifiedColumns);
            List<object> SqlParams = new List<object>();
            foreach (string PropName in DB.ORM_TabInfo.ORMColList)
            {
                if (DB.ORM_TabInfo.ORM_NoAddCols.Contains(PropName) || DB.ORM_TabInfo.ORM_AutoIncreaseColName == PropName || !Model.ModifiedColumns.Contains("[" + PropName + "]"))
                {
                    continue;
                }
                PropertyInfo Prop = DB.ORM_TabInfo.ORM_TypePropDic[PropName];
                object PropValue = Prop.GetValue(Model);
                if (Prop.PropertyType == typeof(DateTime))
                {
                    DateTime DT = Convert.ToDateTime(PropValue);
                    if (DT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(PropValue);
                    }
                }
                else
                {
                    SqlParams.Add(PropValue);
                }
            }
            if (!string.IsNullOrEmpty(DB.ORM_TabInfo.ORM_AutoIncreaseColName) && DB.ORM_TabInfo.ORM_TypePropDic.ContainsKey(DB.ORM_TabInfo.ORM_AutoIncreaseColName))
            {
                SqlParams.Add(DB.ORM_TabInfo.ORM_TypePropDic[DB.ORM_TabInfo.ORM_AutoIncreaseColName].GetValue(Model));
            }
            else if (DB.ORM_TabInfo.ORM_PrimaryKeys.Count > 0)
            {
                foreach (string PrimaryKeyName in DB.ORM_TabInfo.ORM_PrimaryKeys)
                {
                    PropertyInfo PrimaryKeyProp = DB.ORM_TabInfo.ORM_TypePropDic[PrimaryKeyName];
                    SqlParams.Add(PrimaryKeyProp.GetValue(Model));
                }
            }
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            try
            {
                return db.ExecTextNonQuery(SqlText, SqlParams.ToArray()) >= 1;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 动态模型更新
        /// </summary>
        /// <param name="DSet">动态更新列模型</param>
        /// <param name="DWhere">动态更新条件模型</param>
        /// <returns></returns>
        public static int Update(dynamic DSet, dynamic DWhere)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Update(DB.ORM_TabInfo, DSet, DWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams.ToArray());
        }
        /// <summary>
        /// 模型更新，动态条件类型
        /// </summary>
        /// <param name="Model">数据模型</param>
        /// <param name="DWhere">动态更新条件模型</param>
        /// <returns></returns>
        public static int Update(T Model, dynamic DWhere)
        {
            if (DWhere == null)
            {
                return Update(Model) ? 1 : 0;
            }
            List<object> SqlParams = new List<object>();
            List<object> SqlWhereParams = new List<object>();
            ModelBase<T> TempModel = Model as ModelBase<T>;
            string SqlText = SqlGenerator.SQLBUILDER_Update(DB.ORM_TabInfo,TempModel.ModifiedColumns, DWhere, out SqlWhereParams);
            foreach (string PropName in DB.ORM_TabInfo.ORMColList)
            {
                if (DB.ORM_TabInfo.ORM_NoAddCols.Contains(PropName) || DB.ORM_TabInfo.ORM_AutoIncreaseColName == PropName)
                {
                    continue;
                }
                PropertyInfo Prop = DB.ORM_TabInfo.ORM_TypePropDic[PropName];
                object PropValue = Prop.GetValue(Model);
                if (Prop.PropertyType == typeof(DateTime))
                {
                    DateTime DT = Convert.ToDateTime(PropValue);
                    if (DT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(PropValue);
                    }
                }
                else
                {
                    SqlParams.Add(PropValue);
                }
            }
            if (SqlWhereParams != null)
            {
                SqlParams.AddRange(SqlWhereParams);
            }
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            try
            {
                return db.ExecTextNonQuery(SqlText, SqlParams.ToArray());
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// 模型更新，动态条件类型
        /// </summary>
        /// <param name="Model">数据模型</param>
        /// <param name="DWhere">动态更新条件模型</param>
        /// <returns></returns>
        public static int Update(T Model, ExpandoObject EWhere)
        {
            if (EWhere == null)
            {
                return Update(Model) ? 1 : 0;
            }
            List<object> SqlParams = new List<object>();
            List<object> SqlWhereParams = new List<object>();
            ModelBase<T> TempModel = Model as ModelBase<T>;
            string SqlText = SqlGenerator.SQLBUILDER_Update(DB.ORM_TabInfo,TempModel.ModifiedColumns, EWhere, out SqlWhereParams);
            foreach (string PropName in DB.ORM_TabInfo.ORMColList)
            {
                if (DB.ORM_TabInfo.ORM_NoAddCols.Contains(PropName) || DB.ORM_TabInfo.ORM_AutoIncreaseColName == PropName)
                {
                    continue;
                }
                PropertyInfo Prop = DB.ORM_TabInfo.ORM_TypePropDic[PropName];
                object PropValue = Prop.GetValue(Model);
                if (Prop.PropertyType == typeof(DateTime))
                {
                    DateTime DT = Convert.ToDateTime(PropValue);
                    if (DT.Year < 1900)
                    {
                        SqlParams.Add(new DateTime(1900, 1, 1));
                    }
                    else
                    {
                        SqlParams.Add(PropValue);
                    }
                }
                else
                {
                    SqlParams.Add(PropValue);
                }
            }
            if (SqlWhereParams != null)
            {
                SqlParams.AddRange(SqlWhereParams);
            }
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            try
            {
                return db.ExecTextNonQuery(SqlText, SqlParams.ToArray());
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// 更新数据记录
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="SqlSet">设置语句，如：TrueName=@TrueName,Remark=@Remark</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static int Update(string SqlSet, string SqlWhere, params object[] SqlParams)
        {
            string SqlText = SqlGenerator.SQLBUILDER_Update(DB.ORM_TabInfo, SqlWhere, SqlSet);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            for (int i = 0; i < SqlParams.Length; i++)
            {
                object SqlParam = SqlParams[i];
                if (SqlParam is DateTime)
                {
                    if (((DateTime)SqlParam).Year < 1900)
                    {
                        SqlParams[i] = new DateTime(1900, 1, 1);
                    }
                }
            }
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        /// <summary>
        /// 动态模型更新
        /// </summary>
        /// <param name="DSet">动态更新列模型</param>
        /// <param name="DWhere">动态更新条件模型</param>
        /// <returns></returns>
        public static int Update(ExpandoObject ESet, ExpandoObject EWhere)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Update(DB.ORM_TabInfo, ESet, EWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams.ToArray());
        }
        #endregion

        #region 逻辑删除数据记录
        /// <summary>
        /// 逻辑删除数据记录
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static int Del(string SqlWhere, params object[] SqlParams)
        {
            string SqlText = SqlGenerator.SQLBUILDER_Del(DB.ORM_TabInfo, SqlWhere);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        /// <summary>
        /// 逻辑删除数据记录
        /// </summary>
        /// <param name="DWhere"></param>
        /// <returns></returns>
        public static int Del(dynamic DWhere)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Del(DB.ORM_TabInfo, DWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        /// <summary>
        /// 逻辑删除数据记录
        /// </summary>
        /// <param name="EWhere"></param>
        /// <returns></returns>
        public static int Del(ExpandoObject EWhere)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_Del(DB.ORM_TabInfo, EWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        #endregion

        #region 物理删除数据记录
        /// <summary>
        /// 物理删除数据记录
        /// </summary>
        /// <param name="SqlWhere">条件语句，如：IsDel=0 AND Status=@Status AND Id>@MinId</param>
        /// <param name="SqlParams">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static int DbDel(string SqlWhere, params object[] SqlParams)
        {
            string SqlText = SqlGenerator.SQLBUILDER_DbDel(DB.ORM_TabInfo, SqlWhere);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        /// <summary>
        /// 逻辑删除数据记录
        /// </summary>
        /// <param name="DWhere"></param>
        /// <returns></returns>
        public static int DbDel(dynamic DWhere)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_DbDel(DB.ORM_TabInfo, DWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        /// <summary>
        /// 逻辑删除数据记录
        /// </summary>
        /// <param name="EWhere"></param>
        /// <returns></returns>
        public static int DbDel(ExpandoObject EWhere)
        {
            List<object> SqlParams = new List<object>();
            string SqlText = SqlGenerator.SQLBUILDER_DbDel(DB.ORM_TabInfo, EWhere, out SqlParams);
            Data.DBHelper db = new Data.DBHelper(DB.ORM_TabInfo.ORMConnectionMark, false);
            return db.ExecTextNonQuery(SqlText, SqlParams);
        }
        #endregion

        #region 数据库访问信息
        /// <summary>
        /// 数据库访问信息
        /// </summary>
        internal static CBase.Query DB
        {
            get
            {
                Query QueryObj = BuildQuery();
                QueryObj.Init();
                return QueryObj;
            }
        }
        #endregion

        #region 内连接
        /// <summary>
        /// 内链接
        /// </summary>
        /// <typeparam name="ModelBase">链接表类型</typeparam>
        /// <param name="SName">链接表伪名</param>
        /// <param name="JoinCondition">链接条件</param>
        /// <param name="ParamsList">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static Query INNER_JOIN<D>(string SName, string JoinCondition, object[] ParamsList)
        {
            return JoinExtension.INNER_JOIN<D>(DB, SName, JoinCondition, ParamsList);
        }
        #endregion

        #region 左连接
        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="ModelBase">链接表类型</typeparam>
        /// <param name="SName">链接表伪名</param>
        /// <param name="JoinCondition">链接条件</param>
        /// <param name="ParamsList">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static Query LEFT_JOIN<D>(string SName, string JoinCondition, object[] ParamsList = null)
        {
            return JoinExtension.LEFT_JOIN<D>(DB, SName, JoinCondition, ParamsList);
        }
        #endregion

        #region 右连接
        /// <summary>
        /// 右链接
        /// </summary>
        /// <typeparam name="ModelBase">链接表类型</typeparam>
        /// <param name="SName">链接表伪名</param>
        /// <param name="JoinCondition">链接条件</param>
        /// <param name="ParamsList">参数集合，如：{Status,MinId}</param>
        /// <returns></returns>
        public static Query RIGHT_JOIN<D>(string SName, string JoinCondition, object[] ParamsList = null)
        {
            return JoinExtension.RIGHT_JOIN<D>(DB, SName, JoinCondition, ParamsList);
        }
        #endregion
    }
}