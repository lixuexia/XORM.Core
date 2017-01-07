using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using XORM.CBase.Data.Common;

namespace XORM.CBase.Tool
{
    public class DynamicBuilder<T> where T : new()
    {
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        private delegate T Load(DataRow dr);
        private Load handler;

        private DynamicBuilder() { }
        /// <summary>
        /// 实用指定数据行构建对象
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public T Build(DataRow dr)
        {
            return handler(dr);
        }
        /// <summary>
        /// 根据字段名，取得真实的属性名
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="_TypeInfo"></param>
        /// <returns></returns>
        private static string GetRealPropertyName(string ColumnName, Type _TypeInfo)
        {
            Dictionary<string, string> dic = ModelBase<object>.GetRealColNameDic(_TypeInfo);
            if (dic.ContainsKey(ColumnName))
            {
                string RealColumnName = dic[ColumnName];
                return RealColumnName;
            }
            else
            {
                return ColumnName;
            }
        }
        /// <summary>
        /// 构建器缓存
        /// </summary>
        private static Dictionary<string, DynamicBuilder<T>> BuilderDic = new Dictionary<string, DynamicBuilder<T>>();
        /// <summary>
        /// 创建对象构建器
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static DynamicBuilder<T> CreateBuilder(DataRow dr)
        {
            Type TypeInfo = typeof(T);
            if(BuilderDic.ContainsKey(TypeInfo.FullName))
            {
                return BuilderDic[TypeInfo.FullName];
            }
            Type tt = typeof(DataRow);
            tt.GetMethod("this");
            DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();

            DynamicMethod method = new DynamicMethod("DynamicCreate", TypeInfo, new Type[] { typeof(DataRow) }, TypeInfo, true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder result = generator.DeclareLocal(TypeInfo);
            generator.Emit(OpCodes.Newobj, TypeInfo.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dr.ColumnNames.Count; i++)
            {
                PropertyInfo propertyInfo = TypeInfo.GetProperty(GetRealPropertyName(dr.ColumnNames[i], TypeInfo));
                Label endIfLabel = generator.DefineLabel();

                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);

                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    if (propertyInfo.PropertyType.IsValueType() || propertyInfo.PropertyType == typeof(string))
                    {
                        generator.Emit(OpCodes.Call, ConvertMethods[propertyInfo.PropertyType]);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                    }
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

                    generator.MarkLabel(endIfLabel);
                }
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }

        //数据类型和对应的强制转换方法的methodinfo，供实体属性赋值时调用
        private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
        {
           {typeof(int),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
           {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
           {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
           {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
           {typeof(decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
           {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
           {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
           {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})}
        };
    }
}