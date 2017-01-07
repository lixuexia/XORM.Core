using System.Collections.Generic;
using System.Data;
using System.Linq;
using XORM.CBase.Data.Common;
using XORM.CBase.Tool;

namespace XORM.CBase
{
    /// <summary>
    /// 查询结果，对应单行记录
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// 表伪名-数据表字典
        /// </summary>
        internal Dictionary<string, DataRow> RowTabDic = new Dictionary<string, DataRow>();
        /// <summary>
        /// 表类型名-伪名列表
        /// </summary>
        internal Dictionary<string, List<string>> TabSNameDic = new Dictionary<string, List<string>>();

        /// <summary>
        /// 返回类型对应默认表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : ModelBase<T>, new()
        {
            string FullTypeName = typeof(T).FullName;
            if (TabSNameDic.ContainsKey(FullTypeName))
            {
                List<string> SNames = TabSNameDic[FullTypeName];
                if (SNames != null && SNames.Count > 0)
                {
                    string TabSName = SNames[0];
                    DataRow dr = RowTabDic[TabSName];
                    DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                    T Model = DyBuilder.Build(dr);
                    ModelBase<T> TempModel = Model as ModelBase<T>;
                    TempModel.ModifiedColumns.Clear();
                    return (T)TempModel;
                }
            }
            return default(T);
        }
        /// <summary>
        /// 根据表伪名返回数据实体
        /// </summary>
        /// <typeparam name="T">返回实体类型</typeparam>
        /// <param name="TabName">表伪名</param>
        /// <returns></returns>
        public T Get<T>(string TabName) where T : ModelBase<T>, new()
        {
            if (RowTabDic.ContainsKey(TabName))
            {
                DataRow dr = RowTabDic[TabName];
                DynamicBuilder<T> DyBuilder = DynamicBuilder<T>.CreateBuilder(dr);
                T Model = DyBuilder.Build(dr);
                ModelBase<T> TempModel = Model as ModelBase<T>;
                TempModel.ModifiedColumns.Clear();
                return (T)TempModel;
            }
            return default(T);
        }
    }
}