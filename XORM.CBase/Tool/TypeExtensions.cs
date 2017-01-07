using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XORM.CBase.Tool
{
    public static class TypeExtensions
    {
        public static bool IsValueType(this Type typeDef)
        {
            if(typeDef.IsPointer)
            {
                return false;
            }
            return true;
        }
        public static bool Match(this string thisString, string pattern)
        {
            if (thisString == null) return false;
            Regex reg = new Regex(pattern);
            return reg.IsMatch(thisString.ToString());
        }
        public static bool Is<T>(this T thisType, T TypeVal)
        {
            return thisType.GetType() == TypeVal.GetType();
        }
    }
}