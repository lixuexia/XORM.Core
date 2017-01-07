using System;
using System.Data;
using System.Reflection;
using XORM.CBase.Data.Common;

namespace XORM.CBase.Tool
{
    internal class ModelGenerator
    {
        #region 内部方法：根据数据表行记录构建实体类型
        /// <summary>
        /// 内部方法：根据数据表行记录构建实体类型
        /// </summary>
        /// <param name="TabInfo"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T Generate<T>(T ModelObj, TableDefinition TabInfo, DataRow dr)
        {
            int DefineColsCount = TabInfo.ORMColList.Count;
            int ResultColsCount = dr.ColumnNames.Count;
            if (DefineColsCount > ResultColsCount)
            {
                foreach (string ColumnName in dr.ColumnNames)
                {
                    TabInfo.ORM_TypePropDic[ColumnName.ToUpper()].SetValue(ModelObj, dr[ColumnName]);
                }
            }
            else
            {
                foreach (string ColName in TabInfo.ORMColList)
                {
                    if (dr.ColumnNames.Contains(ColName))
                    {
                        if (dr[ColName] != DBNull.Value)
                        {
                            var ColVal = dr[ColName];
                            PropertyInfo Prop = TabInfo.ORM_TypePropDic[ColName.ToUpper()];
                            if (Prop.PropertyType == typeof(int))
                            {
                                Prop.SetValue(ModelObj, Convert.ToInt32(ColVal));
                            }
                            else if (Prop.PropertyType == typeof(long))
                            {
                                Prop.SetValue(ModelObj, Convert.ToInt64(ColVal));
                            }
                            else if (Prop.PropertyType == typeof(decimal))
                            {
                                Prop.SetValue(ModelObj, Convert.ToDecimal(ColVal));
                            }
                            else if (Prop.PropertyType == typeof(DateTime))
                            {
                                Prop.SetValue(ModelObj, Convert.ToDateTime(ColVal));
                            }
                            else if (Prop.PropertyType == typeof(string))
                            {
                                Prop.SetValue(ModelObj, Convert.ToString(ColVal));
                            }                            
                            else
                            {
                                Prop.SetValue(ModelObj, ColVal);
                            }
                        }
                    }
                }
            }
            return ModelObj;
        }
        #endregion        
    }
}