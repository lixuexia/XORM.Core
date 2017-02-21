using System.Text;
using System.Data;

namespace XORM.CTool
{
    /// <summary>
    /// Model实体生成类
    /// </summary>
    public class DBS_CCClass
    {
        private string _namespaceHead = string.Empty;
        private string _folderHead = string.Empty;
        private string _tableName = string.Empty;
        private DataTable DT = null;
        private string _ConnectionMark = string.Empty;

        public DBS_CCClass(DataTable dt, string NameSpaceHead, string FolderHead, string TableName, string ConnectionMark)
        {
            this.DT = dt;
            this._namespaceHead = NameSpaceHead;
            this._folderHead = FolderHead;
            this._tableName = TableName;
            this._ConnectionMark = ConnectionMark;
        }

        public string CreateFileContent()
        {
            StringBuilder txt = new StringBuilder();
            txt.AppendLine("using System;");
            txt.AppendLine("using XORM.CBase;");
            txt.AppendLine("using XORM.CBase.Attr;");
            txt.AppendLine("");
            txt.AppendLine("namespace " + this._namespaceHead.TrimEnd(new char[] { '.' }));
            txt.AppendLine("{");
            txt.AppendLine("\t/// <summary>");
            txt.AppendLine("\t/// 数据实体类:" + this._tableName);
            txt.AppendLine("\t/// </summary>");
            txt.AppendLine("\t[DbSource(\"" + _ConnectionMark + "\")]");
            txt.AppendLine("\tpublic class " + this._tableName + " : ModelBase<" + this._tableName + ">");
            txt.AppendLine("\t{");

            DBS_TypeMap TM = new DBS_TypeMap();
            txt.AppendLine("\t\t#region 字段、属性");
            foreach (DataRow dr in DT.Rows)
            {
                DBS_ColInfo info = new CTool.DBS_ColInfo(dr);
                if (info.DBTypeStr == "uniqueidentifier")
                {
                    continue;
                }
                if (info.DBTypeStr == "timestamp")
                {
                    continue;
                }
                txt.AppendLine("\t\t/// <summary>");
                txt.AppendLine("\t\t/// " + dr["Description"].ToString().Replace("\r", "").Replace("\n", ""));
                txt.AppendLine("\t\t/// </summary>");
                #region 列特性
                StringBuilder ColAttrBuilder = new StringBuilder();
                if (info.IsAutoIncrement)
                {
                    if (ColAttrBuilder.Length > 0)
                    {
                        ColAttrBuilder.Append(",");
                    }
                    ColAttrBuilder.Append("AutoInCrement");
                }
                if (info.IsPrimaryKey)
                {
                    if (ColAttrBuilder.Length > 0)
                    {
                        ColAttrBuilder.Append(",");
                    }
                    ColAttrBuilder.Append("PrimaryKey");
                }
                if (ColAttrBuilder.Length > 0)
                {
                    ColAttrBuilder.Append(",");
                }
                ColAttrBuilder.Append("DBCol");
                ColAttrBuilder.Append("]");
                ColAttrBuilder.Insert(0, "\t\t[");
                txt.AppendLine(ColAttrBuilder.ToString());
                #endregion
                if (TM.IsKeyWord(info.ColName))
                {
                    txt.AppendLine("\t\tpublic @" + info.CodeTypeStr + " " + info.ColName);
                }
                else
                {
                    txt.AppendLine("\t\tpublic " + info.CodeTypeStr + " " + info.ColName);
                }
                txt.AppendLine("\t\t{");
                txt.AppendLine("\t\t\tget{ return this._" + info.ColName + ";}");
                txt.AppendLine("\t\t\tset{ this._" + info.ColName + " = value; ModifiedColumns.Add(\"[" + info.ColName.ToUpper() + "]\"); }");
                txt.AppendLine("\t\t}");

                if (info.IsString)
                {
                    if (string.IsNullOrEmpty(info.DefaultVal))
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = \"\";");
                    }
                    else
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = \"" + info.DefaultVal.Replace("(", "").Replace(")", "").Replace("'", "") + "\";");
                    }
                }
                else if (info.IsDecimal)
                {
                    if (string.IsNullOrEmpty(info.DefaultVal))
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = 0M;");
                    }
                    else
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = " + info.DefaultVal.Replace("(", "").Replace(")", "").Replace("'", "") + "M;");
                    }
                }
                else if (info.IsDouble)
                {
                    if (string.IsNullOrEmpty(info.DefaultVal))
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = 0;");
                    }
                    else
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = " + info.DefaultVal.Replace("(", "").Replace(")", "").Replace("'", "") + ";");
                    }
                }
                else if (info.IsInt)
                {
                    if (string.IsNullOrEmpty(info.DefaultVal))
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = 0;");
                    }
                    else
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = " + info.DefaultVal.Replace("(", "").Replace(")", "").Replace("'", "") + ";");
                    }
                }
                else if (info.IsLong)
                {
                    if (string.IsNullOrEmpty(info.DefaultVal))
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = 0L;");
                    }
                    else
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = " + info.DefaultVal.Replace("(", "").Replace(")", "").Replace("'", "") + "L;");
                    }
                }
                else if (info.IsDatetime)
                {
                    if (string.IsNullOrEmpty(info.DefaultVal))
                    {
                        txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = DateTime.Now;");
                    }
                    else
                    {
                        if (info.DefaultVal.ToUpper().Contains("GETDATE") || info.DefaultVal.ToUpper().Contains("CURRENT_TIMESTAMP") || info.DefaultVal.ToUpper().Contains("NOW"))
                        {
                            txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = DateTime.Now;");
                        }
                        else
                        {
                            txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + " = DateTime.Parse(\"" + info.DefaultVal.Replace("(", "").Replace(")", "").Replace("'", "") + "\");");
                        }
                    }
                }
                else
                {
                    txt.AppendLine("\t\tprivate " + info.CodeTypeStr + " _" + info.ColName + ";");
                }
            }
            txt.AppendLine("\t\t#endregion");
            txt.AppendLine("\t}");
            txt.Append("}");

            return txt.ToString();
        }
    }
}