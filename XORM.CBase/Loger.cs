using System;
using System.Data.Common;

namespace XORM.CBase.Data
{
    internal class Loger
    {
        internal void Save(Exception e, string cmdText)
        {
            Console.WriteLine(cmdText);
        }
        internal void Save(Exception e, DbCommand cmd)
        {
            Console.WriteLine(cmd.CommandText);
        }

        internal void Save(Exception e, string sQLText, object[] cmdParams)
        {
            Console.WriteLine(sQLText);
        }
    }
}