using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XORM.CTool
{
    public class DBS_ColInfo
    {
        public string ColName { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsAutoIncrement { get; set; }

        public bool IsDecimal { get; set; }

        public bool IsDouble { get; set; }

        public bool IsTimestamp { get; set; }

        public bool IsDatetime { get; set; }

        public bool IsString { get; set; }

        public bool IsInt { get; set; }

        public bool IsLong { get; set; }

        public string CodeTypeStr { get; set; }

        public string DefaultVal { get; set; }

        public string DBTypeStr { get; set; }

        public DBS_ColInfo(DataRow dr)
        {
            DBS_TypeMap TM = new DBS_TypeMap();
            #region 类型判断
            if (TM[dr["Xtype_Name"].ToString()].CodeType == "string")
            {
                IsString = true;
            }
            else if (TM[dr["Xtype_Name"].ToString()].CodeType == "decimal")
            {
                IsDecimal = true;
            }
            else if (TM[dr["Xtype_Name"].ToString()].CodeType == "double")
            {
                IsDouble = true;
            }
            else if (TM[dr["Xtype_Name"].ToString()].CodeType == "int" || TM[dr["Xtype_Name"].ToString()].CodeType == "Int32")
            {
                IsInt = true;
            }
            else if (TM[dr["Xtype_Name"].ToString()].CodeType == "Int64")
            {
                IsLong = true;
            }
            else if (TM[dr["Xtype_Name"].ToString()].CodeType == "DateTime")
            {
                IsDatetime = true;
            }
            #endregion

            if (!string.IsNullOrEmpty(dr["dval"].ToString()))
            {
                DefaultVal = dr["dval"].ToString();
            }
            CodeTypeStr = TM[dr["Xtype_Name"].ToString()].CodeType;
            ColName = dr["name"].ToString();
            DBTypeStr = TM[dr["Xtype_Name"].ToString()].DBType;
            if(dr["IsIdentity"].ToString()=="1")
            {
                IsAutoIncrement = true;
            }
            if(dr["PK"].ToString()=="1")
            {
                IsPrimaryKey = true;
            }
        }
    }
}